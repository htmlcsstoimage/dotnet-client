using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using HtmlCssToImage.Helpers;

namespace HtmlCssToImage.Models.Converters;

internal class PdfValueWithUnitsConverter:JsonConverter<PdfValueWithUnits>
{
    public override PdfValueWithUnits Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {

        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException($"Unexpected token {reader.TokenType} when parsing pdf value with units");
        }
        var val_bytes = reader.ValueSpan;
        if (val_bytes.Length == 0)
        {
            throw new JsonException($"Empty pdf value with units");
        }

        PdfUnit unit;
        var unit_len = 2;
        if (val_bytes[^1] == '%')
        {
            unit = PdfUnit.PERCENTAGE;
            unit_len = 1;
        }
        else
        {
            if(val_bytes[^2]=='p' && val_bytes[^1]=='x')
            {
                unit = PdfUnit.PIXELS;
            }else if (val_bytes[^2] == 'c' && val_bytes[^1] == 'm')
            {
                unit = PdfUnit.CENTIMETERS;
            }else if (val_bytes[^2] == 'i' && val_bytes[^1] == 'n')
            {
                unit = PdfUnit.INCHES;
            }else if (val_bytes[^2] == 'm' && val_bytes[^1] == 'm')
            {
                unit = PdfUnit.MILLIMETERS;
            }else if (val_bytes[^2] == 'p' && val_bytes[^1] == 't')
            {
                unit = PdfUnit.POINTS;
            }
            else
            {
                throw new JsonException($"Unexpected unit {val_bytes[^2]}{val_bytes[^1]}");
            }
        }
        var number = decimal.TryParse(val_bytes[..^unit_len], NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed_value) ? parsed_value : throw new JsonException($"Could not parse pdf value {reader.GetString()}");
        return new PdfValueWithUnits(number, unit);
    }

    public override void Write(Utf8JsonWriter writer, PdfValueWithUnits value, JsonSerializerOptions options)
    {
        // 32 chars for value
        // 2 chars for unit
        Span<char> span = stackalloc char[34];
        if (!value.Value.TryFormat(span, out int charsWritten, "0.####", CultureInfo.InvariantCulture))
        {
            throw new InvalidOperationException($"Could not format value {value.Value} into pdf value");
        }

        if (charsWritten> 32)
        {
            charsWritten = 32;
        }

        if (span[charsWritten] == '.')
        {
            charsWritten = charsWritten - 1;
        }

        var unit_str = value.Unit.PdfUnitCssValue();
        unit_str.CopyTo(span[charsWritten..]);
        charsWritten += unit_str.Length;
        writer.WriteStringValue(span[..charsWritten]);
    }
}