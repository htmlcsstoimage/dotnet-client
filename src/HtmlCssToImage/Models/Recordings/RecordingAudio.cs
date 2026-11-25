using System.Text.Json.Serialization;

namespace HtmlCssToImage.Models.Recordings;

/// <summary>
/// A single soundtrack for an MP4 or WebM recording. Choose <see cref="RecordingAssetAudio"/> or <see cref="RecordingUrlAudio"/>.
/// <see href="https://docs.htmlcsstoimage.com/parameters/audio/">Audio parameter reference.</see>
/// </summary>
[JsonDerivedType(typeof(RecordingAssetAudio))]
[JsonDerivedType(typeof(RecordingUrlAudio))]
public abstract class RecordingAudio
{
    private protected RecordingAudio() { }

    /// <summary>Offset into the source file in milliseconds. Defaults to 0.</summary>
    public uint? SourceStartMs { get; set; }

    /// <summary>Delay from the start of the finished video in milliseconds. Defaults to 0.</summary>
    public uint? DelayMs { get; set; }

    /// <summary>Behavior when the source ends. Defaults to <see cref="RecordingAudioEndBehavior.Silence"/>.</summary>
    public RecordingAudioEndBehavior? WhenAudioEnds { get; set; }

    /// <summary>Defaults to true. Set to false to return the full silent video if the track cannot play at render time.</summary>
    public bool? ErrorOnSourceInvalid { get; set; }
}
