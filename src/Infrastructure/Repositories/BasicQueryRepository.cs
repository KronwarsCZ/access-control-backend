using Application.UnitOfWorkStuff_to_sort;
using AutoMapper;
using Core.Entities;
using Framework.Extensions;
using Infrastructure.Database;
using Infrastructure.DbEntities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.UnitOfWork.BasicRepositories;

public class BasicQueryRepository<T, TDb> : IBasicQueryRepository<T>
    where T : Entity
    where TDb : DbEntity
{
    public readonly AccessControlDbContext Context;
    protected readonly IMapper _mapper;

    public BasicQueryRepository(AccessControlDbContext context, IMapper mapper)
    {
        
        Context = context;
        _mapper = mapper;

    }

    protected BasicQueryRepository(AccessControlDbContext context)
    {
    }

    public async Task<T> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity =  await Context.Set<TDb>().FindAsync(id);
        return _mapper.Map<T>(entity);
    }

    public async Task<IEnumerable<T>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
    {
        var entities =  await Context.Set<TDb>().Where(e => ids.Contains(e.Id)).ToListAsync(cancellationToken);
        return entities.Select(e => _mapper.Map<T>(e));
    }
    
    private class EntityMappingProfile<T, TDb> : Profile
        where T : Entity
        where TDb : DbEntity
    {
        public EntityMappingProfile()
        {
            CreateMap<TDb, T>();
        }
    }
}


