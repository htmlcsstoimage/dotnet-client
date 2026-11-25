namespace HtmlCssToImage.Models.Recordings;

/// <summary>Use a publicly accessible audio file as the soundtrack.</summary>
public sealed class RecordingUrlAudio : RecordingAudio
{
    /// <summary>Direct HTTP or HTTPS URL of the audio file, accessible at render time.</summary>
    public required string Url { get; set; }
}
