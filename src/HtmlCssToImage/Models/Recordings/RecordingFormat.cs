using FancyEnumGenerator.Attributes;
using System.Text.Json.Serialization;

namespace HtmlCssToImage.Models.Recordings;

/// <summary>Output media format. Recordings do not include audio.</summary>
[FancyEnum(AllowNoUnknown = true, DefaultToStringBehavior = FancyEnumDefaultToStringBehavior.JsonStringEnumMemberNameAttribute)]
public enum RecordingFormat
{
    /// <summary>Mp4.</summary>
    [JsonStringEnumMemberName("mp4")]
    Mp4,
    /// <summary>Webm.</summary>
    [JsonStringEnumMemberName("webm")]
    Webm,
    /// <summary>Gif.</summary>
    [JsonStringEnumMemberName("gif")]
    Gif,
}
