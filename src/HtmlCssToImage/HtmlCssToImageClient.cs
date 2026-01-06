using System.Buffers;
using System.Buffers.Text;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization.Metadata;
using System.Web;
using HtmlCssToImage.Helpers;
using HtmlCssToImage.Models;
using HtmlCssToImage.Models.Requests;
using HtmlCssToImage.Models.Responses;
using HtmlCssToImage.Models.Results;

namespace HtmlCssToImage;

/// <inheritdoc />
public class HtmlCssToImageClient : IHtmlCssToImageClient
{
    private readonly HttpClient _client;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly string _apiKey;

    private readonly string _apiId;



    /// <summary>
    /// A client for interacting with the HtmlCssToImage service, providing functionality for creating images using HTML and CSS input, managing templates, and generating rendered image URLs.
    /// </summary>
    public HtmlCssToImageClient(HttpClient client, HtmlCssToImageOptions options)
    {
        _client = client;
        _client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("HCTIDotNet", LibraryInfo.Version));
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", options.AuthHeader());
        _apiKey = options.ApiKey;
        _apiId = options.ApiId;
    }

    private const string HOST = "https://hcti.io";
    private const string CREATE_PATH = $"{HOST}/v1/image";
    private const string CREATE_AND_RENDER_PATH = $"{CREATE_PATH}/create-and-render";
    private const string CREATE_URL = $"{CREATE_PATH}?includeId=true";
    private const string CREATE_BATCH_URL = $"{CREATE_PATH}/batch";

    private const string TEMPLATE_VERSION_QUERY_PARAM = "template_version";

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
        CancellationToken cancellationToken = default) where T:IBatchAllowedImageRequest
    {
        HttpResponseMessage response;
        if (request is CreateImageBatchRequest<CreateUrlImageRequest> url_request)
        {
           response = await _client.PostAsJsonAsync(CREATE_BATCH_URL, url_request, JsonContext.Default.CreateImageBatchRequestCreateUrlImageRequest, cancellationToken).ConfigureAwait(false);
        }else if (request is CreateImageBatchRequest<CreateHtmlCssImageRequest> html_request)
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

    /// <inheritdoc />
    public string CreateTemplatedImageUrl<T>(string templateId, T templateValues,
        JsonTypeInfo<T> typeInfo, long? templateVersion = null, RenderImageFormat format = RenderImageFormat.PNG) => CreateTemplatedImageUrl(templateId, templateValues, templateVersion, typeInfo, null, format);

    /// <inheritdoc />
    [RequiresUnreferencedCode("If AOT is needed, use one of the overloads with explicit type information")]
    [RequiresDynamicCode("If AOT is needed, use one of the overloads with explicit type information")]
    public string CreateTemplatedImageUrl<T>(string templateId, T templateValues, long? templateVersion = null, RenderImageFormat format = RenderImageFormat.PNG) =>
        CreateTemplateImageUrlNoTypeInfo(templateId, templateValues, templateVersion, format);

    /// <inheritdoc />
    public string CreateTemplatedImageUrl<T>(string templateId, T templateValues,
        JsonSerializerOptions jsonSerializerOptions, long? templateVersion = null, RenderImageFormat format = RenderImageFormat.PNG) =>
        CreateTemplatedImageUrl(templateId, templateValues, templateVersion, null, jsonSerializerOptions, format);

    [RequiresUnreferencedCode("If AOT is needed, use one of the overloads with explicit type information")]
    [RequiresDynamicCode("If AOT is needed, use one of the overloads with explicit type information")]
    private string CreateTemplateImageUrlNoTypeInfo<T>(string templateId, T templateValues, long? templateVersion = null, RenderImageFormat format = RenderImageFormat.PNG)
    {
        var serialized_values = JsonSerializer.SerializeToNode(templateValues);
        if (serialized_values == null || serialized_values.GetValueKind() != JsonValueKind.Object)
        {
            throw new ArgumentException("Invalid parameter values");
        }

        return CreateTemplatedImageUrl(templateId, serialized_values.AsObject(), templateVersion, format);
    }

    private string CreateTemplatedImageUrl<T>(string templateId, T templateValues, long? templateVersion = null,
        JsonTypeInfo<T>? typeInfo = null, JsonSerializerOptions? jsonSerializerOptions = null, RenderImageFormat format = RenderImageFormat.PNG)
    {
        JsonNode? serialized_values;
        if (typeInfo != null)
        {
            serialized_values = JsonSerializer.SerializeToNode(templateValues, typeInfo);
        }
        else if (jsonSerializerOptions != null)
        {
            serialized_values = JsonSerializer.SerializeToNode(templateValues, jsonSerializerOptions.GetTypeInfo(typeof(T)));
        }
        else
        {
            throw new ArgumentException("Must provide either typeInfo or jsonSerializerOptions");
        }

        if (serialized_values == null || serialized_values.GetValueKind() != JsonValueKind.Object)
        {
            throw new ArgumentException("Invalid parameter values");
        }

        return CreateTemplatedImageUrl(templateId, serialized_values.AsObject(), templateVersion, format);
    }

    /// <inheritdoc />
    public string CreateTemplatedImageUrl(string templateId, JsonObject templateValues, long? templateVersion = null, RenderImageFormat format = RenderImageFormat.PNG)
    {
        char[]? rented = null;
        Span<char> chars = stackalloc char[512];
        var curr_char = 0;
        if (templateVersion.HasValue)
        {
            QueryStringEncoder.Write(TEMPLATE_VERSION_QUERY_PARAM, templateVersion.Value, ref curr_char, ref chars, ref rented);
        }

        foreach (var (key, value) in templateValues.OrderBy(x => x.Key))
        {
            if (value is not null)
            {
                QueryStringEncoder.Encode(key, value.ToJsonString(), ref curr_char, ref chars, ref rented);
            }
        }

        var token = HmacToken.CreateToken(chars[..curr_char], _apiKey);
        var format_string = string.Empty;
        if (format != RenderImageFormat.PNG)
        {
            format_string = $"/{format.RenderFormatToExtensionWithoutDot()}";
        }

        var url = $"{CREATE_PATH}/{templateId}/{token}{format_string}?{chars[..curr_char]}";

        if (rented != null)
        {
            ArrayPool<char>.Shared.Return(rented);
        }

        return url;
    }

    /// <inheritdoc />
    public string CreateAndRenderUrl(CreateUrlImageRequest request, RenderImageFormat format = RenderImageFormat.PNG)
    {
        char[]? rented = null;
        Span<char> chars = stackalloc char[512];
        var curr_char = 0;
        QueryStringEncoder.Encode("url", request.Url, ref curr_char, ref chars, ref rented);
        if (!string.IsNullOrWhiteSpace(request.Css))
        {
            QueryStringEncoder.Encode("css", request.Css, ref curr_char, ref chars, ref rented);
        }

        if (!string.IsNullOrWhiteSpace(request.Selector))
        {
            QueryStringEncoder.Encode("selector", request.Selector, ref curr_char, ref chars, ref rented);
        }

        if (!string.IsNullOrWhiteSpace(request.Timezone))
        {
            QueryStringEncoder.Encode("timezone", request.Timezone, ref curr_char, ref chars, ref rented);
        }

        if (request.FullScreen == true)
        {
            QueryStringEncoder.Encode("full_screen", "true", ref curr_char, ref chars, ref rented);
        }

        if (request.BlockConsentBanners == true)
        {
            QueryStringEncoder.Encode("block_consent_banners", "true", ref curr_char, ref chars, ref rented);
        }

        if (request.DisableTwemoji == true)
        {
            QueryStringEncoder.Encode("disable_twemoji", "true", ref curr_char, ref chars, ref rented);
        }

        if (request.MaxRenderOnce == true)
        {
            QueryStringEncoder.Encode("max_render_once", "true", ref curr_char, ref chars, ref rented);
        }

        if (request.RenderWhenReady == true)
        {
            QueryStringEncoder.Encode("render_when_ready", "true", ref curr_char, ref chars, ref rented);
        }

        if (request.ColorScheme != null)
        {
            QueryStringEncoder.Encode("color_scheme", request.ColorScheme.Value.ToString(), ref curr_char, ref chars, ref rented);
        }

        if (request.DeviceScale != null)
        {
            QueryStringEncoder.Write("device_scale", request.DeviceScale.Value, ref curr_char, ref chars, ref rented);
        }

        if (request.MaxWaitMs != null)
        {
            QueryStringEncoder.Write("max_wait_ms", request.MaxWaitMs.Value, ref curr_char, ref chars, ref rented);
        }

        if (request.MsDelay != null)
        {
            QueryStringEncoder.Write("ms_delay", request.MsDelay.Value, ref curr_char, ref chars, ref rented);
        }

        if (request.ViewportHeight != null)
        {
            QueryStringEncoder.Write("viewport_height", request.ViewportHeight.Value, ref curr_char, ref chars, ref rented);
        }

        if (request.ViewportWidth != null)
        {
            QueryStringEncoder.Write("viewport_width", request.ViewportWidth.Value, ref curr_char, ref chars, ref rented);
        }

        var token = HmacToken.CreateToken(chars[..curr_char], _apiKey);

        var format_string = string.Empty;
        if (format != RenderImageFormat.PNG)
        {
            format_string = $"/{format.RenderFormatToExtensionWithoutDot()}";
        }

        var url = $"{CREATE_AND_RENDER_PATH}/{_apiId}/{token}{format_string}?{chars[..curr_char]}";

        if (rented != null)
        {
            ArrayPool<char>.Shared.Return(rented);
        }

        return url;
    }
}