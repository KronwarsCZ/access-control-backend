using Application.IRepositories;
using Core.Entities;

namespace Application.Services;

public class DeviceService : IDeviceService
{
    private readonly IDeviceRepository _deviceRepository;
    
    public DeviceService(IDeviceRepository necoRepository)
    {
        _deviceRepository = necoRepository;
    }

    public IEnumerable<Device> ContractedProcessMethod()
    {
        throw new NotImplementedException();
    }
}