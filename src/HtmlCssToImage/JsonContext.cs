using System.Text.Json.Serialization;
using HtmlCssToImage.Models;
using HtmlCssToImage.Models.Converters;
using HtmlCssToImage.Models.Requests;
using HtmlCssToImage.Models.Responses;
using HtmlCssToImage.Models.Results;

namespace HtmlCssToImage;

/// <summary>
/// Represents a JSON serialization context for handling models in the HtmlCssToImage namespace.
/// This context is configured with specific serialization options and custom converters for
/// seamless processing of JSON data related to image generation requests and responses.
/// The configuration includes:
/// - String enumeration conversion.
/// - Allowing numbers to be read as strings.
/// - Support for trailing commas.
/// - Snake case naming policy.
/// - Respect for nullable annotations.
/// - Ignoring null properties during serialization.
/// This context predefines serializable types related to image generation,
/// including requests, responses, and other associated models.
/// </summary>
/// <remarks>
/// The JsonContext class uses custom JSON converters for specialized data types such as:
/// - <see cref="PdfValueWithUnitsNullableConverter"/>
/// - <see cref="PdfValueWithUnitsConverter"/>
/// - <see cref="PdfMarginsConverter"/>
/// Predefined serializable types include:
/// - <see cref="CreateHtmlCssImageRequest"/>
/// - <see cref="CreateUrlImageRequest"/>
/// - <see cref="CreateTemplatedImageRequest"/>
/// - <see cref="CreateImageBatchRequest{T}"/> (for specific batch image types)
/// - <see cref="CreateImageBatchResponse"/>
/// - <see cref="CreateImageResponse"/>
/// - <see cref="ErrorDetails"/>
/// - <see cref="PdfValueWithUnits"/>
/// </remarks>
[JsonSourceGenerationOptions(UseStringEnumConverter = true, NumberHandling = JsonNumberHandling.AllowReadingFromString, AllowTrailingCommas = true, PropertyNamingPolicy = JsonKnownNamingPolicy.SnakeCaseLower, RespectNullableAnnotations = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    Converters = [typeof(PdfValueWithUnitsNullableConverter),typeof(PdfValueWithUnitsConverter), typeof(PdfMarginsConverter)])]
[JsonSerializable(typeof(CreateHtmlCssImageRequest))]
[JsonSerializable(typeof(CreateUrlImageRequest))]
[JsonSerializable(typeof(CreateTemplatedImageRequest))]
[JsonSerializable(typeof(CreateImageBatchRequest<CreateUrlImageRequest>))]
[JsonSerializable(typeof(CreateImageBatchRequest<CreateHtmlCssImageRequest>))]
[JsonSerializable(typeof(CreateImageBatchResponse))]
[JsonSerializable(typeof(CreateImageResponse))]
[JsonSerializable(typeof(ErrorDetails))]
[JsonSerializable(typeof(PdfValueWithUnits[]))]
public partial class JsonContext:JsonSerializerContext
{

}