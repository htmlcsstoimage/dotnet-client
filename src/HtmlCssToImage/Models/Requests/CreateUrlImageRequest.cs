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
    /// Gets or sets custom HTTP headers for top-level requests to the requested URL's origin and any <see cref="AdditionalHeaderOrigins"/>.
    /// <see href="https://docs.htmlcsstoimage.com/parameters/headers/">Read the headers documentation for more information.</see>
    /// </summary>
    public Dictionary<string, string>? Headers { get; set; }

    /// <summary>
    /// Gets or sets additional exact HTTP or HTTPS origins allowed to receive custom <see cref="Headers"/>.
    /// <see href="https://docs.htmlcsstoimage.com/parameters/headers/#additional-header-origins">Read the additional_header_origins documentation for more information.</see>
    /// </summary>
    public string[]? AdditionalHeaderOrigins { get; set; }

    /// <summary>
    /// Gets or sets whether custom <see cref="Headers"/> should also be sent with subrequests to allowed origins.
    /// <see href="https://docs.htmlcsstoimage.com/parameters/headers/#include-headers-on-subrequests">Read the subrequest header documentation for more information.</see>
    /// </summary>
    public bool? IncludeHeadersOnSubrequests { get; set; }

    /// <summary>
    /// Gets or sets whether the top-level page request should include <c>X-HCTI-SCREENSHOT: 1</c>.
    /// <see href="https://docs.htmlcsstoimage.com/parameters/identify_as_hcti/">Read the identify_as_hcti documentation for more information.</see>
    /// </summary>
    public bool? IdentifyAsHcti { get; set; }

    /// <summary>
    /// Indicates whether the screenshot should capture the entire webpage in full height.
    /// <see href="https://docs.htmlcsstoimage.com/parameters/full_screen/">Read the full_screen Guide for more information.</see>
    /// </summary>
    /// <remarks>
    /// When set to true, this property ensures that the screenshot includes the full vertical content of the webpage,
    /// scrolling beyond the visible portion of the viewport if necessary. If set to false or null, only the visible
    /// portion of the webpage within the configured viewport dimensions will be captured.
    /// </remarks>
    public bool? FullScreen { get; set; }

    /// <summary>
    /// Attempt to block cookie/consent banners from displaying.
    /// <see href="https://docs.htmlcsstoimage.com/guides/advanced/blocking-cookie-banners/">Read the Blocking Cookie Banners Guide for more information.</see>
    /// </summary>
    public bool? BlockConsentBanners { get; set; }
}
