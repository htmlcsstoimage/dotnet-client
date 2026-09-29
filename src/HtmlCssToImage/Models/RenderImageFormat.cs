using FancyEnumGenerator.Attributes;

namespace HtmlCssToImage.Models;

/// <summary>
/// Specifies the available image formats for rendering output.
/// </summary>
[FancyEnum(AllowNoUnknown = true, DefaultToStringBehavior = FancyEnumDefaultToStringBehavior.NameOfLower)]
public enum RenderImageFormat
{
    /// <summary>
    /// png format
    /// </summary>
    PNG,
    /// <summary>
    /// jpg format
    /// </summary>
    JPG,
    /// <summary>
    /// webp format
    /// </summary>
    WEBP,
    /// <summary>
    /// pdf format
    /// </summary>
    PDF
}
