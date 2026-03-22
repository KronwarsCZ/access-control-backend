using Core.Entities.Accounts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

public sealed partial class AccessControlDbContext
{
    public DbSet<Account> Accounts { get; set; }

    private void ConfigureAccountEntities(ModelBuilder mb)
    {
        mb.Entity<Account>().HasIndex(a => a.Email).IsUnique();
        mb.Entity<Account>().Property(a => a.Email).IsRequired().HasMaxLength(64);
        mb.Entity<Account>().Property(a => a.Role);
        mb.Entity<Account>().Property(a => a.PasswordHash).IsRequired().HasMaxLength(255);
        mb.Entity<Account>().Property(a => a.TwoFactorAuthenticationEnabled).IsRequired();
        mb.Entity<Account>().Property(a => a.TokenVersion).IsRequired().HasDefaultValue(0);
        mb.Entity<Account>().HasOne(a => a.Owner).WithMany(o => o.Accounts).HasForeignKey(a => a.OwnerId);
    }
}
