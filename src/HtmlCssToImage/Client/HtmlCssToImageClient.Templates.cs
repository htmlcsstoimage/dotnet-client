using System.Net.Http.Json;
using System.Text;
using HtmlCssToImage.Helpers;
using HtmlCssToImage.Models;
using HtmlCssToImage.Models.Requests;
using HtmlCssToImage.Models.Responses;
using HtmlCssToImage.Models.Results;

namespace HtmlCssToImage;

public partial class HtmlCssToImageClient
{
    internal const string TEMPLATE_BASE_URL = $"{HOST}/v1/template";

    /// <inheritdoc />
    public Task<ApiResult<CreateTemplateResponse?>> CreateTemplateAsync(CreateTemplateRequest request, CancellationToken cancellationToken = default) => CreateTemplateCore(request, cancellationToken: cancellationToken);

    /// <inheritdoc />
    public Task<ApiResult<CreateTemplateResponse?>> CreateTemplateVersionAsync(string templateId, CreateTemplateRequest request, CancellationToken cancellationToken = default) => CreateTemplateCore(request, templateId, cancellationToken);

    private async Task<ApiResult<CreateTemplateResponse?>> CreateTemplateCore(CreateTemplateRequest request, string? templateId = null, CancellationToken cancellationToken = default)
    {
        var url = templateId == null ? TEMPLATE_BASE_URL : $"{TEMPLATE_BASE_URL}/{templateId}";

        var response =await _client.PostAsJsonAsync(url, request, JsonContext.Default.CreateTemplateRequest, cancellationToken).ConfigureAwait(false);

        var result = new ApiResult<CreateTemplateResponse?>()
        {
            HttpResponseMessage = response,
            StatusCode = (int)response.StatusCode,
            Success = response.IsSuccessStatusCode
        };
        if (response.IsSuccessStatusCode)
        {
            var response_data =
                await response.Content.ReadFromJsonAsync<CreateTemplateResponse>(
                    JsonContext.Default.CreateTemplateResponse, cancellationToken).ConfigureAwait(false);
            result.Response = response_data;
        }
        else
        {
            result.ErrorDetails = await response.Content.ReadFromJsonAsync<ErrorDetails>(JsonContext.Default.ErrorDetails, cancellationToken).ConfigureAwait(false);
        }

        return result;
    }

    /// <inheritdoc />
    public  Task<ApiResult<PaginatedResponse<Template>?>>  ListTemplateVersionsAsync(string templateId, int count = 10, long? nextPageStart = null, CancellationToken cancellationToken = default)=>ListTemplatesCore(templateId, (uint)count, nextPageStart, cancellationToken);

    /// <inheritdoc />
    public  Task<ApiResult<PaginatedResponse<Template>?>>  ListTemplatesAsync(int count = 10, long? nextPageStart = null, CancellationToken cancellationToken = default)=>ListTemplatesCore(null, (uint)count, nextPageStart, cancellationToken);

    private async Task<ApiResult<PaginatedResponse<Template>?>> ListTemplatesCore(string? templateId, uint count = 10, long? nextPageStart = null, CancellationToken cancellationToken = default)
    {
        if (count == 0)
        {
            count = 10;
        }
        if (count > 100)
        {
            count = 100;
        }

        var url = GetTemplateListUrl(templateId, count, nextPageStart);
        //var url = $"{TEMPLATE_BASE_URL}{(string.IsNullOrEmpty(templateId)?"":$"/{templateId}")}?count={count}&max_version={nextPageStart ?? long.MaxValue}";

        var response = await _client.GetAsync(url, cancellationToken);
        var result = new ApiResult<PaginatedResponse<Template>?>()
        {
            HttpResponseMessage = response,
            StatusCode = (int)response.StatusCode,
            Success = response.IsSuccessStatusCode
        };
        if (response.IsSuccessStatusCode)
        {
            result.Response = await response.Content.ReadFromJsonAsync<PaginatedResponse<Template>>(JsonContext.Default.PaginatedResponseTemplate, cancellationToken).ConfigureAwait(false);
        }
        else
        {
            result.ErrorDetails = await response.Content.ReadFromJsonAsync<ErrorDetails>(JsonContext.Default.ErrorDetails, cancellationToken).ConfigureAwait(false);
        }

        return result;

    }


    internal static string GetTemplateListUrl(string? templateId, uint count, long? nextPageStart)
    {
        var num_chars = TEMPLATE_BASE_URL.Length+7;
        if (!string.IsNullOrEmpty(templateId))
        {
            num_chars+=templateId.Length+1;
        }

        num_chars += NumberHelpers.GetDigitsCount(count);
        if (nextPageStart.HasValue)
        {
            num_chars += 13;
            num_chars += NumberHelpers.GetDigitsCount((ulong)nextPageStart.Value);
        }

        return string.Create(num_chars, (templateId, count, nextPageStart), (chars, state) =>
        {
            var pos = 0;
            TEMPLATE_BASE_URL.AsSpan().CopyTo(chars[pos..]);
            pos += TEMPLATE_BASE_URL.Length;
            if (!string.IsNullOrEmpty(state.templateId))
            {
                chars[pos++] = '/';
                state.templateId.AsSpan().CopyTo(chars[pos..]);
                pos += state.templateId.Length;
            }

            "?count=".AsSpan().CopyTo(chars[pos..]);
            pos += 7;
            state.count.TryFormat(chars[pos..], out int cW);
            pos += cW;
            if (state.nextPageStart.HasValue)
            {
                "&max_version=".AsSpan().CopyTo(chars[pos..]);
                pos += 13;
                state.nextPageStart.Value.TryFormat(chars[pos..], out int vW);
                pos += vW;
            }
        });
    }
}