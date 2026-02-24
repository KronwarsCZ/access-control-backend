using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using ApiShared.Dto.IoT;
using Application.IUnitOfWork;
using Application.Services.BasicService;
using Application.Services.Certificate;
using Application.Services.Devices;
using Core.Entities.DeviceProvisioning;
using Core.Entities.Devices;
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
    private readonly CertificateConfiguration _configuration;
    private readonly ILogger<ProvisioningService> _logger;

    [GeneratedRegex(@"^DEV-[A-F0-9]{12}$", RegexOptions.Compiled)]
    private static partial Regex DeviceIdPattern();

    public ProvisioningService(
        IUnitOfWork.IUnitOfWork unitOfWork,
        ICertificateService certificateService,
        IProvisioningSecretProvider secretProvider,
        IDeviceService deviceService,
        IOptions<CertificateConfiguration> configuration,
        ILogger<ProvisioningService> logger)
        : base(unitOfWork)
    {
        _certificateService = certificateService;
        _secretProvider = secretProvider;
        _deviceService = deviceService;
        _configuration = configuration.Value;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<ProvisioningResponse> ProvisionDeviceAsync(ProvisioningRequest request, CancellationToken cancellationToken)
    {
        // Validate device ID format
        if (!ValidateDeviceIdFormat(request.DeviceId))
        {
            throw new ArgumentException("Invalid device ID format", nameof(request.DeviceId));
        }

        // Validate shared secret (checks database first, then config)
        if (!await _secretProvider.ValidateSecretAsync(request.SharedSecret, cancellationToken))
        {
            _logger.LogWarning("Invalid provisioning secret for device {DeviceId}", request.DeviceId);
            throw new UnauthorizedAccessException("Invalid shared secret");
        }

        // Check if device is already provisioned
        var existingProvisionedDevice = await GetProvisionedDeviceAsync(request.DeviceId, cancellationToken);
        if (existingProvisionedDevice != null && existingProvisionedDevice.IsActive)
        {
            _logger.LogWarning("Device already provisioned: {DeviceId}", request.DeviceId);
            throw new InvalidOperationException("Device already provisioned");
        }

        // Check if a Device entity already exists with this hardware ID
        var existingDevice = await UnitOfWork.DeviceRepository.GetByHardwareIdAsync(request.DeviceId, cancellationToken);

        // Validate and sign CSR
        if (!_certificateService.ValidateCsr(request.Csr, request.DeviceId))
        {
            throw new ArgumentException("Invalid CSR", nameof(request.Csr));
        }

        var signedCert = await _certificateService.SignCsrAsync(request.Csr, request.DeviceId, cancellationToken);
        var caCert = _certificateService.GetCaCertificate();

        // Parse the signed certificate to get metadata
        var cert = X509Certificate2.CreateFromPem(signedCert);
        var now = DateTime.UtcNow;

        Device? device = existingDevice;

        // Auto-create Device entity if enabled and doesn't exist
        if (_configuration.AutoCreateDevice && existingDevice == null)
        {
            device = await CreateDeviceForProvisioningAsync(request.DeviceId, cancellationToken);
        }

        // Create or update provisioned device record
        if (existingProvisionedDevice != null)
        {
            // Reactivate previously deactivated device
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
            // Create new provisioned device
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

    /// <summary>
    /// Creates a Device entity for a newly provisioned hardware device.
    /// </summary>
    private async Task<Device?> CreateDeviceForProvisioningAsync(string hardwareId, CancellationToken cancellationToken)
    {
        // Get the owner for this device
        var owner = await GetOwnerForProvisioningAsync(cancellationToken);
        if (owner == null)
        {
            _logger.LogWarning("No owner available for auto-provisioning device {HardwareId}", hardwareId);
            return null;
        }

        // Create the device with a friendly default name
        var device = Device.CreateFromProvisioning(owner, hardwareId, DeviceType.Lock);
        
        await _deviceService.RegisterNewDeviceAsync(device, cancellationToken);
        
        _logger.LogInformation("Auto-created Device entity {DeviceId} for hardware {HardwareId} with name '{Name}'",
            device.Id, hardwareId, device.Name);

        return device;
    }

    /// <summary>
    /// Gets the owner to use for auto-provisioned devices.
    /// </summary>
    private async Task<Core.Entities.Owners.Owner?> GetOwnerForProvisioningAsync(CancellationToken cancellationToken)
    {
        // If a default owner is configured, use it
        if (_configuration.DefaultOwnerId.HasValue && _configuration.DefaultOwnerId != Guid.Empty)
        {
            var owner = await UnitOfWork.OwnerRepository.GetByIdAsync(_configuration.DefaultOwnerId.Value, cancellationToken);
            if (owner != null)
            {
                return owner;
            }
            _logger.LogWarning("Configured default owner {OwnerId} not found", _configuration.DefaultOwnerId);
        }

        // Fall back to finding any owner (first one)
        // In a multi-tenant system, you might want to handle this differently
        var owners = await UnitOfWork.OwnerRepository.FindByConditionAsync(o => !o.IsDeleted, cancellationToken);
        var firstOwner = owners.FirstOrDefault();
        
        if (firstOwner == null)
        {
            _logger.LogError("No owners found in the system for auto-provisioning");
        }

        return firstOwner;
    }

    /// <inheritdoc />
    public async Task<RenewalResponse> RenewCertificateAsync(RenewalRequest request, string clientCertificateCn, CancellationToken cancellationToken)
    {
        // Validate device ID format
        if (!ValidateDeviceIdFormat(request.DeviceId))
        {
            throw new ArgumentException("Invalid device ID format", nameof(request.DeviceId));
        }

        // Validate that the mTLS client cert CN matches the requested device ID
        if (!string.Equals(clientCertificateCn, request.DeviceId, StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogWarning("mTLS CN mismatch. Expected: {Expected}, Got: {Actual}", 
                request.DeviceId, clientCertificateCn);
            throw new UnauthorizedAccessException("Certificate CN does not match device ID");
        }

        // Validate renewal flag
        if (!request.Renewal)
        {
            throw new ArgumentException("Renewal flag must be true", nameof(request.Renewal));
        }

        // Check if device is provisioned
        var provisionedDevice = await GetProvisionedDeviceAsync(request.DeviceId, cancellationToken);
        if (provisionedDevice == null || !provisionedDevice.IsActive)
        {
            _logger.LogWarning("Device not provisioned or inactive: {DeviceId}", request.DeviceId);
            throw new InvalidOperationException("Device not provisioned");
        }

        // Validate and sign CSR
        if (!_certificateService.ValidateCsr(request.Csr, request.DeviceId))
        {
            throw new ArgumentException("Invalid CSR", nameof(request.Csr));
        }

        var signedCert = await _certificateService.SignCsrAsync(request.Csr, request.DeviceId, cancellationToken);

        // Parse the signed certificate to get metadata
        var cert = X509Certificate2.CreateFromPem(signedCert);
        var now = DateTime.UtcNow;

        // Update provisioned device record
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
