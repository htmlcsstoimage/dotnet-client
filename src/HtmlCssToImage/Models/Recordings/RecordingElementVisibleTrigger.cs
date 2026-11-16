namespace HtmlCssToImage.Models.Recordings;

/// <summary>
/// Wait for a matching element to become visible before starting recording. Fails if the timeout expires.
/// <see href="https://docs.htmlcsstoimage.com/parameters/trigger/">Read the trigger documentation for more information.</see>
/// </summary>
public sealed class RecordingElementVisibleTrigger : RecordingTrigger
{
    /// <summary>CSS selector for the element that must become visible before recording starts.</summary>
    public required string Selector { get; set; }

    /// <summary>Maximum wait in milliseconds. Defaults to 1000; minimum 100. The maximum depends on your plan.</summary>
    public uint? TimeoutMs { get; set; }
}
