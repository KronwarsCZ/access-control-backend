using Application.IRepositories;
using Application.IRepositories.Accounts;
using Application.IRepositories.Devices;
using Application.IRepositories.Keys;
using Application.IRepositories.Owners;
using Application.IRepositories.Roles;
using Application.Services.Time;
using Application.UnitOfWorkStuff_to_sort;
using Infrastructure.Database;
using Infrastructure.Repositories.Accounts;
using Infrastructure.Repositories.Devices;
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
        
        SetupQueryRepositories(_databaseContext);
        
        SetupCommandRepositories(_databaseContext);
    }

    private void SetupCommandRepositories(AccessControlDbContext databaseContext)
    {
        AccountCommandRepository = new AccountCommandRepository(databaseContext);
        OwnerCommandRepository = new OwnerCommandRepository(databaseContext);    
        KeyCommandRepository = new KeyCommandRepository(databaseContext);    
        DeviceCommandRepository = new DeviceCommandRepository(databaseContext);
        RoleCommandRepository = new RoleCommandRepository(databaseContext);

    }
    public IAccountCommandRepository AccountCommandRepository { get; set; }
    public IDeviceCommandRepository DeviceCommandRepository { get; set; }
    public IOwnerCommandRepository OwnerCommandRepository { get; set; }
    public IKeyCommandRepository KeyCommandRepository { get; set; }
    public IRoleCommandRepository RoleCommandRepository { get; set; }

    private void SetupQueryRepositories(AccessControlDbContext databaseContext)
    {
        AccountQueryRepository = new AccountQueryRepository(databaseContext);
        OwnerQueryRepository = new OwnerQueryRepository(databaseContext);
        KeyQueryRepository = new KeyQueryRepository(databaseContext);
        DeviceQueryRepository = new DeviceQueryRepository(databaseContext);
        RoleQueryRepository = new RoleQueryRepository(databaseContext);
    }

    public IAccountQueryRepository AccountQueryRepository { get; set; }
    public IDeviceQueryRepository DeviceQueryRepository { get; set; }
    public IOwnerQueryRepository OwnerQueryRepository { get; set; }
    public IKeyQueryRepository KeyQueryRepository { get; set; }
    public IRoleQueryRepository RoleQueryRepository { get; set; }
    
    
    // old repositories
    public IDevicesRolesRepository DevicesRolesRepository { get; set; }

    // time services
    public TimeFreezeService TimeService { get; }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _databaseContext.SaveChangesAsync(cancellationToken);
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