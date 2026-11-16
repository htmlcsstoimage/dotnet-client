using FancyEnumGenerator.Attributes;
using System.Text.Json.Serialization;

namespace HtmlCssToImage.Models.Recordings;

/// <summary>Mouse button used by a click action.</summary>
[FancyEnum(AllowNoUnknown = true, DefaultToStringBehavior = FancyEnumDefaultToStringBehavior.JsonStringEnumMemberNameAttribute)]
public enum RecordingMouseButton
{
    /// <summary>Left.</summary>
    [JsonStringEnumMemberName("left")]
    Left,
    /// <summary>Right.</summary>
    [JsonStringEnumMemberName("right")]
    Right,
    /// <summary>Middle.</summary>
    [JsonStringEnumMemberName("middle")]
    Middle,
    /// <summary>Back.</summary>
    [JsonStringEnumMemberName("back")]
    Back,
    /// <summary>Forward.</summary>
    [JsonStringEnumMemberName("forward")]
    Forward,
}
