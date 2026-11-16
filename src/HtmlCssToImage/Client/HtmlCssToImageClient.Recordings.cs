using System.Diagnostics;
using System.Net.Http.Json;
using HtmlCssToImage.Helpers;
using HtmlCssToImage.Models.Requests;
using HtmlCssToImage.Models.Responses;
using HtmlCssToImage.Models.Results;

namespace HtmlCssToImage;

public partial class HtmlCssToImageClient
{
    private const string CREATE_RECORDING_PATH = "/v1/recording";
    private const string RECORDINGS_PATH = "/v1/recordings";

    /// <inheritdoc />
    public async Task<ApiResult<CreateRecordingResponse?>> CreateRecordingAsync<T>(T request,
        CancellationToken cancellationToken = default) where T : ICreateRecordingRequestBase
    {
        HttpResponseMessage response;
        if (request is CreateHtmlCssRecordingRequest html_css_request)
        {
            response = await _client.PostAsJsonAsync(CREATE_RECORDING_PATH, html_css_request,
                JsonContext.Default.CreateHtmlCssRecordingRequest, cancellationToken).ConfigureAwait(false);
        }
        else if (request is CreateUrlRecordingRequest url_request)
        {
            response = await _client.PostAsJsonAsync(CREATE_RECORDING_PATH, url_request,
                JsonContext.Default.CreateUrlRecordingRequest, cancellationToken).ConfigureAwait(false);
        }
        else
        {
            throw new UnreachableException();
        }

        return await response.ParseHttpResponseAsync(
            JsonContext.Default.CreateRecordingResponse, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<ApiResult<RecordingStatusResponse?>> FetchRecordingStatusAsync(string recordingId,
        CancellationToken cancellationToken = default)
    {
        var response = await _client.GetAsync(
            $"{RECORDINGS_PATH}/{recordingId}/status", cancellationToken).ConfigureAwait(false);
        return await response.ParseHttpResponseAsync(
            JsonContext.Default.RecordingStatusResponse, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<ApiResult<bool?>> DeleteRecordingAsync(string recordingId,
        CancellationToken cancellationToken = default)
    {
        var response = await _client.DeleteAsync(
            $"{RECORDINGS_PATH}/{recordingId}", cancellationToken).ConfigureAwait(false);
        return await response.CreateDeleteResultAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<ApiResult<bool?>> DeleteRecordingBatchAsync(IEnumerable<string> recordingIds,
        CancellationToken cancellationToken = default)
    {
        var request = new DeleteRecordingBatchRequest { Ids = recordingIds.ToArray() };
        if (request.Ids.Length == 0)
        {
            return CreateEmptyBatchDeleteResult();
        }

        var request_message = new HttpRequestMessage(HttpMethod.Delete, $"{RECORDINGS_PATH}/batch")
        {
            Content = JsonContent.Create(request, JsonContext.Default.DeleteRecordingBatchRequest)
        };
        var response = await _client.SendOwnedAsync(request_message, cancellationToken).ConfigureAwait(false);
        return await response.CreateDeleteResultAsync(cancellationToken).ConfigureAwait(false);
    }
}
