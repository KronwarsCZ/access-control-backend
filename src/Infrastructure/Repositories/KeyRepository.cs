using System.Collections.Immutable;
using Application.IRepositories;
using Core.Entities;
using Infrastructure.Database;
using Infrastructure.Mapping;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class KeyRepository : IKeyRepository
{
	public KeyRepository(AccessControlDbContext context)
	{
		_context = context;
	}
    
	private readonly AccessControlDbContext _context;
	
	public async Task<IEnumerable<Key>> GetAsync(KeyFilters filters, CancellationToken cancellationToken)
	{

		var query = await _context.Devices
			.Join(_context.DevicesRoles, d => d.Id, dr => dr.DeviceFId, (d, dr) => new { d, dr })
			.Join(_context.RolesKeys, @t => @t.dr.RoleFId, rk => rk.RoleFId, (@t, rk) => new { @t, rk })
			.Join(_context.Keys, @t => @t.rk.KeyFId, k => k.Id, (@t, k) => new { @t, k })
			.Where(@t => @t.@t.@t.d.Id == (Guid)filters.Id.GetOrElse(new Guid()))
			.Select(@t => t.k)
			.ToListAsync(cancellationToken);

		return query.Select(Mapper.MapKey).ToImmutableList();
	}
}