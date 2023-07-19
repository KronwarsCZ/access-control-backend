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
		var dbKeys = await _context.Keys
			.Include(r => r.DbRolesKeys)
			.ThenInclude(rk => rk.DbRole) //.Where(k => filters.Ids.GetOrNull().Contains(k.Id))
			.ToListAsync(cancellationToken);
		
		return dbKeys.Select(Mapper.MapKey);
	}
}