using Application.IRepositories.Accounts;
using Application.IRepositories.Devices;
using Application.IRepositories.JoinRepositories;
using Application.IRepositories.Keys;
using Application.IRepositories.Owners;
using Application.IRepositories.Roles;
using Application.IUnitOfWork;
using Application.Services.Time;
using Infrastructure.Database;
using Infrastructure.Repositories.Accounts;
using Infrastructure.Repositories.Devices;
using Infrastructure.Repositories.JoinRepositories;
using Infrastructure.Repositories.Keys;
using Infrastructure.Repositories.Owners;
using Infrastructure.Repositories.Roles;

namespace Infrastructure.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    public UnitOfWork(AccessControlDbContext databaseContext, TimeFreezeService timeService)
    {
        _databaseContext = databaseContext;
        TimeService = timeService;

        AccountRepository = new AccountRepository(databaseContext);
        OwnerRepository = new OwnerRepository(databaseContext);    
        KeyRepository = new KeyRepository(databaseContext);    
        DeviceRepository = new DeviceRepository(databaseContext);
        RoleRepository = new RoleRepository(databaseContext);
        DeviceRoleRepository = new DeviceRoleRepository(databaseContext);
        RoleKeyRepository = new RoleKeyRepository(databaseContext);
    }
    
    public IAccountRepository AccountRepository { get; set; }
    public IDeviceRepository DeviceRepository { get; set; }
    public IOwnerRepository OwnerRepository { get; set; }
    public IKeyRepository KeyRepository { get; set; }
    public IRoleRepository RoleRepository { get; set; }
    public IDeviceRoleRepository DeviceRoleRepository { get; set; }
    public IRoleKeyRepository RoleKeyRepository { get; set; }
    
    
    // time services

    public TimeFreezeService TimeService { get; }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _databaseContext.SaveChangesAsync(cancellationToken);
    }
    
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }


    ~UnitOfWork()
    {
        Dispose(false);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
            if (disposing)
                _databaseContext.Dispose();
        _disposed = true;
    }
    
    private readonly AccessControlDbContext _databaseContext;
    private bool _disposed;
}