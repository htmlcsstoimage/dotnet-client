using System.Text.Json;
using System.Text.Json.Serialization;

namespace HtmlCssToImage.Models.Converters;

internal class PdfMarginsConverter:JsonConverter<PdfMargins?>
{
    public override PdfMargins? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        if (reader.TokenType != JsonTokenType.StartArray)
        {
            throw new JsonException($"Unexpected token {reader.TokenType} when parsing pdf margins");
        }

        var arr = JsonSerializer.Deserialize<PdfValueWithUnits[]>(ref reader, JsonContext.Default.PdfValueWithUnitsArray);
        if (arr is not { Length: 4 })
        {
            return null;
        }
        return new PdfMargins(arr[0], arr[1], arr[2], arr[3]);
    }

    public override void Write(Utf8JsonWriter writer, PdfMargins? value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
            return;
        }
        writer.WriteStartArray();
        JsonSerializer.Serialize(writer, value.Value.Top, JsonContext.Default.PdfValueWithUnits);
        JsonSerializer.Serialize(writer, value.Value.Right, JsonContext.Default.PdfValueWithUnits);
        JsonSerializer.Serialize(writer, value.Value.Bottom, JsonContext.Default.PdfValueWithUnits);
        JsonSerializer.Serialize(writer, value.Value.Left, JsonContext.Default.PdfValueWithUnits);
        writer.WriteEndArray();
    }
}