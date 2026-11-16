using System.Net.Http.Json;
using System.Text.Json.Serialization.Metadata;
using HtmlCssToImage.Models.Results;

namespace HtmlCssToImage.Helpers;

internal static class HttpResponseMessageExtensions
{
    /// <summary>
    /// Sends a request this client created. <see cref="HttpClient"/> never disposes the request, so it is disposed here
    /// when sending fails. On success, ensures the response links to the request so <see cref="ApiResult{T}"/>
    /// disposes it even when a custom handler does not populate <see cref="HttpResponseMessage.RequestMessage"/>.
    /// </summary>
    internal static async Task<HttpResponseMessage> SendOwnedAsync(this HttpClient client, HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);
            response.RequestMessage ??= request;
            return response;
        }
        catch
        {
            request.Dispose();
            throw;
        }
    }

    internal static async Task<ApiResult<T?>> ParseHttpResponseAsync<T>(this HttpResponseMessage response,
        JsonTypeInfo<T> jsonType, CancellationToken cancellationToken) where T : class
    {
        var result = new ApiResult<T?>
        {
            HttpResponseMessage = response,
            StatusCode = (int)response.StatusCode,
            Success = response.IsSuccessStatusCode
        };
        try
        {
            if (response.IsSuccessStatusCode)
            {
                result.Response = await response.Content.ReadFromJsonAsync(jsonType, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                result.ErrorDetails = await response.Content.ReadFromJsonAsync(JsonContext.Default.ErrorDetails, cancellationToken).ConfigureAwait(false);
            }
        }
        catch
        {
            // The caller never receives the result, so release the response and its request here.
            result.Dispose();
            throw;
        }

        return result;
    }

    internal static async Task<ApiResult<bool?>> CreateDeleteResultAsync(this HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        var result = new ApiResult<bool?>
        {
            HttpResponseMessage = response,
            StatusCode = (int)response.StatusCode,
            Success = response.IsSuccessStatusCode
        };

        if (response.IsSuccessStatusCode)
        {
            result.Response = true;
            return result;
        }

        try
        {
            result.ErrorDetails = await response.Content.ReadFromJsonAsync(JsonContext.Default.ErrorDetails, cancellationToken).ConfigureAwait(false);
        }
        catch
        {
            // The caller never receives the result, so release the response and its request here.
            result.Dispose();
            throw;
        }

        return result;
    }
}
