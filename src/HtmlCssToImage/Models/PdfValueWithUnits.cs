using System.Text.Json.Serialization;
using HtmlCssToImage.Models.Converters;

namespace HtmlCssToImage.Models;

/// Represents a value and its associated unit specifically for use in PDF-related
/// operations, such as defining measurements with different units like pixels,
/// inches, percentage, centimeters, millimeters, or points.
[JsonConverter(typeof(PdfValueWithUnitsNullableConverter))]
public record struct PdfValueWithUnits(decimal Value, PdfUnit Unit)
{
    /// Defines an implicit conversion operator that allows a decimal value
    /// to be converted to a PdfValueWithUnits instance. When a decimal is
    /// converted using this operator, the resulting PdfValueWithUnits will
    /// use PdfUnit.PIXELS as the unit.
    public static implicit operator PdfValueWithUnits(decimal value) => new(value, PdfUnit.PIXELS);

    /// Creates a PdfValueWithUnits instance representing a value in inches.
    /// This method is specifically used to define measurements in the INCHES unit for PDF-related operations.
    /// <param name="value">The measurement value expressed in inches.</param>
    /// <return>A PdfValueWithUnits instance with the specified value and the unit set to PdfUnit.INCHES.</return>
    public static PdfValueWithUnits Inches(decimal value) => new(value, PdfUnit.INCHES);

}