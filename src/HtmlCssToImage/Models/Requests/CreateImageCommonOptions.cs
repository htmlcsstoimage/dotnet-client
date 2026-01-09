using System.Text.Json.Serialization;

namespace HtmlCssToImage.Models.Requests;

/// <summary>
/// Represents common configuration options for creating an image using HTML/CSS or a URL.
/// This abstract class provides shared properties to define the rendering and processing behavior
/// for image generation requests.
/// </summary>
[JsonDerivedType(typeof(CreateHtmlCssImageRequest))]
[JsonDerivedType(typeof(CreateUrlImageRequest))]
public abstract class CreateImageCommonOptions
{
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
        /// Options for generating a PDF from the HTML/CSS.
        /// </summary>
        public PDFOptions? PDFOptions { get; set; }

        /// <summary>
        /// Set Chrome to render assuming the user has explicitly set their browser to Light or Dark mode.
        /// </summary>
        public ColorSchemeType? ColorScheme { get; set; }

        /// <summary>
        /// Sets the timezone for the browser instance. Use IANA timezone format (e.g. 'America/New_York').
        /// </summary>
        public string? Timezone { get; set; }

}