using Core.Entities;
using Infrastructure.DbEntities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

public sealed partial class AccessControlDbContext
{
    public DbSet<AccountEntity> Users { get; set; }

    private void ConfigureUserEntities(ModelBuilder mb)
    {
        mb.Entity<AccountEntity>().Property(u => u.Id).IsRequired();
        mb.Entity<AccountEntity>().Property(u => u.Email).IsRequired().HasMaxLength(64);
        mb.Entity<AccountEntity>().Property(u => u.Role).IsRequired().HasMaxLength(64);
        mb.Entity<AccountEntity>().Property(u => u.PasswordHash).IsRequired().HasMaxLength(256);
        mb.Entity<AccountEntity>().Property(u => u.TwoFactorAuthenticationEnabled).IsRequired();
    }
}
