using Core.Entities;

namespace Application.IRepositories;

public interface IDeviceRepository
{
    IEnumerable<Device> GetAllAsync();
    IEnumerable<Device> GetByIdAsync(IEnumerable<Guid> ids);
    IEnumerable<Device> AddAsync(IEnumerable<Device> devices);
    IEnumerable<Device> DeleteAsync(IEnumerable<Device> devices);
}