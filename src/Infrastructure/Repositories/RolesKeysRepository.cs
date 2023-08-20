using System.Collections;
using System.Collections.Immutable;
using Application.IRepositories;
using Core.Entities;
using Infrastructure.Database;
using Infrastructure.DbEntities;
using Infrastructure.Mapping;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class RolesKeysRepository : IRolesKeysRepository
{
    private readonly AccessControlDbContext _context;
    
    public RolesKeysRepository(AccessControlDbContext context)
    {
        _context = context;
    }
    
    private class AToManyB<A, B> : IEnumerable<DbRolesKeys>
        where A : Entity
        where B : Entity
    {
        private readonly A _one;
        private readonly IEnumerable<B> _manyEnumerable;

        internal AToManyB(A one, IEnumerable<B> manyEnumerable)
        {
            _one = one;
            _manyEnumerable = manyEnumerable;
        }
        
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        
        public IEnumerator<DbRolesKeys> GetEnumerator()
        {
            foreach (var elementFromMany in _manyEnumerable)
            {
                switch (_one)
                {
                    case Key one:
                        yield return new DbRolesKeys()
                        {
                            KeyFId = one.Id,
                            KeyEntity = Mapper.Map(one),

                            RoleFId = elementFromMany.Id,
                            RoleEntity = Mapper.Map(elementFromMany as Role)
                        };
                        break;
                    case Role one:
                        yield return new DbRolesKeys()
                        {
                            KeyFId = elementFromMany.Id,
                            KeyEntity = Mapper.Map(elementFromMany as Key),

                            RoleFId = one.Id,
                            RoleEntity = Mapper.Map(one)
                        };
                        break;
                }
            }
        }
    }

    public async Task<IEnumerable<Key>> GetAtoBAsync(Role role, CancellationToken cancellationToken)
    {
        var query = await _context.RolesKeys
            .Where(rk => rk.RoleFId == role.Id)
            .Include(rk => rk.KeyEntity)
            .AsNoTracking()
            .ToArrayAsync(cancellationToken);

        var allRoles = query.Select(rk => rk.KeyEntity).ToArray();

        return allRoles.Select(Mapper.Map).ToImmutableArray();
    }

    public async Task<IEnumerable<Role>> GetBtoAAsync(Key key, CancellationToken cancellationToken)
    {
        var query = await _context.RolesKeys
            .Where(rk => rk.KeyFId == key.Id)
            .Include(rk => rk.RoleEntity)
            .AsNoTracking()
            .ToArrayAsync(cancellationToken);

        var allRoles = query.Select(rk => rk.RoleEntity).ToArray();

        return allRoles.Select(Mapper.Map).ToImmutableArray();
    }

    public async Task<Guid> AddAtoBAsync(Role role, IEnumerable<Key> keys, CancellationToken cancellationToken)
    {
        var roleToManyKeys = new AToManyB<Role, Key>(role, keys);
        await _context.AddRangeAsync(roleToManyKeys, cancellationToken);
        return role.Id;
    }

    public async Task<Guid> AddBtoAAsync(Key key, IEnumerable<Role> roles, CancellationToken cancellationToken)
    {
        var keyToManyRoles = new AToManyB<Key, Role>(key, roles);
        await _context.AddRangeAsync(keyToManyRoles, cancellationToken);
        return key.Id;
    }

    public void DeleteAtoB(Role role, IEnumerable<Key> keys)
    {
        var roleToManyKeys = new AToManyB<Role, Key>(role, keys);
        _context.RolesKeys.RemoveRange(roleToManyKeys);
    }

    public void DeleteBtoA(Key key, IEnumerable<Role> roles)
    {
        var keyToManyRoles = new AToManyB<Key, Role>(key, roles);
        _context.RolesKeys.RemoveRange(keyToManyRoles);

    }
}
