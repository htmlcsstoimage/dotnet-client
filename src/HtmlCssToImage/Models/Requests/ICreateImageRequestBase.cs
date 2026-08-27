using System.Text.Json.Serialization;

namespace HtmlCssToImage.Models.Requests;

/// <summary>
/// Defines the base interface for image creation request types.
/// </summary>
/// <remarks>
/// This interface serves as the foundation for various types of image creation requests, including
/// generating images from HTML and CSS, URLs, or pre-designed templates. Implementations of this
/// interface specify the necessary parameters and properties for their respective image generation workflows.
/// </remarks>
[JsonDerivedType(typeof(CreateHtmlCssImageRequest))]
[JsonDerivedType(typeof(CreateUrlImageRequest))]
[JsonDerivedType(typeof(CreateTemplatedImageRequest))]
public interface ICreateImageRequestBase
{
    /// <summary>
    /// Gets or sets the file format used in the URL returned by the image creation request.
    /// </summary>
    /// <remarks>
    /// This option is supported for HTML/CSS, URL, and templated image requests, including batch requests. It only changes the extension of the initially returned URL; it does not change the stored image definition or prevent the image from being rendered in another supported format.
    /// When omitted, the API returns its default image URL.
    /// <para>
    /// See <see href="https://docs.htmlcsstoimage.com/getting-started/using-the-api/#file-formats">the file format documentation</see>
    /// for supported formats and URL examples.
    /// </para>
    /// </remarks>
    public RenderImageFormat? Format { get; set; }
}
