using Core.Entities;
using Infrastructure.DbEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure.Database;

public sealed partial class AccessControlDbContext
{
    public DbSet<AccountEntity> Accounts { get; set; }

    private void ConfigureAccountEntities(ModelBuilder mb)
    {
        mb.Entity<AccountEntity>().Property(a => a.Email).IsRequired().HasMaxLength(64);
        mb.Entity<AccountEntity>().Property(a => a.Role).HasConversion(new EnumToStringConverter<AccountRole>());
        mb.Entity<AccountEntity>().Property(a => a.PasswordHash).IsRequired().HasMaxLength(256);
        mb.Entity<AccountEntity>().Property(a => a.TwoFactorAuthenticationEnabled).IsRequired();
        mb.Entity<AccountEntity>().HasOne(a => a.Owner).WithMany(o => o.Accounts).HasForeignKey(a => a.OwnerId);
    }
}
