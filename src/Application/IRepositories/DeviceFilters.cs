using System;
using System.Collections.Generic;
using Framework.Extensions;
using Framework.Option;

namespace Application.IRepositories;

public class DeviceFilters
{
    public DeviceFilters(IEnumerable<Guid> ids = null)
    {
        Ids = ids.ToOption();
    }

    public Option<IEnumerable<Guid>> Ids;
}