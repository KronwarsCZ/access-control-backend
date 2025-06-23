using Application.IRepositories.Devices;
using Core.Entities;
using Core.Entities.DeviceIoTInfo;
using Infrastructure.Database;
using Infrastructure.UnitOfWork;

namespace Infrastructure.Repositories.Devices;

public class IoTHubDeviceRepository : BasicRepository<DeviceIoTInfo>, IIoTHubDeviceRepository
{
    public IoTHubDeviceRepository(AccessControlDbContext context) : base(context) { }
}
