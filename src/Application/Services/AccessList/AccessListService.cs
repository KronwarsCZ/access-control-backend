using System.Buffers.Binary;
using System.IO.Hashing;
using System.Text.RegularExpressions;
using ApiShared.Dto.IoT;
using Application.IRepositories.Devices;
using Application.Services.Devices;
using Microsoft.Extensions.Logging;

namespace Application.Services.AccessList;

/// <summary>
/// Implementation of access list service for generating allowed UID lists.
/// Supports both JSON and binary formats for ESP32 devices.
/// </summary>
public partial class AccessListService : IAccessListService
{
    // Binary format constants
    private const uint Magic = 0x414C4B59; // "ALKY"
    private const byte FormatVersion = 1;
    private const int HeaderSize = 20;
    private const int EntrySize = 20;

    private readonly IDeviceService _deviceService;
    private readonly IDeviceRepository _deviceRepository;
    private readonly ILogger<AccessListService> _logger;

    // Device ID format: DEV-XXXXXXXXXXXX (12 hex chars)
    [GeneratedRegex(@"^DEV-[A-F0-9]{12}$", RegexOptions.Compiled)]
    private static partial Regex DeviceIdPattern();

    public AccessListService(
        IDeviceService deviceService, 
        IDeviceRepository deviceRepository,
        ILogger<AccessListService> logger)
    {
        _deviceService = deviceService;
        _deviceRepository = deviceRepository;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<AllowedUidEntry>> GetAllowedUidsAsync(string deviceId, CancellationToken cancellationToken)
    {
        if (!ValidateDeviceId(deviceId))
        {
            _logger.LogWarning("Invalid device ID format: {DeviceId}", deviceId);
            return [];
        }

        var deviceGuid = await FindDeviceGuidByDeviceIdAsync(deviceId, cancellationToken);
        if (deviceGuid == null)
        {
            _logger.LogWarning("Device not found: {DeviceId}", deviceId);
            return [];
        }

        // Get all keys that can open this device
        var openingKeys = await _deviceService.GetOpeningKeys(deviceGuid.Value, cancellationToken);
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        var entries = openingKeys
            .Where(k => !k.IsDeleted)
            .Select(k => new AllowedUidEntry
            {
                Uid = SanitizeUid(k.KeyUid),
                ValidFrom = k.ValidityStartUtc != default 
                    ? new DateTimeOffset(k.ValidityStartUtc, TimeSpan.Zero).ToUnixTimeSeconds() 
                    : 0,
                ValidTo = k.ValidityEndUtc.HasValue 
                    ? new DateTimeOffset(k.ValidityEndUtc.Value, TimeSpan.Zero).ToUnixTimeSeconds() 
                    : 0
            })
            .Where(e => !string.IsNullOrEmpty(e.Uid))
            .ToList();

        _logger.LogInformation("Found {Count} allowed UIDs for device {DeviceId}", entries.Count, deviceId);
        return entries;
    }

    /// <inheritdoc />
    public async Task<byte[]> GenerateBinaryAllowlistAsync(string deviceId, CancellationToken cancellationToken)
    {
        var entries = (await GetAllowedUidsAsync(deviceId, cancellationToken)).ToList();
        var timestamp = (uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        var totalSize = HeaderSize + (entries.Count * EntrySize);
        var buffer = new byte[totalSize];

        var entryData = new byte[entries.Count * EntrySize];
        for (var i = 0; i < entries.Count; i++)
        {
            WriteEntry(entryData.AsSpan(i * EntrySize, EntrySize), entries[i]);
        }

        var checksum = CalculateCrc32(entryData);

        // Write header using direct array access to avoid Span in async method
        BinaryPrimitives.WriteUInt32LittleEndian(buffer.AsSpan(0, 4), Magic);
        buffer[4] = FormatVersion;
        buffer[5] = 0; // reserved
        buffer[6] = 0; // reserved
        buffer[7] = 0; // reserved
        BinaryPrimitives.WriteUInt32LittleEndian(buffer.AsSpan(8, 4), timestamp);
        BinaryPrimitives.WriteUInt32LittleEndian(buffer.AsSpan(12, 4), (uint)entries.Count);
        BinaryPrimitives.WriteUInt32LittleEndian(buffer.AsSpan(16, 4), checksum);

        Array.Copy(entryData, 0, buffer, HeaderSize, entryData.Length);

        _logger.LogInformation("Generated binary allowlist: {Size} bytes, {Count} entries for device {DeviceId}",
            buffer.Length, entries.Count, deviceId);

        return buffer;
    }

    /// <inheritdoc />
    public async Task<AllowedUidsResponse> GetAllowedUidsResponseAsync(string deviceId, CancellationToken cancellationToken)
    {
        var entries = await GetAllowedUidsAsync(deviceId, cancellationToken);
        
        return new AllowedUidsResponse
        {
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            Entries = entries
        };
    }

    /// <inheritdoc />
    public bool ValidateDeviceId(string deviceId)
    {
        if (string.IsNullOrWhiteSpace(deviceId))
        {
            return false;
        }

        return DeviceIdPattern().IsMatch(deviceId);
    }

    /// <inheritdoc />
    public async Task<Guid?> FindDeviceGuidByDeviceIdAsync(string deviceId, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogDebug("Looking up device by hardware ID: {DeviceId}", deviceId);
            
            // Look up device by HardwareId field
            var device = await _deviceRepository.GetByHardwareIdAsync(deviceId, cancellationToken);
            
            if (device != null)
            {
                _logger.LogDebug("Found device {DeviceGuid} for hardware ID {DeviceId}", device.Id, deviceId);
                return device.Id;
            }

            _logger.LogDebug("No device found for hardware ID: {DeviceId}", deviceId);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error looking up device: {DeviceId}", deviceId);
            return null;
        }
    }

    private static void WriteEntry(Span<byte> buffer, AllowedUidEntry entry)
    {
        // Entry format (20 bytes):
        // Offset 0-7: UID bytes (8 bytes, zero-padded)
        // Offset 8: UID length (1 byte)
        // Offset 9-11: reserved (3 bytes)
        // Offset 12-15: valid_from (4 bytes)
        // Offset 16-19: valid_to (4 bytes)

        var uidBytes = HexStringToBytes(entry.Uid);
        var uidLen = Math.Min(uidBytes.Length, 8);

        // Clear buffer first
        buffer.Clear();

        // Write UID (up to 8 bytes)
        uidBytes.AsSpan(0, uidLen).CopyTo(buffer[0..8]);
        
        // Write UID length
        buffer[8] = (byte)uidLen;

        // Reserved bytes (already zeroed)

        // Write validity timestamps
        BinaryPrimitives.WriteUInt32LittleEndian(buffer[12..16], (uint)entry.ValidFrom);
        BinaryPrimitives.WriteUInt32LittleEndian(buffer[16..20], (uint)entry.ValidTo);
    }

    private static uint CalculateCrc32(byte[] data)
    {
        // Use standard CRC32 (polynomial 0xEDB88320)
        var crc = new Crc32();
        crc.Append(data);
        var hash = crc.GetCurrentHash();
        return BinaryPrimitives.ReadUInt32LittleEndian(hash);
    }

    private static byte[] HexStringToBytes(string hex)
    {
        if (string.IsNullOrEmpty(hex))
        {
            return [];
        }

        // Remove any spaces or dashes
        hex = hex.Replace(" ", "").Replace("-", "").Replace(":", "");

        // Ensure even number of characters
        if (hex.Length % 2 != 0)
        {
            hex = "0" + hex;
        }

        var bytes = new byte[hex.Length / 2];
        for (var i = 0; i < bytes.Length; i++)
        {
            bytes[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
        }
        return bytes;
    }

    private static string SanitizeUid(string? uid)
    {
        if (string.IsNullOrEmpty(uid))
            return string.Empty;

        var hexOnly = new string(uid.Where(Uri.IsHexDigit).ToArray()).ToUpperInvariant();
        
        if (hexOnly.Length > 16)
            hexOnly = hexOnly[..16];

        return hexOnly;
    }
}
