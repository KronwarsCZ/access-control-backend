using Core.Entities;
using Core.Entities.Accounts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure.Database;

public sealed partial class AccessControlDbContext
{
    public DbSet<Account> Accounts { get; set; }

    private void ConfigureAccountEntities(ModelBuilder mb)
    {
        mb.Entity<Account>().Property(a => a.Email).IsRequired().HasMaxLength(64);
        mb.Entity<Account>().Property(a => a.Role).HasConversion(new EnumToStringConverter<AccountRole>());
        mb.Entity<Account>().Property(a => a.PasswordHash).IsRequired().HasMaxLength(256);
        mb.Entity<Account>().Property(a => a.TwoFactorAuthenticationEnabled).IsRequired();
        mb.Entity<Account>().HasOne(a => a.Owner).WithMany(o => o.Accounts).HasForeignKey(a => a.OwnerId);
    }
}
