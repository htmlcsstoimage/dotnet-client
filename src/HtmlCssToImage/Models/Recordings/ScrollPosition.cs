using FancyEnumGenerator.Attributes;
using System.Text.Json.Serialization;

namespace HtmlCssToImage.Models.Recordings;

/// <summary>Edge of the document or container to scroll to.</summary>
[FancyEnum(AllowNoUnknown = true, DefaultToStringBehavior = FancyEnumDefaultToStringBehavior.JsonStringEnumMemberNameAttribute)]
public enum ScrollPosition
{
    /// <summary>Top.</summary>
    [JsonStringEnumMemberName("top")]
    Top,
    /// <summary>Bottom.</summary>
    [JsonStringEnumMemberName("bottom")]
    Bottom,
    /// <summary>Left.</summary>
    [JsonStringEnumMemberName("left")]
    Left,
    /// <summary>Right.</summary>
    [JsonStringEnumMemberName("right")]
    Right,
}
