namespace HtmlCssToImage.Models.Recordings;

/// <summary>
/// Scroll to an edge of the document or selected container.
/// <see href="https://docs.htmlcsstoimage.com/parameters/actions/">Read the actions documentation for more information.</see>
/// </summary>
public sealed class PositionScrollTarget : ScrollTarget
{
    /// <summary>Edge to scroll to: top (default), bottom, left, or right.</summary>
    public ScrollPosition? Position { get; set; }
}
