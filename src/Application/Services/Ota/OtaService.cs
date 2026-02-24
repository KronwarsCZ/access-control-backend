using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Application.Services.Ota;

/// <summary>
/// Implementation of OTA service for firmware version and binary management.
/// </summary>
public class OtaService : IOtaService
{
    private readonly OtaConfiguration _configuration;
    private readonly ILogger<OtaService> _logger;
    private readonly string _basePath;

    public OtaService(IOptions<OtaConfiguration> configuration, ILogger<OtaService> logger)
    {
        _configuration = configuration.Value;
        _logger = logger;
        _basePath = AppDomain.CurrentDomain.BaseDirectory;
    }

    /// <inheritdoc />
    public string GetFirmwareVersion()
    {
        var version = _configuration.FirmwareVersion;
        
        // Validate version format: only allow digits and dots, max length 31
        if (string.IsNullOrWhiteSpace(version) || version.Length > 31 || !version.All(c => char.IsDigit(c) || c == '.'))
        {
            _logger.LogWarning("Invalid firmware version configured: {Version}", version);
            return "0.0.0";
        }

        return version;
    }

    /// <inheritdoc />
    public async Task<Stream?> GetFirmwareStreamAsync(CancellationToken cancellationToken)
    {
        var filePath = GetFirmwareFilePath();
        if (filePath == null)
        {
            return null;
        }

        try
        {
            var fileInfo = new FileInfo(filePath);
            
            if (fileInfo.Length > _configuration.MaxFirmwareSizeBytes)
            {
                _logger.LogError("Firmware file exceeds maximum allowed size: {Size} bytes", fileInfo.Length);
                return null;
            }

            // Return file stream for streaming response
            return new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 
                bufferSize: 81920, useAsync: true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error opening firmware file: {Path}", filePath);
            return null;
        }
    }

    /// <inheritdoc />
    public string? GetFirmwareFilePath()
    {
        var configuredPath = _configuration.FirmwarePath;
        
        if (string.IsNullOrWhiteSpace(configuredPath))
        {
            _logger.LogWarning("Firmware path not configured");
            return null;
        }

        // Handle relative and absolute paths
        var fullPath = Path.IsPathRooted(configuredPath)
            ? configuredPath
            : Path.Combine(_basePath, configuredPath);

        // Security: ensure path doesn't escape base directory (path traversal prevention)
        var normalizedPath = Path.GetFullPath(fullPath);
        if (!Path.IsPathRooted(configuredPath) && !normalizedPath.StartsWith(_basePath, StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogError("Attempted path traversal detected: {Path}", configuredPath);
            return null;
        }

        if (!File.Exists(normalizedPath))
        {
            _logger.LogWarning("Firmware file not found: {Path}", normalizedPath);
            return null;
        }

        return normalizedPath;
    }

    /// <inheritdoc />
    public bool IsFirmwareAvailable()
    {
        var path = GetFirmwareFilePath();
        return path != null && File.Exists(path);
    }
}
