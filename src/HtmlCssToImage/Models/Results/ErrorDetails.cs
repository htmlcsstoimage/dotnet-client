using System.Text.Json.Serialization;
using HtmlCssToImage.Models.Responses;

namespace HtmlCssToImage.Models.Results;

/// <summary>
/// Represents the details of an error that occurred during an operation.
/// </summary>
public class ErrorDetails
{
    /// <summary>
    /// The error message describing the specific error that occurred.
    /// </summary>
    [JsonPropertyName("message")]
    public string Message { get; init; }= null!;

    /// <summary>
    /// The type of error that occurred.
    /// </summary>
    [JsonPropertyName("error")] public string ErrorType { get; init; } = null!;

    /// <summary>
    /// A list of validation errors that occurred during the request.
    /// </summary>
    [JsonPropertyName("validationErrors")]
    public List<ValidationError> ValidationErrors { get; set; } = [];

}