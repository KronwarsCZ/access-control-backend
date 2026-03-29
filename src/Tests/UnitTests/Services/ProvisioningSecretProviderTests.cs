using System.Security.Cryptography;
using System.Text;
using Application.IUnitOfWork;
using Application.IRepositories.DeviceProvisioning;
using Application.Services.Certificate;
using Core.Entities.DeviceProvisioning;
using Core.Entities.Owners;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace UnitTests.Services;

[TestFixture]
public class ProvisioningSecretProviderTests
{
    private Mock<IUnitOfWork> _unitOfWorkMock = null!;
    private Mock<IProvisioningSecretRepository> _secretRepoMock = null!;
    private Mock<ILogger<ProvisioningSecretProvider>> _loggerMock = null!;
    private ProvisioningSecretProvider _sut = null!;

    [SetUp]
    public void SetUp()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _secretRepoMock = new Mock<IProvisioningSecretRepository>();
        _loggerMock = new Mock<ILogger<ProvisioningSecretProvider>>();

        _unitOfWorkMock.Setup(u => u.ProvisioningSecretRepository).Returns(_secretRepoMock.Object);

        _sut = new ProvisioningSecretProvider(_unitOfWorkMock.Object, _loggerMock.Object);
    }

    private static string HashSecret(string secret)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(secret));
        return Convert.ToBase64String(bytes);
    }

    [TestFixture]
    public sealed class IsProvisioningEnabledAsyncTests : ProvisioningSecretProviderTests
    {
        [Test]
        public async Task ReturnsTrue_WhenActiveSecretsExist()
        {
            var owner = Owner.Create("Test Owner");
            var secret = ProvisioningSecret.Create(HashSecret("secret"), owner.Id);
            secret.Owner = owner;

            _secretRepoMock
                .Setup(r => r.GetAllActiveAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new[] { secret });

            var result = await _sut.IsProvisioningEnabledAsync(CancellationToken.None);

            result.Should().BeTrue();
        }

        [Test]
        public async Task ReturnsFalse_WhenNoActiveSecrets()
        {
            _secretRepoMock
                .Setup(r => r.GetAllActiveAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(Enumerable.Empty<ProvisioningSecret>());

            var result = await _sut.IsProvisioningEnabledAsync(CancellationToken.None);

            result.Should().BeFalse();
        }
    }

    [TestFixture]
    public sealed class ValidateSecretAsyncTests : ProvisioningSecretProviderTests
    {
        [Test]
        public async Task ReturnsNull_ForEmptySecret()
        {
            var result = await _sut.ValidateSecretAsync("", CancellationToken.None);

            result.Should().BeNull();
        }

        [Test]
        public async Task ReturnsNull_ForNullSecret()
        {
            var result = await _sut.ValidateSecretAsync(null!, CancellationToken.None);

            result.Should().BeNull();
        }

        [Test]
        public async Task ReturnsNull_WhenProvisioningDisabled()
        {
            _secretRepoMock
                .Setup(r => r.GetAllActiveAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(Enumerable.Empty<ProvisioningSecret>());

            var result = await _sut.ValidateSecretAsync("some-secret", CancellationToken.None);

            result.Should().BeNull();
        }

        [Test]
        public async Task ReturnsOwner_WhenSecretMatches()
        {
            var plaintext = "my-provisioning-secret";
            var owner = Owner.Create("Acme Corp");
            var secret = ProvisioningSecret.Create(HashSecret(plaintext), owner.Id);
            secret.Owner = owner;

            _secretRepoMock
                .Setup(r => r.GetAllActiveAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new[] { secret });

            var result = await _sut.ValidateSecretAsync(plaintext, CancellationToken.None);

            result.Should().NotBeNull();
            result!.OwnerId.Should().Be(owner.Id);
            result.OwnerName.Should().Be("Acme Corp");
        }

        [Test]
        public async Task ReturnsNull_WhenSecretDoesNotMatch()
        {
            var owner = Owner.Create("Acme Corp");
            var secret = ProvisioningSecret.Create(HashSecret("correct-secret"), owner.Id);
            secret.Owner = owner;

            _secretRepoMock
                .Setup(r => r.GetAllActiveAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new[] { secret });

            var result = await _sut.ValidateSecretAsync("wrong-secret", CancellationToken.None);

            result.Should().BeNull();
        }

        [Test]
        public async Task MatchesCorrectOwner_WhenMultipleSecretsExist()
        {
            var owner1 = Owner.Create("Owner A");
            var owner2 = Owner.Create("Owner B");

            var secret1 = ProvisioningSecret.Create(HashSecret("secret-a"), owner1.Id);
            secret1.Owner = owner1;

            var secret2 = ProvisioningSecret.Create(HashSecret("secret-b"), owner2.Id);
            secret2.Owner = owner2;

            _secretRepoMock
                .Setup(r => r.GetAllActiveAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new[] { secret1, secret2 });

            var result = await _sut.ValidateSecretAsync("secret-b", CancellationToken.None);

            result.Should().NotBeNull();
            result!.OwnerId.Should().Be(owner2.Id);
            result.OwnerName.Should().Be("Owner B");
        }
    }
}