namespace HtmlCssToImage.Models.Recordings;

/// <summary>
/// Scroll to an element matching a CSS selector, with optional offsets.
/// <see href="https://docs.htmlcsstoimage.com/parameters/actions/">Read the actions documentation for more information.</see>
/// </summary>
public sealed class SelectorScrollTarget : ScrollTarget
{
    /// <summary>CSS selector for the destination element.</summary>
    public required string Selector { get; set; }

    /// <summary>Horizontal offset in CSS pixels added to the destination element's position. Defaults to 0.</summary>
    public double? OffsetX { get; set; }

    /// <summary>Vertical offset in CSS pixels added to the destination element's position. Defaults to 0.</summary>
    public double? OffsetY { get; set; }

    /// <summary>Actions to run when the destination is missing. Omitted, null, or empty skips this movement. If the branch completes, continue with the next step. Use an error action to fail instead.</summary>
    public RecordingAction[]? IfNotFound { get; set; }
}
