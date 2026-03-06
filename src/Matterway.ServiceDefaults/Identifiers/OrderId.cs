using System.Buffers.Binary;
using System.ComponentModel;
using System.Globalization;
using System.Numerics;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Matterway.ServiceDefaults.Identifiers;

[JsonConverter(typeof(OrderIdJsonConverter))]
[TypeConverter(typeof(OrderIdTypeConverter))]
public readonly record struct OrderId : IParsable<OrderId>, ISpanParsable<OrderId>
{
    private const int LegacyDigitsPerGroup = 4;
    private const int LegacyGroupCount = 4;
    private const int LegacyTotalDigits = LegacyDigitsPerGroup * LegacyGroupCount;
    private const string EncodedTimestampFormat = "yyyyMMddHHmmssfff";

    public const int Length = 19;
    public const int MaxLength = Length;

    public string Value { get; }

    private OrderId(string value)
    {
        Value = value;
    }

    public static OrderId New(DateTime? utcNow = null)
    {
        var timestamp = (utcNow ?? DateTime.UtcNow).ToUniversalTime();
        var encodedTimestamp = timestamp.ToString(EncodedTimestampFormat, CultureInfo.InvariantCulture);

        Span<byte> randomBytes = stackalloc byte[2];
        RandomNumberGenerator.Fill(randomBytes);
        var randomSuffix = BinaryPrimitives.ReadUInt16BigEndian(randomBytes)
            .ToString("D5", CultureInfo.InvariantCulture);

        var encodedValue = BigInteger.Parse(
                encodedTimestamp + randomSuffix,
                CultureInfo.InvariantCulture)
            .ToString("x", CultureInfo.InvariantCulture)
            .PadLeft(Length, '0');

        return Parse(encodedValue, CultureInfo.InvariantCulture);
    }

    public static OrderId Parse(string s, IFormatProvider? provider)
    {
        if (TryParse(s, provider, out var result))
            return result;

        throw new FormatException(
            $"Order id '{s}' is invalid. Use {Length} hexadecimal characters or the legacy 0000-0000-0000-0000 format.");
    }

    public static OrderId Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
    {
        return Parse(s.ToString(), provider);
    }

    public static bool TryParse(string? s, IFormatProvider? provider, out OrderId result)
    {
        if (TryNormalize(s, out var normalized))
        {
            result = new OrderId(normalized);
            return true;
        }

        result = default;
        return false;
    }

    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out OrderId result)
    {
        return TryParse(s.ToString(), provider, out result);
    }

    public static OrderId FromStorage(string value)
    {
        return Parse(value, CultureInfo.InvariantCulture);
    }

    public override string ToString()
    {
        return Value ?? string.Empty;
    }

    public static implicit operator string(OrderId value)
    {
        return value.ToString();
    }

    public static explicit operator OrderId(string value)
    {
        return Parse(value, CultureInfo.InvariantCulture);
    }

    private static bool TryNormalize(string? value, out string normalized)
    {
        normalized = string.Empty;

        if (string.IsNullOrWhiteSpace(value))
            return false;

        var trimmed = value.Trim();
        return TryNormalizeLegacy(trimmed, out normalized) || TryNormalizeHex(trimmed, out normalized);
    }

    private static bool TryNormalizeLegacy(string value, out string normalized)
    {
        normalized = string.Empty;

        Span<char> digits = stackalloc char[LegacyTotalDigits];
        var digitCount = 0;

        foreach (var character in value)
        {
            if (character == '-')
                continue;

            if (!char.IsAsciiDigit(character) || digitCount >= LegacyTotalDigits)
                return false;

            digits[digitCount++] = character;
        }

        if (digitCount != LegacyTotalDigits)
            return false;

        normalized =
            $"{new string(digits[..4])}-{new string(digits[4..8])}-{new string(digits[8..12])}-{new string(digits[12..16])}";
        return true;
    }

    private static bool TryNormalizeHex(string value, out string normalized)
    {
        normalized = string.Empty;

        if (value.Length != Length)
            return false;

        var lowered = value.ToLowerInvariant();
        foreach (var character in lowered)
            if (!char.IsAsciiHexDigit(character))
                return false;

        normalized = lowered;
        return true;
    }

    public sealed class OrderIdJsonConverter : JsonConverter<OrderId>
    {
        public override OrderId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.String)
                throw new JsonException("Order id must be a string.");

            var raw = reader.GetString();
            if (TryParse(raw, CultureInfo.InvariantCulture, out var result))
                return result;

            throw new JsonException(
                $"Order id '{raw}' is invalid. Use {Length} hexadecimal characters or the legacy 0000-0000-0000-0000 format.");
        }

        public override void Write(Utf8JsonWriter writer, OrderId value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }

    public sealed class OrderIdTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        {
            if (value is string text)
                return Parse(text, culture);

            return base.ConvertFrom(context, culture, value);
        }
    }
}