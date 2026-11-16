namespace HtmlCssToImage.Models.Requests;

/// <summary>Creates a recording from a public HTTP or HTTPS URL.</summary>
public class CreateUrlRecordingRequest : CreateRecordingCommonOptions
{
    /// <summary>
    /// Public HTTP or HTTPS URL to record.
    /// <see href="https://docs.htmlcsstoimage.com/parameters/url/">Read the URL documentation for more information.</see>
    /// </summary>
    public required string Url { get; set; }

    /// <summary>
    /// Attempts to hide cookie and consent banners before recording.
    /// <see href="https://docs.htmlcsstoimage.com/guides/advanced/blocking-cookie-banners/">Read the blocking cookie banners documentation for more information.</see>
    /// </summary>
    public bool? BlockConsentBanners { get; set; }

    /// <summary>
    /// Adds <c>X-HCTI-SCREENSHOT: 1</c> to the top-level page navigation to identify the capture request.
    /// <see href="https://docs.htmlcsstoimage.com/parameters/identify_as_hcti/">Read the identify_as_hcti documentation for more information.</see>
    /// </summary>
    public bool? IdentifyAsHcti { get; set; }

    /// <summary>
    /// Custom HTTP headers for top-level navigations to the URL's origin and any <see cref="AdditionalHeaderOrigins"/>.
    /// <see href="https://docs.htmlcsstoimage.com/parameters/headers/">Read the headers documentation for more information.</see>
    /// </summary>
    public Dictionary<string, string>? Headers { get; set; }

    /// <summary>
    /// Additional exact HTTP or HTTPS origins allowed to receive <see cref="Headers"/>. Use <c>scheme://host[:port]</c> without a path.
    /// <see href="https://docs.htmlcsstoimage.com/parameters/headers/#additional-header-origins">Read the additional header origins documentation for more information.</see>
    /// </summary>
    public string[]? AdditionalHeaderOrigins { get; set; }

    /// <summary>
    /// Sends <see cref="Headers"/> with subrequests to allowed origins as well as top-level navigations. Defaults to false.
    /// <see href="https://docs.htmlcsstoimage.com/parameters/headers/#include-headers-on-subrequests">Read the subrequest headers documentation for more information.</see>
    /// </summary>
    public bool? IncludeHeadersOnSubrequests { get; set; }
}
