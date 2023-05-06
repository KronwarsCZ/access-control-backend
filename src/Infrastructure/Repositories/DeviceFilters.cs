namespace Infrastructure.Repositories;

public class DeviceFilters
{
    public DeviceFilters(IEnumerable<Guid> ids = null)
    {
        Ids = ids;
    }

    private IEnumerable<Guid> Ids;
}