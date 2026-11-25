namespace HtmlCssToImage.Models.Recordings;

/// <summary>Use an audio file from the recording organization's Asset Library, including a private upload.</summary>
public sealed class RecordingAssetAudio : RecordingAudio
{
    /// <summary>The asset ID returned by the completed upload.</summary>
    public required string AssetId { get; set; }
}
