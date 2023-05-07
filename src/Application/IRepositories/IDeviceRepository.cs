using Core.Entities;
using Infrastructure.Repositories;

namespace Application.IRepositories;

public interface IDeviceRepository
{
    IEnumerable<Device> GetAllAsync(DeviceFilters filters);
    Device GetByIdAsync(Guid id);
    void Add(IEnumerable<Device> devices);
    void DeleteAsync(IEnumerable<Device> devices);
}