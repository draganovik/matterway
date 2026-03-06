using System.ComponentModel;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Matterway.ServiceDefaults.Identifiers;

[JsonConverter(typeof(ArticleCodeJsonConverter))]
[TypeConverter(typeof(ArticleCodeTypeConverter))]
public readonly record struct ArticleCode : ISpanParsable<ArticleCode>
{
    public const int Length = 8;

    public string Value { get; }

    private ArticleCode(string value)
    {
        Value = value;
    }

    public static ArticleCode Parse(string s, IFormatProvider? provider)
    {
        if (TryParse(s, provider, out var result))
            return result;

        throw new FormatException(
            $"Article code '{s}' is invalid. Use exactly {Length} letters or numbers.");
    }

    public static ArticleCode Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
    {
        return Parse(s.ToString(), provider);
    }

    public static bool TryParse(string? s, IFormatProvider? provider, out ArticleCode result)
    {
        if (TryNormalize(s, out var normalized))
        {
            result = new ArticleCode(normalized);
            return true;
        }

        result = default;
        return false;
    }

    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out ArticleCode result)
    {
        return TryParse(s.ToString(), provider, out result);
    }

    public static ArticleCode FromStorage(string value)
    {
        return Parse(value, CultureInfo.InvariantCulture);
    }

    public override string ToString()
    {
        return Value ?? string.Empty;
    }

    public static implicit operator string(ArticleCode value)
    {
        return value.ToString();
    }

    public static explicit operator ArticleCode(string value)
    {
        return Parse(value, CultureInfo.InvariantCulture);
    }

    private static bool TryNormalize(string? value, out string normalized)
    {
        normalized = string.Empty;

        if (string.IsNullOrWhiteSpace(value))
            return false;

        var trimmed = value.Trim().ToUpperInvariant();
        if (trimmed.Length != Length)
            return false;

        foreach (var character in trimmed)
            if (!char.IsAsciiLetterOrDigit(character))
                return false;

        normalized = trimmed;
        return true;
    }

    public sealed class ArticleCodeJsonConverter : JsonConverter<ArticleCode>
    {
        public override ArticleCode Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.String)
                throw new JsonException("Article code must be a string.");

            var raw = reader.GetString();
            if (TryParse(raw, CultureInfo.InvariantCulture, out var result))
                return result;

            throw new JsonException(
                $"Article code '{raw}' is invalid. Use exactly {Length} letters or numbers.");
        }

        public override void Write(Utf8JsonWriter writer, ArticleCode value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }

    public sealed class ArticleCodeTypeConverter : TypeConverter
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