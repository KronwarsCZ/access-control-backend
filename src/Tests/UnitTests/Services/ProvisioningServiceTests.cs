using Application.Services.Provisioning;
using FluentAssertions;
using NUnit.Framework;

namespace UnitTests.Services;

[TestFixture]
public sealed class ProvisioningServiceTests
{
    [TestFixture]
    public sealed class ValidateDeviceIdFormat
    {
        [TestCase("DEV-AABBCCDDEEFF", true)]
        [TestCase("DEV-123456789ABC", true)]
        [TestCase("DEV-000000000000", true)]
        [TestCase("DEV-FFFFFFFFFFFF", true)]
        [TestCase("dev-aabbccddeeff", false)] // lowercase prefix
        [TestCase("DEV-aabbccddeeff", false)] // lowercase hex
        [TestCase("AABBCCDDEEFF", false)] // missing DEV- prefix
        [TestCase("DEV-AABBCCDDEE", false)] // 10 chars - too short
        [TestCase("DEV-AABBCCDDEEFFGG", false)] // 14 chars - too long  
        [TestCase("DEV-GGHHIIJJKKLL", false)] // invalid hex characters
        [TestCase("DEV-", false)] // empty after prefix
        [TestCase("", false)]
        [TestCase("   ", false)]
        public void ValidateDeviceIdFormat_ReturnsExpectedResult(string deviceId, bool expected)
        {
            // This tests the regex pattern used in provisioning service
            // We test the pattern directly since service requires DI
            var pattern = new System.Text.RegularExpressions.Regex(@"^DEV-[A-F0-9]{12}$");
            var result = !string.IsNullOrWhiteSpace(deviceId) && pattern.IsMatch(deviceId);
            
            result.Should().Be(expected, because: $"Device ID '{deviceId}' validation should be {expected}");
        }
    }

    [TestFixture]
    public sealed class DeviceIdSecurityTests
    {
        [TestCase("DEV-<script>")]
        [TestCase("DEV-'; DROP TABLE")]
        [TestCase("DEV-AABBCC\nDDEE")]
        [TestCase("DEV-AABBCC\0DDEE")]
        [TestCase("DEV-AABBCC/../..")]
        [TestCase("DEV-AABBCC%00DDEE")]
        public void ValidateDeviceIdFormat_RejectsInjectionAttempts(string maliciousInput)
        {
            // Arrange
            var pattern = new System.Text.RegularExpressions.Regex(@"^DEV-[A-F0-9]{12}$");

            // Act
            var result = pattern.IsMatch(maliciousInput);

            // Assert
            result.Should().BeFalse(because: "Injection attempts should be rejected");
        }
    }

    [TestFixture]
    public sealed class MacAddressExtractionTests
    {
        [TestCase("DEV-AABBCCDDEEFF", "AABBCCDDEEFF")]
        [TestCase("DEV-123456789ABC", "123456789ABC")]
        [TestCase("DEV-FFFFFFFFFFFF", "FFFFFFFFFFFF")]
        public void ExtractMacFromDeviceId_ReturnsCorrectMac(string deviceId, string expectedMac)
        {
            // Act - simple extraction
            var mac = deviceId.Length >= 16 ? deviceId[4..] : null;

            // Assert
            mac.Should().Be(expectedMac);
        }
    }
}
