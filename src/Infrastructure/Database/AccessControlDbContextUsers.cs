using Core.Entities;
using Infrastructure.DbEntities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

public sealed partial class AccessControlDbContext
{
    public DbSet<UserEntity> Users { get; set; }

    private void ConfigureUserEntities(ModelBuilder mb)
    {
        mb.Entity<UserEntity>().Property(u => u.Id).IsRequired();
        mb.Entity<UserEntity>().Property(u => u.Email).HasMaxLength(64).IsRequired();
        mb.Entity<UserEntity>().Property(u => u.PasswordHash).HasMaxLength(256).IsRequired();
        mb.Entity<UserEntity>().Property(u => u.TwoFactorAuthenticationEnabled).IsRequired();
    }
}
