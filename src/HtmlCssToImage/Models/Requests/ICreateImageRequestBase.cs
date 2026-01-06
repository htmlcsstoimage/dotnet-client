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

}