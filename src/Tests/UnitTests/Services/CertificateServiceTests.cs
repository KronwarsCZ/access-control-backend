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
            CertificateValidityDays = 365
        };
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
            // Arrange — env vars CA_CERT / CA_KEY are not set, so CA won't load
            var options = Options.Create(_configuration);
            var service = new CertificateService(options, _loggerMock.Object);

            // Act
            var result = service.IsConfigured();

            // Assert
            result.Should().BeFalse();
        }
    }
}
