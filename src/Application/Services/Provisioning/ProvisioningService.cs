using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using ApiShared.Dto.IoT;
using Application.IUnitOfWork;
using Application.Services.BasicService;
using Application.Services.Certificate;
using Application.Services.Devices;
using Application.Services.Roles;
using Core.Entities.DeviceProvisioning;
using Core.Entities.Devices;
using Core.Entities.Roles;
using Framework.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Application.Services.Provisioning;

/// <summary>
/// Implementation of device provisioning service.
/// </summary>
public partial class ProvisioningService : BasicService.BasicService, IProvisioningService
{
    private readonly ICertificateService _certificateService;
    private readonly IProvisioningSecretProvider _secretProvider;
    private readonly IDeviceService _deviceService;
    private readonly IRoleService _roleService;
    private readonly CertificateConfiguration _configuration;
    private readonly ILogger<ProvisioningService> _logger;

    [GeneratedRegex(@"^DEV-[A-F0-9]{12}$", RegexOptions.Compiled)]
    private static partial Regex DeviceIdPattern();

    public ProvisioningService(
        IUnitOfWork.IUnitOfWork unitOfWork,
        ICertificateService certificateService,
        IProvisioningSecretProvider secretProvider,
        IDeviceService deviceService,
        IRoleService roleService,
        IOptions<CertificateConfiguration> configuration,
        ILogger<ProvisioningService> logger)
        : base(unitOfWork)
    {
        _certificateService = certificateService;
        _secretProvider = secretProvider;
        _deviceService = deviceService;
        _roleService = roleService;
        _configuration = configuration.Value;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<ProvisioningResponse> ProvisionDeviceAsync(ProvisioningRequest request, CancellationToken cancellationToken)
    {
        if (!ValidateDeviceIdFormat(request.DeviceId))
        {
            throw new ArgumentException("Invalid device ID format", nameof(request.DeviceId));
        }

        // Validate shared secret and resolve the owner from the matching per-owner secret
        var secretResult = await _secretProvider.ValidateSecretAsync(request.SharedSecret, cancellationToken);
        if (secretResult == null)
        {
            _logger.LogWarning("Invalid provisioning secret for device {DeviceId}", request.DeviceId);
            throw new UnauthorizedAccessException("Invalid shared secret");
        }

        var existingProvisionedDevice = await GetProvisionedDeviceAsync(request.DeviceId, cancellationToken);
        if (existingProvisionedDevice != null && existingProvisionedDevice.IsActive)
        {
            _logger.LogWarning("Device already provisioned: {DeviceId}", request.DeviceId);
            throw new InvalidOperationException("Device already provisioned");
        }

        var existingDevice = await UnitOfWork.DeviceRepository.GetByHardwareIdAsync(request.DeviceId, cancellationToken);

        if (!_certificateService.IsConfigured())
        {
            _logger.LogError("Certificate service is not configured. Device provisioning rejected for {DeviceId}", request.DeviceId);
            throw new InvalidOperationException("Certificate service is not configured");
        }

        if (!_certificateService.ValidateCsr(request.Csr, request.DeviceId))
        {
            throw new ArgumentException("Invalid CSR", nameof(request.Csr));
        }

        var signedCert = await _certificateService.SignCsrAsync(request.Csr, request.DeviceId, cancellationToken);
        var caCert = _certificateService.GetCaCertificate();

        var cert = X509Certificate2.CreateFromPem(signedCert);
        var now = DateTime.UtcNow;

        Device? device = existingDevice;

        // Auto-create Device entity using the owner resolved from the secret
        if (_configuration.AutoCreateDevice && existingDevice == null)
        {
            var owner = await UnitOfWork.OwnerRepository.GetByIdAsync(secretResult.OwnerId, cancellationToken);
            if (owner != null)
            {
                device = Device.CreateFromProvisioning(owner, request.DeviceId, DeviceType.Lock);
                await _deviceService.RegisterNewDeviceAsync(device, cancellationToken);

                var defaultRole = Role.Create(owner, device.Name);
                await _roleService.AddAsync(defaultRole.ToEnumerable(), cancellationToken);
                await _deviceService.AttachRoleToDevice(device, defaultRole, cancellationToken);

                _logger.LogInformation(
                    "Auto-created Device {DeviceId} with default Role for hardware {HardwareId} under owner {OwnerId}",
                    device.Id, request.DeviceId, secretResult.OwnerId);
            }
            else
            {
                _logger.LogWarning("Owner {OwnerId} from secret not found for auto-provisioning {HardwareId}",
                    secretResult.OwnerId, request.DeviceId);
            }
        }

        if (existingProvisionedDevice != null)
        {
            existingProvisionedDevice.CertificateSerialNumber = cert.SerialNumber;
            existingProvisionedDevice.CertificateThumbprint = cert.Thumbprint;
            existingProvisionedDevice.CertificateIssuedUtc = now;
            existingProvisionedDevice.CertificateExpiresUtc = cert.NotAfter.ToUniversalTime();
            existingProvisionedDevice.IsActive = true;
            existingProvisionedDevice.RenewalCount = 0;
            existingProvisionedDevice.UpdatedUtc = now;
            existingProvisionedDevice.LinkedDeviceId = device?.Id;
        }
        else
        {
            var provisionedDevice = ProvisionedDevice.Create(
                request.DeviceId,
                cert.SerialNumber,
                cert.Thumbprint,
                now,
                cert.NotAfter.ToUniversalTime());
            
            provisionedDevice.LinkedDeviceId = device?.Id;

            UnitOfWork.ProvisionedDeviceRepository.Add([provisionedDevice]);
        }

        await SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully provisioned device {DeviceId}, Device entity: {DeviceEntityId}", 
            request.DeviceId, device?.Id);

        return new ProvisioningResponse
        {
            ClientCert = signedCert,
            CaCert = caCert
        };
    }

    /// <inheritdoc />
    public async Task<RenewalResponse> RenewCertificateAsync(RenewalRequest request, string clientCertificateCn, CancellationToken cancellationToken)
    {
        if (!ValidateDeviceIdFormat(request.DeviceId))
        {
            throw new ArgumentException("Invalid device ID format", nameof(request.DeviceId));
        }

        if (!string.Equals(clientCertificateCn, request.DeviceId, StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogWarning("mTLS CN mismatch. Expected: {Expected}, Got: {Actual}", 
                request.DeviceId, clientCertificateCn);
            throw new UnauthorizedAccessException("Certificate CN does not match device ID");
        }

        if (!request.Renewal)
        {
            throw new ArgumentException("Renewal flag must be true", nameof(request.Renewal));
        }

        var provisionedDevice = await GetProvisionedDeviceAsync(request.DeviceId, cancellationToken);
        if (provisionedDevice == null || !provisionedDevice.IsActive)
        {
            _logger.LogWarning("Device not provisioned or inactive: {DeviceId}", request.DeviceId);
            throw new InvalidOperationException("Device not provisioned");
        }

        if (!_certificateService.IsConfigured())
        {
            _logger.LogError("Certificate service is not configured. Renewal rejected for {DeviceId}", request.DeviceId);
            throw new InvalidOperationException("Certificate service is not configured");
        }

        if (!_certificateService.ValidateCsr(request.Csr, request.DeviceId))
        {
            throw new ArgumentException("Invalid CSR", nameof(request.Csr));
        }

        var signedCert = await _certificateService.SignCsrAsync(request.Csr, request.DeviceId, cancellationToken);

        var cert = X509Certificate2.CreateFromPem(signedCert);
        var now = DateTime.UtcNow;

        provisionedDevice.CertificateSerialNumber = cert.SerialNumber;
        provisionedDevice.CertificateThumbprint = cert.Thumbprint;
        provisionedDevice.CertificateIssuedUtc = now;
        provisionedDevice.CertificateExpiresUtc = cert.NotAfter.ToUniversalTime();
        provisionedDevice.RenewalCount++;
        provisionedDevice.LastSeenUtc = now;
        provisionedDevice.UpdatedUtc = now;

        await SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully renewed certificate for device {DeviceId}. Renewal count: {Count}",
            request.DeviceId, provisionedDevice.RenewalCount);

        return new RenewalResponse
        {
            ClientCert = signedCert
        };
    }

    /// <inheritdoc />
    public async Task<bool> IsDeviceProvisionedAsync(string deviceId, CancellationToken cancellationToken)
    {
        var device = await GetProvisionedDeviceAsync(deviceId, cancellationToken);
        return device != null && device.IsActive;
    }

    /// <inheritdoc />
    public async Task<ProvisionedDevice?> GetProvisionedDeviceAsync(string deviceId, CancellationToken cancellationToken)
    {
        return await UnitOfWork.ProvisionedDeviceRepository.GetByDeviceIdAsync(deviceId, cancellationToken);
    }

    /// <inheritdoc />
    public bool ValidateDeviceIdFormat(string deviceId)
    {
        if (string.IsNullOrWhiteSpace(deviceId))
        {
            return false;
        }

        return DeviceIdPattern().IsMatch(deviceId);
    }
}
