namespace HtmlCssToImage.Models.Recordings;

/// <summary>Output format, frame rate, and scaling for a recording.</summary>
public class RecordingOutputOptions
{
    /// <summary>
    /// The media format to produce. Defaults to <see cref="RecordingFormat.Mp4"/>.
    /// <see href="https://docs.htmlcsstoimage.com/parameters/output/">Read the output documentation for more information.</see>
    /// </summary>
    public RecordingFormat? Format { get; set; }

    /// <summary>
    /// Frames per second. Defaults to 15 for GIF and 30 for MP4/WebM; GIF supports 10 to 50, and video supports 10 to 120.
    /// </summary>
    public int? Fps { get; set; }

    /// <summary>
    /// Scales the captured pixel dimensions without changing the viewport or page layout. Defaults to 1; 0.5 halves both dimensions.
    /// </summary>
    public double? Scale { get; set; }
}
