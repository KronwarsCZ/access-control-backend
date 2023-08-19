using Application.UnitOfWorkStuff_to_sort;
using AutoMapper;
using Core.Entities;
using Framework.Extensions;
using Infrastructure.Database;
using Infrastructure.DbEntities;
using Infrastructure.Mapping;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.UnitOfWork.BasicRepositories;


public class BasicCommandRepository<T, TDb> : IBasicCommandRepository<T>
    where T : Entity
    where TDb : DbEntity
{
    public readonly AccessControlDbContext Context;
    protected readonly IMapper _mapper;

    public BasicCommandRepository(AccessControlDbContext context, IMapper mapper)
    {
        Context = context;
        _mapper = mapper;
    }

    protected BasicCommandRepository(AccessControlDbContext context)
    {
    }

    public void Add(T entity, CancellationToken cancellationToken = default)
    {
        var dbEntity = _mapper.Map<TDb>(entity);
        Context.Set<TDb>().Add(dbEntity);
    }

    public void Update(T entity, CancellationToken cancellationToken = default)
    {
        var dbEntity = _mapper.Map<TDb>(entity);
        Context.Set<TDb>().Update(dbEntity);    
    }

    public void Delete(T entity, CancellationToken cancellationToken = default)
    {
        var dbEntity = _mapper.Map<TDb>(entity);
        dbEntity.IsDeleted = true;
        Context.Set<TDb>().Update(dbEntity);        
    }

    public void HardDelete(T entity, CancellationToken cancellationToken = default)
    {
        var dbEntity = _mapper.Map<TDb>(entity);
        Context.Set<TDb>().Remove(dbEntity);        
    }
}
