using System.Net;
using IntegrationTests;
using Xunit;

namespace ApiIntegrationTests.IoT;

/// <summary>
/// Integration tests for OTA API endpoints.
/// </summary>
public class OtaApiTests : AccessControlIntegrationTests
{
    private readonly Uri _versionUri = new("ota/version.txt", UriKind.Relative);
    private readonly Uri _firmwareUri = new("ota/firmware.bin", UriKind.Relative);
    private readonly Uri _healthUri = new("ota/health", UriKind.Relative);

    [Fact]
    public async Task GetFirmwareVersion_ReturnsOk()
    {
        // Act
        var response = await _httpClient.GetAsync(_versionUri);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var content = await response.Content.ReadAsStringAsync();
        Assert.False(string.IsNullOrEmpty(content));
        Assert.True(content.Length <= 31, "Version should be max 31 characters");
    }

    [Fact]
    public async Task GetFirmwareVersion_ReturnsPlainText()
    {
        // Act
        var response = await _httpClient.GetAsync(_versionUri);

        // Assert
        Assert.Equal("text/plain", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task GetFirmwareVersion_ReturnsSemanticVersion()
    {
        // Act
        var response = await _httpClient.GetAsync(_versionUri);
        var version = await response.Content.ReadAsStringAsync();

        // Assert - check it looks like semantic version
        var parts = version.Split('.');
        Assert.True(parts.Length >= 2, "Version should have at least major.minor");
    }

    [Fact]
    public async Task GetFirmwareBinary_ReturnsNotFoundWhenNoFirmware()
    {
        // Act - without firmware file configured, should return 404
        var response = await _httpClient.GetAsync(_firmwareUri);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetHealth_ReturnsHealthStatus()
    {
        // Act
        var response = await _httpClient.GetAsync(_healthUri);

        // Assert - should return OK or ServiceUnavailable depending on firmware availability
        Assert.True(
            response.StatusCode == HttpStatusCode.OK || 
            response.StatusCode == HttpStatusCode.ServiceUnavailable,
            "Health endpoint should return OK or ServiceUnavailable");
    }
}
