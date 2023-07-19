using Framework.Extensions;
using Framework.Option;

namespace Application.IRepositories;

public class KeyFilters
{
	public KeyFilters(IEnumerable<Guid> ids = null)
	{
		Ids = ids.ToOption();
	}

	public Option<IEnumerable<Guid>> Ids;
}