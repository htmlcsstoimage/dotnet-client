using FancyEnumGenerator.Attributes;
using System.Text.Json.Serialization;

namespace HtmlCssToImage.Models;


/// <summary>
/// Represents the units of measurement available for PDF dimensions.
/// </summary>
[FancyEnum(AllowNoUnknown = true, DefaultToStringBehavior = FancyEnumDefaultToStringBehavior.JsonStringEnumMemberNameAttribute)]
public enum PdfUnit
{

    /// <summary>
    /// px
    /// </summary>
    [JsonStringEnumMemberName("px")]
    PIXELS,

    /// <summary>
    /// in
    /// </summary>
    [JsonStringEnumMemberName("in")]
    INCHES,


    /// <summary>
    /// %
    /// </summary>
    [JsonStringEnumMemberName("%")]
    PERCENTAGE,


    /// <summary>
    /// cm
    /// </summary>
    [JsonStringEnumMemberName("cm")]
    CENTIMETERS,


    /// <summary>
    /// mm
    /// </summary>
    [JsonStringEnumMemberName("mm")]
    MILLIMETERS,

    /// <summary>
    /// pt
    /// </summary>
    [JsonStringEnumMemberName("pt")]
    POINTS,
}
