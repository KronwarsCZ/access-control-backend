using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.Entities;

namespace Application.IRepositories;

public interface IDeviceRepository
{
    Task<IEnumerable<Device>> GetAsync(DeviceFilters filters, CancellationToken cancellationToken);
    Task<Device> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task AddAsync(IEnumerable<Device> devices, CancellationToken cancellationToken);
    void Delete(IEnumerable<Device> devices);
}