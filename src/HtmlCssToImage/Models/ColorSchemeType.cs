using FancyEnumGenerator.Attributes;

namespace HtmlCssToImage.Models;

/// <summary>
/// Represents the color scheme types available, setting Chrome to render as if the user prefers light or dark mode.
/// </summary>
[FancyEnum(AllowNoUnknown = true)]
public enum ColorSchemeType
{
    /// <summary>
    /// light mode
    /// </summary>
    light,
    /// <summary>
    /// dark mode
    /// </summary>
    dark
}
