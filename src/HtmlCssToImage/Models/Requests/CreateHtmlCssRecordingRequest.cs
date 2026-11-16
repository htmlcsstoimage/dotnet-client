using System.Text.Json.Serialization;
using HtmlCssToImage.Models.Converters;

namespace HtmlCssToImage.Models.Requests;

/// <summary>Creates a recording from HTML and optional CSS.</summary>
public class CreateHtmlCssRecordingRequest : CreateRecordingCommonOptions
{
    /// <summary>
    /// Google Fonts to load for the recording. Set <c>font-family</c> in your CSS to use them; multiple fonts are sent as a pipe-separated list.
    /// <see href="https://docs.htmlcsstoimage.com/parameters/google_fonts/">Read the Google Fonts documentation for more information.</see>
    /// </summary>
    [JsonConverter(typeof(GoogleFontsJsonConverter))]
    public string[]? GoogleFonts { get; set; }

    /// <summary>
    /// HTML document or fragment to record. Fragments are rendered in a wrapper document.
    /// <see href="https://docs.htmlcsstoimage.com/parameters/html/">Read the HTML documentation for more information.</see>
    /// </summary>
    public required string Html { get; set; }
}
