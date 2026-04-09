using Application.IRepositories.Accounts;
using Application.IUnitOfWork;
using Application.Services.Accounts;
using Core.Entities.Accounts;
using Core.Entities.Owners;
using FluentAssertions;
using Framework.Utilities;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using NUnit.Framework;

namespace UnitTests.Services;

[TestFixture]
public sealed class AccountServiceAuthorizationTests
{
    private Mock<IUnitOfWork> _unitOfWork = null!;
    private Mock<IAccountRepository> _accountRepository = null!;
    private MemoryCache _memoryCache = null!;
    private AccountService _sut = null!;

    [SetUp]
    public void SetUp()
    {
        _unitOfWork = new Mock<IUnitOfWork>();
        _accountRepository = new Mock<IAccountRepository>();
        _memoryCache = new MemoryCache(new MemoryCacheOptions());
        _unitOfWork.Setup(u => u.AccountRepository).Returns(_accountRepository.Object);
        _unitOfWork
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _sut = new AccountService(_unitOfWork.Object, _memoryCache);
    }

    [TearDown]
    public void TearDown()
    {
        _memoryCache.Dispose();
    }

    [Test]
    public async Task AddAccountUnderOwner_DefaultRole_CreatesAdminAccount()
    {
        var owner = Owner.Create("Test Owner");
        var email = "admin@example.com";
        var password = "StrongPass1!";

        _accountRepository
            .Setup(r => r.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Account)null!);

        await _sut.AddAccountUnderOwner(owner, email, password, CancellationToken.None);

        _accountRepository.Verify(
            r => r.Add(It.Is<IEnumerable<Account>>(accounts =>
                accounts.Any(a => a.Role == AccountRole.Admin && a.Email == email))),
            Times.Once);
    }

    [Test]
    public async Task AddAccountUnderOwner_ExplicitUserRole_CreatesUserAccount()
    {
        var owner = Owner.Create("Test Owner");
        var email = "user@example.com";
        var password = "StrongPass1!";

        _accountRepository
            .Setup(r => r.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Account)null!);

        await _sut.AddAccountUnderOwner(owner, email, password, CancellationToken.None, AccountRole.User);

        _accountRepository.Verify(
            r => r.Add(It.Is<IEnumerable<Account>>(accounts =>
                accounts.Any(a => a.Role == AccountRole.User && a.Email == email))),
            Times.Once);
    }

    [Test]
    public async Task AddAccountUnderOwner_ExplicitAdminRole_CreatesAdminAccount()
    {
        var owner = Owner.Create("Test Owner");
        var email = "admin2@example.com";
        var password = "StrongPass1!";

        _accountRepository
            .Setup(r => r.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Account)null!);

        await _sut.AddAccountUnderOwner(owner, email, password, CancellationToken.None, AccountRole.Admin);

        _accountRepository.Verify(
            r => r.Add(It.Is<IEnumerable<Account>>(accounts =>
                accounts.Any(a => a.Role == AccountRole.Admin && a.Email == email))),
            Times.Once);
    }

    [Test]
    public async Task AddAccountUnderOwner_DuplicateEmail_ThrowsContractException()
    {
        var owner = Owner.Create("Test Owner");
        var email = "existing@example.com";
        var password = "StrongPass1!";
        var existingAccount = CreateAccount(owner, email, AccountRole.User);

        _accountRepository
            .Setup(r => r.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingAccount);

        var act = async () => await _sut.AddAccountUnderOwner(owner, email, password, CancellationToken.None);

        await act.Should().ThrowAsync<ContractException>()
            .WithMessage("*already exists*");
    }

    [Test]
    public async Task GetAccountsByOwnerIdAsync_ReturnsAccountsForOwner()
    {
        var ownerId = Guid.NewGuid();
        var accounts = new List<Account>
        {
            CreateAccount(Owner.Create("Test"), "a@example.com", AccountRole.Admin),
            CreateAccount(Owner.Create("Test"), "b@example.com", AccountRole.User)
        };

        _accountRepository
            .Setup(r => r.FindByConditionAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Account, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(accounts);

        var result = await _sut.GetAccountsByOwnerIdAsync(ownerId, CancellationToken.None);

        result.Should().HaveCount(2);
    }

    [Test]
    public async Task DeleteAccountAsync_SoftDeletesAndInvalidatesTokens()
    {
        var owner = Owner.Create("Test Owner");
        var account = CreateAccount(owner, "delete-me@example.com", AccountRole.User);
        var originalTokenVersion = account.TokenVersion;

        await _sut.DeleteAccountAsync(account, CancellationToken.None);

        account.IsDeleted.Should().BeTrue();
        account.TokenVersion.Should().Be(originalTokenVersion + 1);
        _accountRepository.Verify(r => r.Update(It.IsAny<IEnumerable<Account>>()), Times.Once);
    }

    [Test]
    public async Task DeleteAccountAsync_InvalidatesCache()
    {
        var owner = Owner.Create("Test Owner");
        var email = "cached@example.com";
        var account = CreateAccount(owner, email, AccountRole.User);

        _accountRepository
            .Setup(r => r.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(account);

        // Populate cache
        await _sut.GetAccountByEmailAsync(email, CancellationToken.None);

        // Delete account (should invalidate cache)
        await _sut.DeleteAccountAsync(account, CancellationToken.None);

        // Next lookup should hit repository again
        await _sut.GetAccountByEmailAsync(email, CancellationToken.None);

        _accountRepository.Verify(
            r => r.GetByEmailAsync(email, It.IsAny<CancellationToken>()),
            Times.Exactly(2));
    }

    private static Account CreateAccount(Owner owner, string email, AccountRole role)
    {
        var hash = BCrypt.Net.BCrypt.HashPassword("Default1!", workFactor: 11);
        return Account.Create(owner, email, role, hash);
    }
}
