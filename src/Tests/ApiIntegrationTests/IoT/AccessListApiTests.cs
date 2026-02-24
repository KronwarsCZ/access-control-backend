using System.Net;
using IntegrationTests;
using Xunit;

namespace ApiIntegrationTests.IoT;

/// <summary>
/// Integration tests for access list API endpoints.
/// </summary>
public class AccessListApiTests : AccessControlIntegrationTests
{
    [Fact]
    public async Task GetAllowedUids_ReturnsUnauthorizedWithoutClientCert()
    {
        // Arrange
        var uri = new Uri("api/v1/locks/DEV-AABBCCDDEEFF/allowed", UriKind.Relative);

        // Act - without mTLS client certificate
        var response = await _httpClient.GetAsync(uri);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetAllowedUidsJson_ReturnsUnauthorizedWithoutClientCert()
    {
        // Arrange
        var uri = new Uri("api/v1/locks/DEV-AABBCCDDEEFF/allowed.json", UriKind.Relative);

        // Act
        var response = await _httpClient.GetAsync(uri);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetAllowedUidsBinary_ReturnsUnauthorizedWithoutClientCert()
    {
        // Arrange
        var uri = new Uri("api/v1/locks/DEV-AABBCCDDEEFF/allowed.bin", UriKind.Relative);

        // Act
        var response = await _httpClient.GetAsync(uri);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetAllowedUids_RequiresMtlsAuthentication()
    {
        // Arrange - test that all access list endpoints require mTLS
        var endpoints = new[]
        {
            "api/v1/locks/DEV-AABBCCDDEEFF/allowed",
            "api/v1/locks/DEV-AABBCCDDEEFF/allowed.json",
            "api/v1/locks/DEV-AABBCCDDEEFF/allowed.bin"
        };

        foreach (var endpoint in endpoints)
        {
            // Act
            var response = await _httpClient.GetAsync(new Uri(endpoint, UriKind.Relative));

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.Unauthorized,
                $"Endpoint {endpoint} should require mTLS, but got {response.StatusCode}");
        }
    }
}
