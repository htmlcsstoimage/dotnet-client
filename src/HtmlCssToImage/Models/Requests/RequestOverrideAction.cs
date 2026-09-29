using FancyEnumGenerator.Attributes;
using System.Text.Json.Serialization;

namespace HtmlCssToImage.Models.Requests;

/// <summary>The action to take when a browser request matches an override rule.</summary>
[FancyEnum(AllowNoUnknown = true, DefaultToStringBehavior = FancyEnumDefaultToStringBehavior.NameOfLower)]
public enum RequestOverrideAction
{
    /// <summary>Block the matching request.</summary>
    [JsonStringEnumMemberName("block")]
    Block
}
