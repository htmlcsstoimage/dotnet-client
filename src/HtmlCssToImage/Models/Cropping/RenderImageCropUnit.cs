namespace HtmlCssToImage.Models;

/// <summary>
/// Specifies the unit used by a crop position or size.
/// </summary>
public enum RenderImageCropUnit
{
    /// <summary>
    /// The value is measured in pixels.
    /// </summary>
    Pixels,

    /// <summary>
    /// The value is a percentage of the corresponding image dimension.
    /// </summary>
    Percent
}
