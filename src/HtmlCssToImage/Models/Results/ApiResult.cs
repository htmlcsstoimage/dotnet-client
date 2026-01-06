using System.Diagnostics.CodeAnalysis;

namespace HtmlCssToImage.Models.Results;

/// <summary>
/// Represents the result of an API operation, providing information about the success or failure of the operation,
/// the associated response data, and additional metadata related to the HTTP response.
/// </summary>
/// <typeparam name="T">The type of the response data returned by the API if the operation is successful.</typeparam>
public class ApiResult<T>
{
    /// <summary>
    /// The response item, when successful
    /// </summary>
    public T? Response { get; set; }

    /// <summary>
    /// True if the request was successful, false otherwise.
    /// </summary>
    [MemberNotNullWhen(true, nameof(Response))]
    [MemberNotNullWhen(false, nameof(ErrorDetails))]
    public bool Success { get; set; }

    /// <summary>
    /// Error details, if any
    /// </summary>
    public ErrorDetails? ErrorDetails { get; set; }

    /// <summary>
    /// The HTTP status code returned by the API.
    /// </summary>
    public int StatusCode { get; set; }

    /// <summary>
    /// The raw HTTP response message returned by the API.
    /// </summary>
    public HttpResponseMessage? HttpResponseMessage { get; internal set; } = null!;
}