namespace HtmlCssToImage.Models;

/// <summary>
/// Specifies where a size-only crop span is positioned within an axis.
/// </summary>
public enum RenderImageCropOrigin
{
    /// <summary>
    /// Positions the span at the beginning of the axis.
    /// </summary>
    Start,

    /// <summary>
    /// Centers the span within the axis.
    /// </summary>
    Center,

    /// <summary>
    /// Positions the span at the end of the axis.
    /// </summary>
    End
}
