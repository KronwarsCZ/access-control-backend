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
    
    private class AToManyB<A, B> : IEnumerable<JointRolesKeys>
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
        
        public IEnumerator<JointRolesKeys> GetEnumerator()
        {
            foreach (var elementFromMany in _manyEnumerable)
            {
                switch (_one)
                {
                    case Key one:
                        yield return new JointRolesKeys()
                        {
                            KeyId = one.Id,
                            KeyEntity = null,

                            RoleId = elementFromMany.Id,
                            RoleEntity = null
                        };
                        break;
                    case Role one:
                        yield return new JointRolesKeys()
                        {
                            KeyId = elementFromMany.Id,
                            KeyEntity = null,

                            RoleId = one.Id,
                            RoleEntity = null
                        };
                        break;
                }
            }
        }
    }

    public async Task<IEnumerable<Key>> GetAtoBAsync(Role role, CancellationToken cancellationToken)
    {
        var query = await _context.RolesKeys
            .Where(rk => rk.RoleId == role.Id)
            .Include(rk => rk.KeyEntity)
            .AsNoTracking()
            .ToArrayAsync(cancellationToken);

        var allRoles = query.Select(rk => rk.KeyEntity).ToArray();

        return allRoles.Select(Mapper.Map).ToImmutableArray();
    }

    public async Task<IEnumerable<Role>> GetBtoAAsync(Key key, CancellationToken cancellationToken)
    {
        var query = await _context.RolesKeys
            .Where(rk => rk.KeyId == key.Id)
            .Include(rk => rk.RoleEntity)
            .AsNoTracking()
            .ToArrayAsync(cancellationToken);

        var allRoles = query.Select(rk => rk.RoleEntity).ToArray();

        return allRoles.Select(Mapper.Map).ToImmutableArray();
    }

    public async Task<Guid> AddAtoBAsync(Role role, IEnumerable<Key> keys, CancellationToken cancellationToken)
    {
        var roleToManyKeys = new AToManyB<Role, Key>(role, keys);
        await _context.RolesKeys.AddRangeAsync(roleToManyKeys, cancellationToken);
        return role.Id;
    }

    public async Task<Guid> AddBtoAAsync(Key key, IEnumerable<Role> roles, CancellationToken cancellationToken)
    {
        var keyToManyRoles = new AToManyB<Key, Role>(key, roles);
        await _context.RolesKeys.AddRangeAsync(keyToManyRoles, cancellationToken);
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
