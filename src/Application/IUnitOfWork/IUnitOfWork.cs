using Application.IRepositories.Accounts;
using Application.IRepositories.Devices;
using Application.IRepositories.DeviceProvisioning;
using Application.IRepositories.JoinRepositories;
using Application.IRepositories.Keys;
using Application.IRepositories.Ota;
using Application.IRepositories.Owners;
using Application.IRepositories.Roles;
using Application.IRepositories.Settings;
using Application.Services.Time;

namespace Application.IUnitOfWork;

public interface IUnitOfWork : IDisposable
{
    public IAccountRepository AccountRepository { get; set; }    
    public IDeviceRepository DeviceRepository { get; set; }
    public IOwnerRepository OwnerRepository { get; set; }
    public IKeyRepository KeyRepository { get; set; }
    public IRoleRepository RoleRepository { get; set; }
    public IDeviceRoleRepository DeviceRoleRepository { get; set; }
    public IRoleKeyRepository RoleKeyRepository { get; set; }
    public IProvisionedDeviceRepository ProvisionedDeviceRepository { get; set; }
    public ISystemSettingsRepository SystemSettingsRepository { get; set; }
    public IOwnerFirmwareRepository OwnerFirmwareRepository { get; set; }
    public IProvisioningSecretRepository ProvisioningSecretRepository { get; set; }
    public TimeFreezeService TimeService { get; }


    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
