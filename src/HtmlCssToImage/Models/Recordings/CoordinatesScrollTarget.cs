namespace HtmlCssToImage.Models.Recordings;

/// <summary>
/// Scroll to absolute coordinates within the document or selected container.
/// <see href="https://docs.htmlcsstoimage.com/parameters/actions/">Read the actions documentation for more information.</see>
/// </summary>
public sealed class CoordinatesScrollTarget : ScrollTarget
{
    /// <summary>Absolute horizontal scroll position in CSS pixels. Defaults to 0.</summary>
    public double? X { get; set; }

    /// <summary>Absolute vertical scroll position in CSS pixels. Defaults to 0.</summary>
    public double? Y { get; set; }
}
