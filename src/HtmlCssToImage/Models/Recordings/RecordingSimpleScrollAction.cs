using System.Text.Json.Nodes;

namespace HtmlCssToImage.Models.Recordings;

/// <summary>
/// Scroll down repeatedly until reaching the bottom or the recording deadline. Each movement has its own duration and easing, followed by a pause.
/// <see href="https://docs.htmlcsstoimage.com/parameters/actions/">Read the actions documentation for more information.</see>
/// </summary>
public sealed class RecordingSimpleScrollAction : RecordingAction
{
    /// <summary>
    /// Duration of each scroll movement in milliseconds. Defaults to 800.
    /// </summary>
    public int? ScrollDurationMs { get; set; }

    /// <summary>
    /// Pause between completed scroll movements in milliseconds. Defaults to 400; set to 0 for continuous scrolling.
    /// </summary>
    public int? MsBetweenScrolls { get; set; }

    /// <summary>
    /// Distance per scroll, as a number of CSS pixels or a string such as <c>900px</c> or <c>50vh</c>. Assign a number or string directly. Leave null to use API defaults.
    /// </summary>
    public JsonNode? ScrollDistance { get; set; }

    /// <summary>
    /// Easing curve for each movement. Leave null to use API defaults.
    /// </summary>
    public ScrollEasing? Easing { get; set; }

    /// <summary>
    /// Time to hold at the bottom in milliseconds, limited by the remaining recording time. Defaults to 0.
    /// </summary>
    public int? StallAtEndMs { get; set; }
}
