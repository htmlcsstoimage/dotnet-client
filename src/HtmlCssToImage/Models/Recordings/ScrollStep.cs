namespace HtmlCssToImage.Models.Recordings;

/// <summary>
/// One timed scroll movement within a scroll action. Steps run in order.
/// <see href="https://docs.htmlcsstoimage.com/parameters/actions/">Read the actions documentation for more information.</see>
/// </summary>
public class ScrollStep
{
    /// <summary>Destination within the document or the parent action's selected scroll container.</summary>
    public required ScrollTarget To { get; set; }

    /// <summary>Required. Duration of the movement in milliseconds. Must be at least 1. The recording deadline can cut the movement short.</summary>
    public required int DurationMs { get; set; }

    /// <summary>Easing curve for the scroll movement. Defaults to linear (constant speed).</summary>
    public ScrollEasing? Easing { get; set; }
}
