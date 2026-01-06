using System.Text.Json.Serialization;

namespace HtmlCssToImage.Models.Responses;

/// <summary>
/// Represents the response from an image creation request.
/// This response contains details about the created image, such as its unique identifier
/// and the URL where the image can be accessed.
/// </summary>
public record CreateImageResponse([property:JsonPropertyName("id")] string Id, [property:JsonPropertyName("url")] string Url);