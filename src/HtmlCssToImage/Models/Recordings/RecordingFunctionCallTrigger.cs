namespace HtmlCssToImage.Models.Recordings;

/// <summary>
/// Wait for the page to call window.RecordingReady() before starting recording. Fails if the timeout expires.
/// <see href="https://docs.htmlcsstoimage.com/parameters/trigger/">Read the trigger documentation for more information.</see>
/// </summary>
public sealed class RecordingFunctionCallTrigger : RecordingTrigger
{
    /// <summary>Maximum wait for window.RecordingReady(), in milliseconds. Defaults to 1000; minimum 100. The maximum depends on your plan.</summary>
    public uint? TimeoutMs { get; set; }
}
