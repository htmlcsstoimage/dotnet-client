namespace HtmlCssToImage.Models.Recordings;

/// <summary>
/// Find and click an element, scrolling it into view when needed.
/// <see href="https://docs.htmlcsstoimage.com/parameters/actions/">Read the actions documentation for more information.</see>
/// </summary>
public sealed class RecordingClickAction : RecordingAction
{
    /// <summary>CSS selector for the element to click.</summary>
    public required string Selector { get; set; }

    /// <summary>Maximum wait for the element in milliseconds. Defaults to 0, which performs a single check. The recording deadline can end the wait earlier.</summary>
    public uint? TimeoutMs { get; set; }

    /// <summary>Actions to run when the click target is missing. Omitted, null, or empty continues. Use an error action to fail instead.</summary>
    public RecordingAction[]? IfNotFound { get; set; }

    /// <summary>Mouse button: left (default), right, middle, back, or forward.</summary>
    public RecordingMouseButton? Button { get; set; }

    /// <summary>Click count reported to the page, from 1 to 3. Defaults to 1. This sets the click event count; it does not repeat the action.</summary>
    public int? Count { get; set; }

    /// <summary>Time between mouse down and mouse up, from 0 to 5000 milliseconds. Defaults to 0.</summary>
    public int? DelayMs { get; set; }

    /// <summary>Horizontal offset in CSS pixels from the element's border-box top-left. If either offset is supplied, the other defaults to 0; otherwise the default clickable point is used.</summary>
    public double? OffsetX { get; set; }

    /// <summary>Vertical offset in CSS pixels from the element's border-box top-left. If either offset is supplied, the other defaults to 0; otherwise the default clickable point is used.</summary>
    public double? OffsetY { get; set; }
}
