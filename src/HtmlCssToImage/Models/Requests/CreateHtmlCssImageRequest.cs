using System.Text.Json.Serialization;
using HtmlCssToImage.Models.Converters;

namespace HtmlCssToImage.Models.Requests;

/// <summary>
/// Represents a request for generating an image from HTML and CSS content.
/// </summary>
/// <remarks>
/// This class is used to define the parameters required to create an image from HTML markup and optional CSS styles.
/// </remarks>
public class CreateHtmlCssImageRequest:CreateImageCommonOptions, IBatchAllowedImageRequest, ICreateImageRequestBase
{
    /// <summary>
    /// HTML to render and take a screenshot of. Your HTML will be rendered in a wrapper document unless you use a full document.
    /// </summary>
    public string Html { get; set; } = null!;

    /// <summary>
    /// CSS used to style your rendered image.
    /// </summary>
    public string? Css { get; set; }

    /// <summary>
    /// Gets or sets the Google Fonts to be included when generating an image.
    /// This property allows specifying one or more Google Fonts
    /// You must also set the font-family in your CSS to use the loaded font(s).
    /// </summary>
    [JsonConverter(typeof(GoogleFontsJsonConverter))]
    public string[]? GoogleFonts { get; set; }

}