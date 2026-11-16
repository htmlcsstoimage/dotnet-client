using System.Text.Json.Serialization;
using HtmlCssToImage.Models.Recordings;

namespace HtmlCssToImage.Models.Requests;

/// <summary>Shared browser settings, duration, triggers, actions, and output options for recording creation. Unspecified options use the API defaults.</summary>
[JsonDerivedType(typeof(CreateHtmlCssRecordingRequest))]
[JsonDerivedType(typeof(CreateUrlRecordingRequest))]
public abstract class CreateRecordingCommonOptions : ICreateRecordingRequestBase
{
    /// <summary>
    /// CSS used to style the recording. For URL recordings, these rules are injected into the page after it loads.
    /// <see href="https://docs.htmlcsstoimage.com/parameters/css/">Read the CSS documentation for more information.</see>
    /// </summary>
    public string? Css { get; set; }

    /// <summary>
    /// Sets the pixel density used to capture the recording. HTML/CSS recordings default to 2; URL recordings default to 1.
    /// <see href="https://docs.htmlcsstoimage.com/parameters/device_scale/">Read the device_scale documentation for more information.</see>
    /// </summary>
    public double? DeviceScale { get; set; }

    /// <summary>
    /// Sets the height of Chrome's viewport in CSS pixels. Supply both <see cref="ViewportHeight"/> and <see cref="ViewportWidth"/> to set the recording size.
    /// <see href="https://docs.htmlcsstoimage.com/parameters/viewport/">Read the viewport documentation for more information.</see>
    /// </summary>
    public int? ViewportHeight { get; set; }

    /// <summary>
    /// Sets the width of Chrome's viewport in CSS pixels. Supply both <see cref="ViewportWidth"/> and <see cref="ViewportHeight"/> to set the recording size.
    /// <see href="https://docs.htmlcsstoimage.com/parameters/viewport/">Read the viewport documentation for more information.</see>
    /// </summary>
    public int? ViewportWidth { get; set; }

    /// <summary>
    /// Disables Twemoji for HTML/CSS recordings when true. URL recordings inject Twemoji only when explicitly false.
    /// <see href="https://docs.htmlcsstoimage.com/parameters/disable_twemoji/">Read the disable_twemoji documentation for more information.</see>
    /// </summary>
    public bool? DisableTwemoji { get; set; }

    /// <summary>
    /// Reuses an identical recording created within this many seconds. Set to 0 to disable deduplication; availability and maximum duration depend on your plan.
    /// <see href="https://docs.htmlcsstoimage.com/parameters/dedupe_duration_s/">Read the dedupe_duration_s documentation for more information.</see>
    /// </summary>
    public int? DedupeDurationS { get; set; }

    /// <summary>
    /// Sets Chrome's preferred light or dark color scheme while recording.
    /// <see href="https://docs.htmlcsstoimage.com/parameters/color_scheme/">Read the color_scheme documentation for more information.</see>
    /// </summary>
    public ColorSchemeType? ColorScheme { get; set; }

    /// <summary>
    /// Sets the browser timezone using an IANA identifier, such as <c>America/New_York</c>.
    /// <see href="https://docs.htmlcsstoimage.com/parameters/timezone/">Read the timezone documentation for more information.</see>
    /// </summary>
    public string? Timezone { get; set; }

    /// <summary>
    /// Enables mobile viewport behavior, including the page's viewport meta tag.
    /// <see href="https://docs.htmlcsstoimage.com/parameters/viewport/">Read the viewport documentation for more information.</see>
    /// </summary>
    public bool? ViewportMobile { get; set; }

    /// <summary>
    /// Sets whether the emulated viewport uses landscape orientation.
    /// <see href="https://docs.htmlcsstoimage.com/parameters/viewport/">Read the viewport documentation for more information.</see>
    /// </summary>
    public bool? ViewportLandscape { get; set; }

    /// <summary>
    /// Sets whether the emulated viewport supports touch events.
    /// <see href="https://docs.htmlcsstoimage.com/parameters/viewport/">Read the viewport documentation for more information.</see>
    /// </summary>
    public bool? ViewportTouch { get; set; }

    /// <summary>
    /// Sets the CSS media type used to render the page while recording.
    /// <see href="https://docs.htmlcsstoimage.com/parameters/media_type/">Read the media_type documentation for more information.</see>
    /// </summary>
    public MediaType? MediaType { get; set; }

    /// <summary>
    /// The ID of the configured organization proxy to use while recording.
    /// <see href="https://docs.htmlcsstoimage.com/guides/advanced/proxies/">Read the proxy documentation for more information.</see>
    /// </summary>
    public string? ProxyId { get; set; }

    /// <summary>
    /// The ID of the configured organization storage destination to receive the recording.
    /// <see href="https://docs.htmlcsstoimage.com/guides/advanced/storage-destinations/">Read the storage destination documentation for more information.</see>
    /// </summary>
    public string? StorageDestinationId { get; set; }

    /// <summary>
    /// Rules that block matching browser network requests while recording. Available on paid plans.
    /// <see href="https://docs.htmlcsstoimage.com/parameters/request_overrides/">Read the request_overrides documentation for more information.</see>
    /// </summary>
    public RequestOverride[]? RequestOverrides { get; set; }

    /// <summary>
    /// Requested recording duration in milliseconds. Accepts 500 to 30000, or up to 90000 with <see cref="RecordingDurationMode.Target"/>. <see cref="DurationMode"/> controls how actions fit into this duration.
    /// <see href="https://docs.htmlcsstoimage.com/parameters/duration_ms/">Read the duration_ms documentation for more information.</see>
    /// </summary>
    public required int DurationMs { get; set; }

    /// <summary>
    /// Controls when recording and actions start after page preparation. When omitted, recording starts immediately.
    /// <see href="https://docs.htmlcsstoimage.com/parameters/trigger/">Read the trigger documentation for more information.</see>
    /// </summary>
    public RecordingTrigger? Trigger { get; set; }

    /// <summary>
    /// Ordered interactions to perform while recording. Leave null or empty to record the page without interactions.
    /// <see href="https://docs.htmlcsstoimage.com/parameters/actions/">Read the actions documentation for more information.</see>
    /// </summary>
    public RecordingAction[]? Actions { get; set; }

    /// <summary>
    /// Output format, frame rate, and scale. Defaults to MP4 at 30 fps. Recordings do not include audio.
    /// <see href="https://docs.htmlcsstoimage.com/parameters/output/">Read the output documentation for more information.</see>
    /// </summary>
    public RecordingOutputOptions? Output { get; set; }

    /// <summary>
    /// Controls how <see cref="DurationMs"/> limits the timeline. Defaults to <see cref="RecordingDurationMode.AtMost"/>, which stops when actions finish or the requested duration is reached.
    /// <see href="https://docs.htmlcsstoimage.com/parameters/duration_mode/">Read the duration_mode documentation for more information.</see>
    /// </summary>
    public RecordingDurationMode? DurationMode { get; set; }

    /// <summary>
    /// When true, waits to render a new recording until its media is requested. Defaults to false and does not reset a deduplicated recording.
    /// <see href="https://docs.htmlcsstoimage.com/parameters/defer_render/">Read the defer_render documentation for more information.</see>
    /// </summary>
    public bool? DeferRender { get; set; }

    /// <summary>
    /// Skips the preparation scroll that loads lazy content before capture. The recording's scrolling actions still run. Defaults to false.
    /// <see href="https://docs.htmlcsstoimage.com/parameters/skip_scroll_preload/">Read the skip_scroll_preload documentation for more information.</see>
    /// </summary>
    public bool? SkipScrollPreload { get; set; }
}
