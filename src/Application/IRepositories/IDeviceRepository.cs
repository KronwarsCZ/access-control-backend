using Core.Entities;
using Infrastructure.Repositories;

namespace Application.IRepositories;

public interface IDeviceRepository
{
    IEnumerable<Device> GetAllAsync(DeviceFilters filters);
    IEnumerable<Device> GetByIdAsync(IEnumerable<Guid> ids);
    IEnumerable<Device> AddAsync(IEnumerable<Device> devices);
    IEnumerable<Device> DeleteAsync(IEnumerable<Device> devices);
}