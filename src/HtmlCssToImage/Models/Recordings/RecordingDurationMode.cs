using FancyEnumGenerator.Attributes;
using System.Text.Json.Serialization;

namespace HtmlCssToImage.Models.Recordings;

/// <summary>How the requested duration controls the recording timeline.</summary>
[FancyEnum(AllowNoUnknown = true, DefaultToStringBehavior = FancyEnumDefaultToStringBehavior.JsonStringEnumMemberNameAttribute)]
public enum RecordingDurationMode
{
    /// <summary>Pads or cuts off the recording at the requested duration.</summary>
    [JsonStringEnumMemberName("fixed")]
    Fixed,
    /// <summary>Adjusts playback speed to fit the completed timeline into the requested duration.</summary>
    [JsonStringEnumMemberName("target")]
    Target,
    /// <summary>Records for at least the requested duration, allowing actions to finish up to the 90-second capture limit.</summary>
    [JsonStringEnumMemberName("at_least")]
    AtLeast,
    /// <summary>Stops when actions finish or the requested duration is reached. This is the default.</summary>
    [JsonStringEnumMemberName("at_most")]
    AtMost,
}
