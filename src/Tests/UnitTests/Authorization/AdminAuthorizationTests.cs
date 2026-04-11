using Core.Entities.Accounts;
using Core.Entities.Owners;
using FluentAssertions;
using Framework.Utilities;
using NUnit.Framework;

namespace UnitTests.Authorization;

[TestFixture]
public sealed class AdminAuthorizationTests
{
    [Test]
    public void AdminAccount_HasAdminRole()
    {
        var owner = Owner.Create("Test Owner");
        var hash = BCrypt.Net.BCrypt.HashPassword("Pass1!", workFactor: 11);
        var account = Account.Create(owner, "admin@test.com", AccountRole.Admin, hash);

        account.Role.Should().Be(AccountRole.Admin);
    }

    [Test]
    public void UserAccount_HasUserRole()
    {
        var owner = Owner.Create("Test Owner");
        var hash = BCrypt.Net.BCrypt.HashPassword("Pass1!", workFactor: 11);
        var account = Account.Create(owner, "user@test.com", AccountRole.User, hash);

        account.Role.Should().Be(AccountRole.User);
    }

    [Test]
    public void RequireAdmin_WithAdminAccount_DoesNotThrow()
    {
        var owner = Owner.Create("Test Owner");
        var hash = BCrypt.Net.BCrypt.HashPassword("Pass1!", workFactor: 11);
        var account = Account.Create(owner, "admin@test.com", AccountRole.Admin, hash);

        var act = () => Check.Contract(account.Role == AccountRole.Admin,
            "Insufficient permissions. This action requires administrator privileges.");

        act.Should().NotThrow();
    }

    [Test]
    public void RequireAdmin_WithUserAccount_ThrowsContractException()
    {
        var owner = Owner.Create("Test Owner");
        var hash = BCrypt.Net.BCrypt.HashPassword("Pass1!", workFactor: 11);
        var account = Account.Create(owner, "user@test.com", AccountRole.User, hash);

        var act = () => Check.Contract(account.Role == AccountRole.Admin,
            "Insufficient permissions. This action requires administrator privileges.");

        act.Should().Throw<ContractException>()
            .WithMessage("*administrator privileges*");
    }

    [Test]
    public void AccountRole_UserIsZero_AdminIsOne()
    {
        ((int)AccountRole.User).Should().Be(0);
        ((int)AccountRole.Admin).Should().Be(1);
    }

    [Test]
    public void JwtAdminClaim_OnlyAddedForAdminRole()
    {
        // Simulates the logic in AuthController
        var adminAccount = CreateAccount(AccountRole.Admin);
        var userAccount = CreateAccount(AccountRole.User);

        ShouldHaveAdminClaim(adminAccount).Should().BeTrue();
        ShouldHaveAdminClaim(userAccount).Should().BeFalse();
    }

    private static bool ShouldHaveAdminClaim(Account account)
    {
        return account.Role == AccountRole.Admin;
    }

    private static Account CreateAccount(AccountRole role)
    {
        var owner = Owner.Create("Test Owner");
        var hash = BCrypt.Net.BCrypt.HashPassword("Pass1!", workFactor: 11);
        return Account.Create(owner, $"{role}@test.com", role, hash);
    }
}
