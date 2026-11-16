using System.Diagnostics;
using System.Net.Http.Json;
using HtmlCssToImage.Helpers;
using HtmlCssToImage.Models.Requests;
using HtmlCssToImage.Models.Responses;
using HtmlCssToImage.Models.Results;

namespace HtmlCssToImage;

public partial class HtmlCssToImageClient
{
    /// <inheritdoc />
    public async Task<ApiResult<bool?>> DeleteImageAsync(string imageId, CancellationToken cancellationToken = default)
    {
        var response = await _client.DeleteAsync(
            $"{CREATE_OR_GET_PATH}/{imageId}",
            cancellationToken).ConfigureAwait(false);

        return await response.CreateDeleteResultAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<ApiResult<bool?>> DeleteImageBatchAsync(IEnumerable<string> imageIds,
        CancellationToken cancellationToken = default)
    {
        var request = new DeleteImageBatchRequest
        {
            Ids = imageIds.ToArray()
        };
        if (request.Ids.Length == 0)
        {
            return CreateEmptyBatchDeleteResult();
        }

        var request_message = new HttpRequestMessage(HttpMethod.Delete, BATCH_URL)
        {
            Content = JsonContent.Create(request, JsonContext.Default.DeleteImageBatchRequest)
        };
        var response = await _client.SendOwnedAsync(request_message, cancellationToken).ConfigureAwait(false);

        return await response.CreateDeleteResultAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <summary>A successful batch delete result for an empty ID list, returned without sending a request.</summary>
    private static ApiResult<bool?> CreateEmptyBatchDeleteResult() => new() { Success = true, Response = true, StatusCode = 200, HttpResponseMessage = null };

    /// <inheritdoc />
    public async Task<ApiResult<CreateImageResponse[]?>> CreateImageBatchAsync<T>(T? defaultOptions, IEnumerable<T> variations,
        CancellationToken cancellationToken = default) where T : IBatchAllowedImageRequest
    {
        var request = new CreateImageBatchRequest<T>
        {
            DefaultOptions = defaultOptions
        };
        request.Variations.AddRange(variations);
        return await CreateImageBatchAsync(request, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<ApiResult<CreateImageResponse[]?>> CreateImageBatchAsync<T>(CreateImageBatchRequest<T> request,
        CancellationToken cancellationToken = default) where T : IBatchAllowedImageRequest
    {
        HttpResponseMessage response;
        if (request is CreateImageBatchRequest<CreateUrlImageRequest> url_request)
        {
            response = await _client.PostAsJsonAsync(BATCH_URL, url_request, JsonContext.Default.CreateImageBatchRequestCreateUrlImageRequest, cancellationToken).ConfigureAwait(false);
        }
        else if (request is CreateImageBatchRequest<CreateHtmlCssImageRequest> html_request)
        {
            response = await _client.PostAsJsonAsync(BATCH_URL, html_request, JsonContext.Default.CreateImageBatchRequestCreateHtmlCssImageRequest, cancellationToken).ConfigureAwait(false);
        }
        else
        {
            throw new UnreachableException();
        }

        return await CreateBatchResultAsync(response, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<ApiResult<CreateImageResponse[]?>> CreateTemplatedImageBatchAsync(CreateTemplatedImageBatchRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _client.PostAsJsonAsync(BATCH_TEMPLATED_URL, request, JsonContext.Default.CreateTemplatedImageBatchRequest, cancellationToken).ConfigureAwait(false);
        return await CreateBatchResultAsync(response, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public Task<ApiResult<CreateImageResponse[]?>> CreateTemplatedImageBatchAsync(TemplatedBatchImageOptions? defaultOptions, IEnumerable<TemplatedBatchImageOptions> variations, CancellationToken cancellationToken = default) =>
        CreateTemplatedImageBatchAsync(new CreateTemplatedImageBatchRequest { DefaultOptions = defaultOptions, Variations = variations.ToList() }, cancellationToken);

    private static async Task<ApiResult<CreateImageResponse[]?>> CreateBatchResultAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        var result = new ApiResult<CreateImageResponse[]?>()
        {
            HttpResponseMessage = response,
            StatusCode = (int)response.StatusCode,
            Success = response.IsSuccessStatusCode
        };
        try
        {
            if (response.IsSuccessStatusCode)
            {
                var response_data =
                    await response.Content.ReadFromJsonAsync<CreateImageBatchResponse>(
                        JsonContext.Default.CreateImageBatchResponse, cancellationToken);
                result.Response = response_data?.Images ?? [];
            }
            else
            {
                result.ErrorDetails = await response.Content.ReadFromJsonAsync<ErrorDetails>(JsonContext.Default.ErrorDetails, cancellationToken);
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

    /// <inheritdoc />
    public async Task<ApiResult<CreateImageResponse?>> CreateImageAsync<T>(T request, CancellationToken cancellationToken = default) where T : ICreateImageRequestBase
    {
        HttpResponseMessage response;
        if (request is CreateTemplatedImageRequest templated_request)
        {
            response = await _client.PostAsJsonAsync(CREATE_URL, templated_request,
                JsonContext.Default.CreateTemplatedImageRequest, cancellationToken).ConfigureAwait(false);
        }
        else if (request is CreateHtmlCssImageRequest html_css_request)
        {
            response = await _client.PostAsJsonAsync(CREATE_URL, html_css_request,
                JsonContext.Default.CreateHtmlCssImageRequest, cancellationToken).ConfigureAwait(false);
        }
        else if (request is CreateUrlImageRequest url_request)
        {
            response = await _client.PostAsJsonAsync(CREATE_URL, url_request,
                JsonContext.Default.CreateUrlImageRequest, cancellationToken).ConfigureAwait(false);
        }
        else
        {
            throw new UnreachableException();
        }

        return await response.ParseHttpResponseAsync(
            JsonContext.Default.CreateImageResponse, cancellationToken).ConfigureAwait(false);
    }
}
