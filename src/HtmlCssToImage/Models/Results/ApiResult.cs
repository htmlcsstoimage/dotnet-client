using System.Diagnostics.CodeAnalysis;

namespace HtmlCssToImage.Models.Results;

/// <summary>
/// Represents the result of an API operation, providing information about the success or failure of the operation,
/// the associated response data, and additional metadata related to the HTTP response.
/// </summary>
/// <typeparam name="T">The type of the response data returned by the API if the operation is successful.</typeparam>
/// <remarks>
/// Dispose the result after reading it to release its owned <see cref="System.Net.Http.HttpResponseMessage"/>.
/// </remarks>
public sealed class ApiResult<T> : IDisposable
{
    private bool _disposed;

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
    /// The raw HTTP response message returned by the API. Use this for advanced scenarios that
    /// need response headers, the originating <see cref="System.Net.Http.HttpResponseMessage.RequestMessage"/>,
    /// the HTTP version, reason phrase, or other transport-level details. This message is disposed
    /// when the <see cref="ApiResult{T}"/> is disposed.
    /// </summary>
    public HttpResponseMessage? HttpResponseMessage { get; internal set; } = null!;

    /// <summary>
    /// Releases the owned HTTP response message.
    /// </summary>
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        HttpResponseMessage?.Dispose();
        _disposed = true;
    }
}
