using Application.UnitOfWorkStuff_to_sort;
using Core.Entities;
using Framework.Identification;

namespace Application.Services.Devices;

public class DeviceService : IDeviceService
{
    private readonly IUnitOfWork context;
    public DeviceService(IUnitOfWork context)
    {
        this.context = context;
    }

    public async Task<IEnumerable<Device>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await context.DeviceQueryRepository.GetAllAsync(cancellationToken);
    }
}
