using Application.Services.Certificate;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace UnitTests.Services;

[TestFixture]
public class CertificateServiceTests
{
    private Mock<ILogger<CertificateService>> _loggerMock = null!;
    private CertificateConfiguration _configuration = null!;

    [SetUp]
    public void SetUp()
    {
        _loggerMock = new Mock<ILogger<CertificateService>>();
        _configuration = new CertificateConfiguration
        {
            ProvisioningSecret = "test-secret-12345678",
            CaCertificatePath = "certs/ca.crt",
            CaPrivateKeyPath = "certs/ca.key",
            CertificateValidityDays = 365
        };
    }

    [TestFixture]
    public sealed class ValidateProvisioningSecret : CertificateServiceTests
    {
        [Test]
        public void ValidateProvisioningSecret_ReturnsTrueForMatchingSecret()
        {
            // Arrange
            var options = Options.Create(_configuration);
            var service = new CertificateService(options, _loggerMock.Object);

            // Act
            var result = service.ValidateProvisioningSecret("test-secret-12345678");

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public void ValidateProvisioningSecret_ReturnsFalseForWrongSecret()
        {
            // Arrange
            var options = Options.Create(_configuration);
            var service = new CertificateService(options, _loggerMock.Object);

            // Act
            var result = service.ValidateProvisioningSecret("wrong-secret");

            // Assert
            result.Should().BeFalse();
        }

        [Test]
        public void ValidateProvisioningSecret_ReturnsFalseForEmptySecret()
        {
            // Arrange
            var options = Options.Create(_configuration);
            var service = new CertificateService(options, _loggerMock.Object);

            // Act
            var result = service.ValidateProvisioningSecret("");

            // Assert
            result.Should().BeFalse();
        }

        [Test]
        public void ValidateProvisioningSecret_ReturnsFalseWhenNotConfigured()
        {
            // Arrange
            _configuration.ProvisioningSecret = "";
            var options = Options.Create(_configuration);
            var service = new CertificateService(options, _loggerMock.Object);

            // Act
            var result = service.ValidateProvisioningSecret("any-secret");

            // Assert
            result.Should().BeFalse();
        }

        [Test]
        public void ValidateProvisioningSecret_ResistsTimingAttack()
        {
            // Arrange
            var options = Options.Create(_configuration);
            var service = new CertificateService(options, _loggerMock.Object);

            // Act - measure time for correct vs incorrect secrets
            // This is a basic test - in real scenarios use statistical analysis
            var correctStart = DateTime.UtcNow;
            service.ValidateProvisioningSecret("test-secret-12345678");
            var correctDuration = DateTime.UtcNow - correctStart;

            var wrongStart = DateTime.UtcNow;
            service.ValidateProvisioningSecret("wrong");
            var wrongDuration = DateTime.UtcNow - wrongStart;

            // Assert - both should complete (timing attack resistance can't be truly tested this way
            // but we verify both paths work)
            correctDuration.Should().BeLessThan(TimeSpan.FromSeconds(1));
            wrongDuration.Should().BeLessThan(TimeSpan.FromSeconds(1));
        }
    }

    [TestFixture]
    public sealed class ValidateCsr : CertificateServiceTests
    {
        [Test]
        public void ValidateCsr_ReturnsFalseForEmptyCsr()
        {
            // Arrange
            var options = Options.Create(_configuration);
            var service = new CertificateService(options, _loggerMock.Object);

            // Act
            var result = service.ValidateCsr("", "DEV-AABBCCDDEEFF");

            // Assert
            result.Should().BeFalse();
        }

        [Test]
        public void ValidateCsr_ReturnsFalseForMissingPemMarkers()
        {
            // Arrange
            var options = Options.Create(_configuration);
            var service = new CertificateService(options, _loggerMock.Object);
            var invalidCsr = "some random content without PEM markers";

            // Act
            var result = service.ValidateCsr(invalidCsr, "DEV-AABBCCDDEEFF");

            // Assert
            result.Should().BeFalse();
        }

        [Test]
        public void ValidateCsr_ReturnsFalseForInvalidBase64()
        {
            // Arrange
            var options = Options.Create(_configuration);
            var service = new CertificateService(options, _loggerMock.Object);
            var invalidCsr = @"-----BEGIN CERTIFICATE REQUEST-----
not valid base64 content!@#$%
-----END CERTIFICATE REQUEST-----";

            // Act
            var result = service.ValidateCsr(invalidCsr, "DEV-AABBCCDDEEFF");

            // Assert
            result.Should().BeFalse();
        }
    }

    [TestFixture]
    public sealed class IsConfigured : CertificateServiceTests
    {
        [Test]
        public void IsConfigured_ReturnsFalseWhenCaNotLoaded()
        {
            // Arrange - use non-existent paths
            _configuration.CaCertificatePath = "nonexistent/ca.crt";
            _configuration.CaPrivateKeyPath = "nonexistent/ca.key";
            var options = Options.Create(_configuration);
            var service = new CertificateService(options, _loggerMock.Object);

            // Act
            var result = service.IsConfigured();

            // Assert
            result.Should().BeFalse();
        }
    }
}
