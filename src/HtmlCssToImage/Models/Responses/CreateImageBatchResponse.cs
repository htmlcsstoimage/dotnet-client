using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace HtmlCssToImage.Models.Responses;

/// <summary>
/// Represents a response containing a batch of created images.
/// This response includes an array of individual image creation results encapsulated
/// in <see cref="CreateImageResponse"/> objects.
/// </summary>
public record CreateImageBatchResponse([property: JsonPropertyName("images")] CreateImageResponse[] Images);