using FancyEnumGenerator.Attributes;
using System.Text.Json.Serialization;

namespace HtmlCssToImage.Models;

/// <summary>
/// Specifies the unit used by a crop position or size.
/// </summary>
[FancyEnum(AllowNoUnknown = true, DefaultToStringBehavior = FancyEnumDefaultToStringBehavior.JsonStringEnumMemberNameAttribute)]
public enum RenderImageCropUnit
{
    /// <summary>
    /// The value is measured in pixels.
    /// </summary>
    [JsonStringEnumMemberName("px")]
    Pixels,

    /// <summary>
    /// The value is a percentage of the corresponding image dimension.
    /// </summary>
    [JsonStringEnumMemberName("%")]
    Percent
}
