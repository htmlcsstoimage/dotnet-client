using FancyEnumGenerator.Attributes;
using System.Text.Json.Serialization;

namespace HtmlCssToImage.Models.Recordings;

/// <summary>How playback continues when the soundtrack ends before the video.</summary>
[FancyEnum(AllowNoUnknown = true, DefaultToStringBehavior = FancyEnumDefaultToStringBehavior.JsonStringEnumMemberNameAttribute)]
public enum RecordingAudioEndBehavior
{
    /// <summary>Play the remainder of the video silently (default).</summary>
    [JsonStringEnumMemberName("silence")]
    Silence,
    /// <summary>Repeat the track from the selected source offset.</summary>
    [JsonStringEnumMemberName("loop")]
    Loop,
    /// <summary>End the video with the track.</summary>
    [JsonStringEnumMemberName("trim_video")]
    TrimVideo,
}
