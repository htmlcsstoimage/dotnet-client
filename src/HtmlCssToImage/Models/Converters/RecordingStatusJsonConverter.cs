using System.Text.Json;
using System.Text.Json.Serialization;
using HtmlCssToImage.Models.Recordings;

namespace HtmlCssToImage.Models.Converters;

/// <summary>
/// A JsonConverter for <see cref="RecordingStatus"/> that reads an unrecognized status string, such as one added in a
/// newer API version, as <see cref="RecordingStatus.Unknown"/> instead of failing. Known values use their API names.
/// </summary>
public sealed class RecordingStatusJsonConverter : JsonConverter<RecordingStatus>
{
    /// <inheritdoc />
    public override RecordingStatus Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException($"Unexpected token {reader.TokenType} when parsing recording status");
        }

        return RecordingStatus.ParseOrUnknown(reader.GetString());
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, RecordingStatus value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToStringFancy());
    }
}
