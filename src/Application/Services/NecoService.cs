using Application.IRepositories;
using Azure.Identity;
using Core;
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
        var aa = _deviceRepository.GetAllAsync();
        return aa;
    }
}