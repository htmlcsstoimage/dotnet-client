using System.Text.Json.Serialization;
using HtmlCssToImage.Models.Converters;

namespace HtmlCssToImage.Models;

/// <summary>
/// Represents a template object including attributes for creation, rendering, and styling of HTML and CSS content.
/// </summary>
public class Template
{
    /// <summary>
    /// Gets or sets the date and time at which the template was created.
    /// </summary>
    public DateTime CreatedAt { get; init; }

    /// <summary>
    /// Gets or sets the date and time when the template was last updated.
    /// </summary>
    public DateTime UpdatedAt { get; init; }

    /// <summary>
    /// Gets or sets the name of the template.
    /// This property is used to identify or label the template.
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// Gets or sets a description of the template. This property provides
    /// additional details or metadata about the template's purpose or usage.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Gets or sets the unique identifier of the template.
    /// </summary>
    /// <remarks>
    /// This property represents the primary identifier for the template object.
    /// It is used to distinguish templates within the system and is typically
    /// generated automatically upon template creation.
    /// </remarks>
    [JsonPropertyName("id")]
    public required string TemplateId { get; init; } = null!;

    /// <summary>
    /// Gets or sets the version number of the template.
    /// This property typically represents the sequential versioning
    /// used to track updates or changes made to the template over time.
    /// </summary>
    [JsonPropertyName("version")]
    public long TemplateVersion { get; init; }

    /// <summary>
    /// The count of images creatied using the template version.
    /// This property tracks how many times the template has been used for image generation.
    /// </summary>
    [JsonPropertyName("render_count")]
    public ulong ImageCount { get; init; }


    /// <summary>
    /// Represents the CSS styles associated with the template.
    /// This property allows defining custom styling for rendered HTML content, supporting standard CSS syntax.
    /// </summary>
    public string? Css { get; init; }

    /// <summary>
    /// The HTML content for the template.
    /// This property defines the structure and elements of the rendered output.
    /// </summary>
    public string? Html { get; init; }

    /// <summary>
    /// Adjusts the pixel ratio for the screenshot. The default is 2 which is equivalent to a 4K monitor.
    /// </summary>
    public double? DeviceScale { get; init; }

    /// <summary>
    /// Set the height of Chrome's viewport. This will disable automatic cropping.
    /// </summary>
    public uint? ViewportHeight { get; init; }

    /// <summary>
    /// Set the width of Chrome's viewport. This will disable automatic cropping.
    /// </summary>
    public uint? ViewportWidth { get; init; }

    /// <summary>
    /// Twemoji is used to render emoji as a fallback for native emoji fonts. This option will disable that behavior.
    /// </summary>
    public bool? DisableTwemoji { get; init; }

    /// <summary>
    /// Gets or sets an array of Google Fonts to be used in rendering HTML content.
    /// Each font is represented as a string, and the list of fonts can optionally include specific font weights
    /// and styles as required by the template. The property will parse and convert fonts from their serialized string representation.
    /// </summary>
    [JsonConverter(typeof(GoogleFontsJsonConverter))]
    public string[]? GoogleFonts { get; init; }

    /// <summary>
    /// The maximum amount of time, in milliseconds, to wait for the HTML and CSS content to render.
    /// </summary>
    /// <remarks>
    /// Sets a limit on time to wait until the screenshot is taken. Use this if your page loads a lot of extra irrelevant content, and you want to reduce the render time.
    /// </remarks>
    public uint? MaxWaitMs { get; init; }

    /// <summary>
    /// Adds extra time before taking the screenshot, like if you need to wait for Javascript to execute
    /// </summary>
    public int? MsDelay { get; init; }

    /// <summary>
    /// Gets or sets a value indicating whether rendering should wait until 'ScreenshotReady()' is called from Javascript to take the screenshot
    /// </summary>
    public bool? RenderWhenReady { get; init; }

    /// <summary>
    /// Set Chrome to render assuming the user has explicitly set their browser to Light or Dark mode.
    /// </summary>
    public ColorSchemeType? ColorScheme { get; init; }

    /// <summary>
    /// Gets or sets the timezone used for rendering operations,
    /// typically provided in standard IANA timezone format (e.g., "America/New_York").
    /// </summary>
    public string? Timezone { get; init; }
}