using System.Diagnostics;
using System.Net.Http.Json;
using HtmlCssToImage.Models.Requests;
using HtmlCssToImage.Models.Responses;
using HtmlCssToImage.Models.Results;

namespace HtmlCssToImage;

public partial class HtmlCssToImageClient
{
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
            response = await _client.PostAsJsonAsync(CREATE_BATCH_URL, url_request, JsonContext.Default.CreateImageBatchRequestCreateUrlImageRequest, cancellationToken).ConfigureAwait(false);
        }
        else if (request is CreateImageBatchRequest<CreateHtmlCssImageRequest> html_request)
        {
            response = await _client.PostAsJsonAsync(CREATE_BATCH_URL, html_request, JsonContext.Default.CreateImageBatchRequestCreateHtmlCssImageRequest, cancellationToken).ConfigureAwait(false);
        }
        else
        {
            throw new UnreachableException();
        }

        var result = new ApiResult<CreateImageResponse[]?>()
        {
            HttpResponseMessage = response,
            StatusCode = (int)response.StatusCode,
            Success = response.IsSuccessStatusCode
        };
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

        var result = new ApiResult<CreateImageResponse?>()
        {
            HttpResponseMessage = response,
            StatusCode = (int)response.StatusCode,
            Success = response.IsSuccessStatusCode
        };
        if (response.IsSuccessStatusCode)
        {
            result.Response = await response.Content.ReadFromJsonAsync<CreateImageResponse>(JsonContext.Default.CreateImageResponse, cancellationToken).ConfigureAwait(false);
        }
        else
        {
            result.ErrorDetails = await response.Content.ReadFromJsonAsync<ErrorDetails>(JsonContext.Default.ErrorDetails, cancellationToken).ConfigureAwait(false);
        }

        return result;
    }
}