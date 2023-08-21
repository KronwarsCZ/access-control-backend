using Application.IRepositories;
using Application.IRepositories.Accounts;
using Application.IRepositories.Devices;
using Application.IRepositories.Keys;
using Application.IRepositories.Owners;
using Application.IRepositories.Roles;
using Application.Services.Time;

namespace Application.UnitOfWorkStuff_to_sort;

public interface IUnitOfWork : IDisposable
{
    public IAccountQueryRepository AccountQueryRepository { get; set; }
    public IAccountCommandRepository AccountCommandRepository { get; set; }    
    public IDeviceCommandRepository DeviceCommandRepository { get; set; }
    public IDeviceQueryRepository DeviceQueryRepository { get; set; }
    public IOwnerQueryRepository OwnerQueryRepository { get; set; }
    public IOwnerCommandRepository OwnerCommandRepository { get; set; }
    public IDevicesRolesRepository DevicesRolesRepository { get; set; }
    public IKeyQueryRepository KeyQueryRepository { get; set; }
    public IKeyCommandRepository KeyCommandRepository { get; set; }
    
    public IRoleQueryRepository RoleQueryRepository { get; set; }
    
    public IRoleCommandRepository RoleCommandRepository { get; set; }
    public TimeFreezeService TimeService { get; }


    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
