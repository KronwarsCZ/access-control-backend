using System.Buffers.Binary;
using Application.IRepositories.Devices;
using Application.Services.AccessList;
using Application.Services.Devices;
using Core.Entities.Keys;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace UnitTests.Services;

[TestFixture]
public class AccessListServiceTests
{
    private Mock<IDeviceService> _deviceServiceMock = null!;
    private Mock<IDeviceRepository> _deviceRepositoryMock = null!;
    private Mock<ILogger<AccessListService>> _loggerMock = null!;
    private AccessListService _service = null!;

    [SetUp]
    public void SetUp()
    {
        _deviceServiceMock = new Mock<IDeviceService>();
        _deviceRepositoryMock = new Mock<IDeviceRepository>();
        _loggerMock = new Mock<ILogger<AccessListService>>();
        _service = new AccessListService(
            _deviceServiceMock.Object, 
            _deviceRepositoryMock.Object,
            _loggerMock.Object);
    }

    [TestFixture]
    public sealed class ValidateDeviceId : AccessListServiceTests
    {
        [TestCase("DEV-AABBCCDDEEFF", true)]
        [TestCase("DEV-123456789ABC", true)]
        [TestCase("DEV-000000000000", true)]
        [TestCase("DEV-FFFFFFFFFFFF", true)]
        [TestCase("dev-aabbccddeeff", false)] // lowercase
        [TestCase("DEV-aabbccddeeff", false)] // lowercase hex
        [TestCase("AABBCCDDEEFF", false)] // missing prefix
        [TestCase("DEV-AABBCCDDEE", false)] // too short
        [TestCase("DEV-AABBCCDDEEFFGG", false)] // too long
        [TestCase("DEV-GGHHIIJJKKLL", false)] // invalid hex
        [TestCase("", false)]
        [TestCase(null, false)]
        public void ValidateDeviceId_ReturnsExpectedResult(string? deviceId, bool expected)
        {
            // Act
            var result = _service.ValidateDeviceId(deviceId!);

            // Assert
            result.Should().Be(expected);
        }
    }

    [TestFixture]
    public sealed class GenerateBinaryAllowlist : AccessListServiceTests
    {
        [Test]
        public async Task GenerateBinaryAllowlist_ReturnsCorrectHeader()
        {
            // Arrange
            _deviceServiceMock
                .Setup(x => x.GetOpeningKeys(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Key>());

            // Act - use a valid device ID
            var result = await _service.GenerateBinaryAllowlistAsync("DEV-AABBCCDDEEFF", CancellationToken.None);

            // Assert
            result.Should().HaveCount(20); // Header only, no entries
            
            // Check magic number "ALKY"
            var magic = BinaryPrimitives.ReadUInt32LittleEndian(result.AsSpan(0, 4));
            magic.Should().Be(0x414C4B59);
            
            // Check version
            result[4].Should().Be(1);
            
            // Check record count is 0
            var recordCount = BinaryPrimitives.ReadUInt32LittleEndian(result.AsSpan(12, 4));
            recordCount.Should().Be(0);
        }

        [Test]
        public async Task GenerateBinaryAllowlist_ReturnsEmptyForInvalidDeviceId()
        {
            // Act
            var result = await _service.GenerateBinaryAllowlistAsync("INVALID", CancellationToken.None);

            // Assert
            result.Should().HaveCount(20); // Just header with 0 entries
        }
    }

    [TestFixture]
    public sealed class GetAllowedUidsResponse : AccessListServiceTests
    {
        [Test]
        public async Task GetAllowedUidsResponseAsync_ReturnsTimestamp()
        {
            // Arrange
            var beforeTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            // Act
            var result = await _service.GetAllowedUidsResponseAsync("DEV-AABBCCDDEEFF", CancellationToken.None);
            var afterTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            // Assert
            result.Timestamp.Should().BeInRange(beforeTime, afterTime);
            result.Entries.Should().NotBeNull();
        }
    }
}
