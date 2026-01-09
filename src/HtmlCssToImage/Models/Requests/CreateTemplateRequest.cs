using System.Text.Json.Serialization;
using HtmlCssToImage.Models.Converters;

namespace HtmlCssToImage.Models.Requests;

/// <summary>
/// Represents a request to create a template using the HtmlCssToImage API.
/// This request encapsulates various parameters required for rendering templates
/// including HTML content, CSS styles, rendering preferences, and more.
/// </summary>
public class CreateTemplateRequest
{
    /// <summary>
    /// The name to use for the template, for your reference only.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// A description of the template, for your reference only.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// A CSS selector to target a specific element on the page. The API will crop the image to the dimensions of this element.
    /// </summary>
    public string? Selector { get; set; }

    /// <summary>
    /// Adjusts the pixel ratio for the screenshot. The default is 2 which is equivalent to a 4K monitor.
    /// </summary>
    public double? DeviceScale { get; set; }

    /// <summary>
    /// Set the height of Chrome's viewport. This will disable automatic cropping.
    /// </summary>
    public uint? ViewportHeight { get; set; }

    /// <summary>
    /// Set the width of Chrome's viewport. This will disable automatic cropping.
    /// </summary>
    public uint? ViewportWidth { get; set; }

    /// <summary>
    /// Sets a limit on time to wait until the screenshot is taken. Use this if your page loads a lot of extra irrelevant content, and you want to reduce the render time.
    /// </summary>
    public uint? MaxWaitMs { get; set; }

    /// <summary>
    /// Adds extra time before taking the screenshot, like if you need to wait for Javascript to execute
    /// </summary>
    public uint? MsDelay { get; set; }

    /// <summary>
    /// This will wait until 'ScreenshotReady()' is called from Javascript to take the screenshot.
    /// </summary>
    public bool? RenderWhenReady { get; set; }

    /// <summary>
    /// Ensure the image is only ever rendered and saved one time. This is an advanced option not applicable to most scenarios.
    /// </summary>
    public bool? MaxRenderOnce { get; set; }

    /// <summary>
    /// Twemoji is used to render emoji as a fallback for native emoji fonts. This option will disable that behavior.
    /// </summary>
    public bool? DisableTwemoji { get; set; }


    /// <summary>
    /// Set Chrome to render assuming the user has explicitly set their browser to Light or Dark mode.
    /// </summary>
    public ColorSchemeType? ColorScheme { get; set; }

    /// <summary>
    /// Sets the timezone for the browser instance. Use IANA timezone format (e.g. 'America/New_York').
    /// </summary>
    public string? Timezone { get; set; }

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