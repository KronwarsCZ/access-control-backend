using Application.IRepositories;
using Core.Entities;

namespace Application.Services;

public class DeviceService : IDeviceService
{
    private readonly IDeviceRepository DeviceRepository;
    private readonly TimeFreezeService TimeService;
    private readonly IDBContextSaveRepository DbContextSaveRepository;

    public DeviceService(IDeviceRepository deviceRepository, TimeFreezeService timeFreezeService, IDBContextSaveRepository dbContextSaveRepository)
    {
        DeviceRepository = deviceRepository;
        TimeService = timeFreezeService;
        DbContextSaveRepository = dbContextSaveRepository;
    }
    
    public Device Create(string name, DeviceType type)
    {
        return new Device
        {
            CreatedUtc = TimeService.GetUtcTime(),
            UpdatedUtc = TimeService.GetUtcTime(),
            Owner = null,
            Name = name,
            Type = type
        };
    }

    public async Task<IEnumerable<Device>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await DeviceRepository.GetAsync(cancellationToken);
    }

    public async Task<IEnumerable<Guid>> AddAsync(IEnumerable<Device> devices, CancellationToken cancellationToken)
    {
        var ids = await DeviceRepository.AddAsync(devices, cancellationToken);
        await DbContextSaveRepository.SaveContextAsync(cancellationToken);
        return ids;
    }

    public async Task Delete(IEnumerable<Device> devices, CancellationToken cancellationToken)
    {
        DeviceRepository.Delete(devices);
        await DbContextSaveRepository.SaveContextAsync(cancellationToken);
    }

    public async Task Update(IEnumerable<Device> devices, CancellationToken cancellationToken)
    {
        DeviceRepository.Update(devices);
        await DbContextSaveRepository.SaveContextAsync(cancellationToken);
    }

    
}
