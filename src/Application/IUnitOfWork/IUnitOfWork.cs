using Application.IRepositories.Accounts;
using Application.IRepositories.Devices;
using Application.IRepositories.Keys;
using Application.IRepositories.Owners;
using Application.IRepositories.Roles;
using Application.Services.Time;

namespace Application.IUnitOfWork;

public interface IUnitOfWork : IDisposable
{
    public IAccountRepository AccountRepository { get; set; }    
    public IDeviceRepository DeviceRepository { get; set; }
    public IOwnerRepository OwnerRepository { get; set; }
    public IKeyRepository KeyRepository { get; set; }
    public IRoleRepository RoleRepository { get; set; }
    public TimeFreezeService TimeService { get; }


    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
