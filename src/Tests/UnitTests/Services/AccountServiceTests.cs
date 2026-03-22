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
public sealed class AccountServiceTests
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
    public async Task GetAccountByEmailAsync_SecondCallWithSameEmail_UsesRepositoryOnce()
    {
        var email = "user@example.com";
        var account = CreateAccount(email);

        _accountRepository
            .Setup(r => r.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(account);

        await _sut.GetAccountByEmailAsync(email, CancellationToken.None);
        await _sut.GetAccountByEmailAsync(email, CancellationToken.None);

        _accountRepository.Verify(
            r => r.GetByEmailAsync(email, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Test]
    public async Task GetAccountByEmailAsync_TwoDifferentEmails_CallsRepositoryTwice()
    {
        var email1 = "a@example.com";
        var email2 = "b@example.com";
        var acc1 = CreateAccount(email1);
        var acc2 = CreateAccount(email2);

        _accountRepository
            .Setup(r => r.GetByEmailAsync(email1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(acc1);
        _accountRepository
            .Setup(r => r.GetByEmailAsync(email2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(acc2);

        await _sut.GetAccountByEmailAsync(email1, CancellationToken.None);
        await _sut.GetAccountByEmailAsync(email2, CancellationToken.None);

        _accountRepository.Verify(
            r => r.GetByEmailAsync(email1, It.IsAny<CancellationToken>()),
            Times.Once);
        _accountRepository.Verify(
            r => r.GetByEmailAsync(email2, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Test]
    public async Task GetAccountByEmailAsync_DifferentCasing_UsesRepositoryOnce()
    {
        var canonical = "user@example.com";
        var account = CreateAccount(canonical);

        _accountRepository
            .Setup(r => r.GetByEmailAsync(canonical, It.IsAny<CancellationToken>()))
            .ReturnsAsync(account);

        await _sut.GetAccountByEmailAsync("User@Example.COM", CancellationToken.None);
        await _sut.GetAccountByEmailAsync("user@example.com", CancellationToken.None);

        _accountRepository.Verify(
            r => r.GetByEmailAsync(canonical, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Test]
    public async Task GetAccountByEmailAsync_WhenRepositoryReturnsNull_DoesNotCacheAndQueriesEachTime()
    {
        var email = "missing@example.com";

        _accountRepository
            .Setup(r => r.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Account)null!);

        await _sut.GetAccountByEmailAsync(email, CancellationToken.None);
        await _sut.GetAccountByEmailAsync(email, CancellationToken.None);

        _accountRepository.Verify(
            r => r.GetByEmailAsync(email, It.IsAny<CancellationToken>()),
            Times.Exactly(2));
    }

    [Test]
    public async Task GetAccountByEmailAsync_InvalidEmail_ThrowsContractException()
    {
        var act = async () => await _sut.GetAccountByEmailAsync("not-an-email", CancellationToken.None);

        await act.Should().ThrowAsync<ContractException>();
    }

    [Test]
    public async Task ChangePasswordAsync_AfterSuccess_NextGetAccountByEmailLoadsFromRepositoryAgain()
    {
        const string currentPassword = "Current1!";
        const string newPassword = "NewPass1!";
        var email = "user@example.com";
        var account = CreateAccount(email, currentPassword);

        var refreshedAfterChange = CreateAccount(email, newPassword);
        refreshedAfterChange.TokenVersion = account.TokenVersion + 1;

        _accountRepository
            .SetupSequence(r => r.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(account)
            .ReturnsAsync(refreshedAfterChange);

        await _sut.GetAccountByEmailAsync(email, CancellationToken.None);

        await _sut.ChangePasswordAsync(account, currentPassword, newPassword, CancellationToken.None);

        var after = await _sut.GetAccountByEmailAsync(email, CancellationToken.None);

        after.Should().BeSameAs(refreshedAfterChange);
        _accountRepository.Verify(
            r => r.GetByEmailAsync(email, It.IsAny<CancellationToken>()),
            Times.Exactly(2));
    }

    private static Account CreateAccount(string email, string? password = null)
    {
        var owner = Owner.Create("Test owner");
        var pwd = password ?? "Default1!";
        var hash = BCrypt.Net.BCrypt.HashPassword(pwd, workFactor: 11);
        return Account.Create(owner, email, AccountRole.User, hash);
    }
}
