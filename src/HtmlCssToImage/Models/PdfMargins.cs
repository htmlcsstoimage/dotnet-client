using System.Text.Json.Serialization;
using HtmlCssToImage.Models.Converters;

namespace HtmlCssToImage.Models;

/// Defines the margins for a PDF document, supporting specification
/// of top, right, bottom, and left margins. Each margin is defined
/// using a configurable unit system such as pixels, inches, percentage,
/// centimeters, millimeters, or points, enabling precise control over
/// document layout and formatting.
[JsonConverter(typeof(PdfMarginsConverter))]
public record struct PdfMargins(PdfValueWithUnits Top, PdfValueWithUnits Right, PdfValueWithUnits Bottom, PdfValueWithUnits Left) { };