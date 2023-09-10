using Core.Entities;

namespace Application.Services.Areas;

public interface IAreaService
{
    Task<Area> GetByIdAsync(Guid id, CancellationToken cancellationToken);
}
