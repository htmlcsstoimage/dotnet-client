namespace HtmlCssToImage.Models.Recordings;

/// <summary>
/// Wait while recording and page activity continue.
/// <see href="https://docs.htmlcsstoimage.com/parameters/actions/">Read the actions documentation for more information.</see>
/// </summary>
public sealed class RecordingStallAction : RecordingAction
{
    /// <summary>Wait duration in milliseconds. Must be nonnegative. The recording deadline can end the wait early.</summary>
    public int? DurationMs { get; set; }
}
