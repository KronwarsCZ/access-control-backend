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
    IEnumerable<Guid> Add(IEnumerable<Device> devices);
    void Delete(IEnumerable<Device> devices);
}