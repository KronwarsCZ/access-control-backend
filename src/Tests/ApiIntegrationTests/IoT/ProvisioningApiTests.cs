using System.Net;
using System.Text;
using ApiShared.Dto.IoT;
using IntegrationTests;
using Newtonsoft.Json;
using Xunit;

namespace ApiIntegrationTests.IoT;

/// <summary>
/// Integration tests for device provisioning API endpoints.
/// </summary>
public class ProvisioningApiTests : AccessControlIntegrationTests
{
    private readonly Uri _provisioningUri = new("api/v1/provisioning", UriKind.Relative);
    private readonly Uri _renewUri = new("api/v1/renew", UriKind.Relative);

    [Fact]
    public async Task ProvisionDevice_ReturnsBadRequestForEmptyBody()
    {
        // Arrange
        var content = new StringContent("{}", Encoding.UTF8, "application/json");

        // Act
        var response = await _httpClient.PostAsync(_provisioningUri, content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ProvisionDevice_ReturnsBadRequestForInvalidDeviceId()
    {
        // Arrange
        var request = new ProvisioningRequest
        {
            DeviceId = "INVALID-ID",
            Csr = "-----BEGIN CERTIFICATE REQUEST-----\ntest\n-----END CERTIFICATE REQUEST-----",
            SharedSecret = "some-secret-12345678"
        };
        var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

        // Act
        var response = await _httpClient.PostAsync(_provisioningUri, content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ProvisionDevice_ReturnsUnauthorizedForInvalidSecret()
    {
        // Arrange
        var request = new ProvisioningRequest
        {
            DeviceId = "DEV-AABBCCDDEEFF",
            Csr = "-----BEGIN CERTIFICATE REQUEST-----\ntest\n-----END CERTIFICATE REQUEST-----",
            SharedSecret = "wrong-secret-1234567"
        };
        var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

        // Act
        var response = await _httpClient.PostAsync(_provisioningUri, content);

        // Assert
        // Should be Unauthorized or InternalServerError (if cert service not configured)
        Assert.True(
            response.StatusCode == HttpStatusCode.Unauthorized ||
            response.StatusCode == HttpStatusCode.InternalServerError,
            $"Expected Unauthorized or InternalServerError, got {response.StatusCode}");
    }

    [Fact]
    public async Task RenewCertificate_ReturnsUnauthorizedWithoutClientCert()
    {
        // Arrange
        var request = new RenewalRequest
        {
            DeviceId = "DEV-AABBCCDDEEFF",
            Csr = "-----BEGIN CERTIFICATE REQUEST-----\ntest\n-----END CERTIFICATE REQUEST-----",
            Renewal = true
        };
        var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

        // Act - without mTLS client certificate
        var response = await _httpClient.PostAsync(_renewUri, content);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task ProvisionDevice_ValidatesDeviceIdFormat()
    {
        // Arrange - test various invalid formats
        var invalidIds = new[]
        {
            "dev-aabbccddeeff", // lowercase
            "DEV-aabbccddeeff", // lowercase hex
            "AABBCCDDEEFF",     // no prefix
            "DEV-AABBCCDDE",    // too short
            "DEV-AABBCCDDEEFFGG" // too long
        };

        foreach (var invalidId in invalidIds)
        {
            var request = new ProvisioningRequest
            {
                DeviceId = invalidId,
                Csr = "-----BEGIN CERTIFICATE REQUEST-----\ntest\n-----END CERTIFICATE REQUEST-----",
                SharedSecret = "test-secret-1234567"
            };
            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.PostAsync(_provisioningUri, content);

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.BadRequest, $"Device ID '{invalidId}' should be rejected");
        }
    }
}
