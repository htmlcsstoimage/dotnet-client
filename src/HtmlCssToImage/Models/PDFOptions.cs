using System.Text.Json.Serialization;
using HtmlCssToImage.Models.Converters;

namespace HtmlCssToImage.Models;

/// <summary>
/// Represents the configuration options for generating a PDF.
/// </summary>
/// <remarks>
/// Provides settings such as margins, page size, scaling, and whether background
/// graphics should be included in the PDF rendering.
/// </remarks>
public class PDFOptions
{
    /// <summary>
    /// Gets or sets the margins to be applied to the PDF output.
    /// </summary>
    /// <remarks>
    /// This property specifies the top, right, bottom, and left margins
    /// for the generated PDF. Margins are defined using the <c>PdfMargins</c> struct,
    /// which allows for specifying values with units such as pixels or percentages.
    /// </remarks>
    [JsonConverter(typeof(PdfMarginsConverter))]
    public PdfMargins? Margins { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the background graphics should be printed in the PDF output.
    /// </summary>
    /// <remarks>
    /// This property determines if background images and colors, specified in the source HTML and CSS,
    /// are rendered as part of the generated PDF document. A value of <c>true</c> retains the background,
    /// while <c>false</c> excludes it. If set to <c>null</c>, the default behavior of the PDF renderer is applied.
    /// </remarks>
    public bool? PrintBackground { get; set; }

    /// <summary>
    /// Gets or sets the height of the PDF page.
    /// </summary>
    /// <remarks>
    /// This property specifies the height of the generated PDF page and
    /// uses the <c>PdfValueWithUnits</c> struct to define the value,
    /// allowing for specific units such as pixels, millimeters, or inches.
    /// Setting this property to null will use the default height for the page.
    /// </remarks>
    [JsonConverter(typeof(PdfValueWithUnitsNullableConverter))]
    public PdfValueWithUnits? PageHeight { get; set; }

    /// <summary>
    /// Gets or sets the width of the PDF page.
    /// </summary>
    /// <remarks>
    /// This property specifies the width of the PDF page by using the <c>PdfValueWithUnits</c> struct.
    /// The value can include a specified unit of measurement, such as pixels or inches,
    /// allowing for precise control over the dimensions of the generated PDF.
    /// </remarks>
    [JsonConverter(typeof(PdfValueWithUnitsNullableConverter))]
    public PdfValueWithUnits? PageWidth { get; set; }

    /// <summary>
    /// Gets or sets the scale factor to be applied when generating the PDF output.
    /// </summary>
    /// <remarks>
    /// This property defines the scaling factor for the content in the generated PDF.
    /// It allows customization of the output size by adjusting the rendering scale
    /// relative to the original dimensions. A value greater than 1 enlarges the content,
    /// whereas a value less than 1 reduces it.
    /// </remarks>
    public decimal? Scale { get; set; }
}