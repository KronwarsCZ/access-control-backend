using Core;
using Core.Entities;

namespace Application.Services;

public interface IDeviceService
{
    IEnumerable<Device> ContractedProcessMethod();
}