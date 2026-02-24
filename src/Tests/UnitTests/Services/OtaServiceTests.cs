using Application.Services.Ota;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace UnitTests.Services;

[TestFixture]
public sealed class OtaServiceTests
{
    private Mock<ILogger<OtaService>> _loggerMock = null!;
    private OtaConfiguration _configuration = null!;

    [SetUp]
    public void SetUp()
    {
        _loggerMock = new Mock<ILogger<OtaService>>();
        _configuration = new OtaConfiguration
        {
            FirmwareVersion = "1.2.3",
            FirmwarePath = "firmware/test.bin",
            MaxFirmwareSizeBytes = 4 * 1024 * 1024
        };
    }

    [Test]
    public void GetFirmwareVersion_ReturnsConfiguredVersion()
    {
        // Arrange
        var options = Options.Create(_configuration);
        var service = new OtaService(options, _loggerMock.Object);

        // Act
        var version = service.GetFirmwareVersion();

        // Assert
        version.Should().Be("1.2.3");
    }

    [Test]
    public void GetFirmwareVersion_ReturnsDefaultForEmptyVersion()
    {
        // Arrange
        _configuration.FirmwareVersion = "";
        var options = Options.Create(_configuration);
        var service = new OtaService(options, _loggerMock.Object);

        // Act
        var version = service.GetFirmwareVersion();

        // Assert
        version.Should().Be("0.0.0");
    }

    [Test]
    public void GetFirmwareVersion_TruncatesVersionOver31Chars()
    {
        // Arrange
        _configuration.FirmwareVersion = new string('1', 50);
        var options = Options.Create(_configuration);
        var service = new OtaService(options, _loggerMock.Object);

        // Act
        var version = service.GetFirmwareVersion();

        // Assert
        version.Should().Be("0.0.0"); // Returns default because it's invalid
    }

    [Test]
    public void IsFirmwareAvailable_ReturnsFalseWhenFileDoesNotExist()
    {
        // Arrange
        _configuration.FirmwarePath = "nonexistent/path/firmware.bin";
        var options = Options.Create(_configuration);
        var service = new OtaService(options, _loggerMock.Object);

        // Act
        var available = service.IsFirmwareAvailable();

        // Assert
        available.Should().BeFalse();
    }

    [Test]
    public void GetFirmwareFilePath_ReturnsNullWhenPathNotConfigured()
    {
        // Arrange
        _configuration.FirmwarePath = "";
        var options = Options.Create(_configuration);
        var service = new OtaService(options, _loggerMock.Object);

        // Act
        var path = service.GetFirmwareFilePath();

        // Assert
        path.Should().BeNull();
    }

    [Test]
    public void GetFirmwareFilePath_ReturnsNullForNonExistentFile()
    {
        // Arrange
        _configuration.FirmwarePath = "nonexistent.bin";
        var options = Options.Create(_configuration);
        var service = new OtaService(options, _loggerMock.Object);

        // Act
        var path = service.GetFirmwareFilePath();

        // Assert
        path.Should().BeNull();
    }

    [Test]
    public void GetFirmwareFilePath_BlocksPathTraversal()
    {
        // Arrange - attempt path traversal
        _configuration.FirmwarePath = "../../../etc/passwd";
        var options = Options.Create(_configuration);
        var service = new OtaService(options, _loggerMock.Object);

        // Act
        var path = service.GetFirmwareFilePath();

        // Assert
        path.Should().BeNull();
    }
}
