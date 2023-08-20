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

    public Device Create(Owner owner, string name)
    {
        return new Device
        {
            Id = SequentialGuid.Create(),
            Name = name,
            Type = DeviceType.Lock,
            Owner = owner,
            OwnerId = owner.Id,
            IsDeleted = false,
            CreatedUtc = context.TimeService.GetUtcTime(),
            UpdatedUtc = context.TimeService.GetUtcTime()
        };
    }
}
