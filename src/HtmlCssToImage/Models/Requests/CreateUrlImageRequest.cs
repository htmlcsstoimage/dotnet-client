namespace HtmlCssToImage.Models.Requests;

/// Represents the data required to create an image from a URL using the specified options.
public class CreateUrlImageRequest:CreateImageCommonOptions, IBatchAllowedImageRequest, ICreateImageRequestBase
{
    /// <summary>
    /// Pass a publicly available URL and the API will screenshot it.
    /// </summary>
    /// <remarks>
    /// This property specifies the target URL of the web page that will be rendered and converted into an image.
    /// The URL must be a valid, publicly reachable web address that conforms to standard URL formats.
    /// </remarks>
    public required string Url { get; set; }

    /// <summary>
    /// Specifies custom CSS rules to apply to the target webpage during rendering.
    /// </summary>
    /// <remarks>
    /// This property allows inline CSS styling to be injected into the webpage before it is rendered into an image.
    /// The provided CSS must be valid and is expected to follow standard CSS syntax. Use this property to customize
    /// the appearance of the webpage, such as styling specific elements, overriding existing styles, or adding new styles.
    /// </remarks>
    public string? Css { get; set; }

    /// <summary>
    /// Indicates whether the screenshot should capture the entire webpage in full height.
    /// </summary>
    /// <remarks>
    /// When set to true, this property ensures that the screenshot includes the full vertical content of the webpage,
    /// scrolling beyond the visible portion of the viewport if necessary. If set to false or null, only the visible
    /// portion of the webpage within the configured viewport dimensions will be captured.
    /// </remarks>
    public bool? FullScreen { get; set; }

    /// <summary>
    /// Attempt to block cookie/consent banners from displaying.
    /// </summary>
    public bool? BlockConsentBanners { get; set; }
}