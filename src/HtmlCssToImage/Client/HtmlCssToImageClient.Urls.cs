using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization.Metadata;
using HtmlCssToImage.Helpers;
using HtmlCssToImage.Models;
using HtmlCssToImage.Models.Requests;

namespace HtmlCssToImage;

public partial class HtmlCssToImageClient
{
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
        ArrayOrSpan<char> chars = new(stackalloc char[512]);
        try
        {
            if (templateVersion.HasValue)
            {
                QueryStringEncoder.WriteSafeKey(TEMPLATE_VERSION_QUERY_PARAM, templateVersion.Value, ref chars);
            }

            foreach (var (key, value) in templateValues.OrderBy(x => x.Key))
            {
                if (value is not null)
                {
                    QueryStringEncoder.Encode(key, value.ToJsonString(), ref chars);
                }
            }

            var token = HmacToken.CreateToken(chars.LimitedSpan, _apiKey);
            var format_string = string.Empty;
            if (format != RenderImageFormat.PNG)
            {
                format_string = $"/{format.RenderFormatToExtensionWithoutDot()}";
            }

            var url = $"{CREATE_PATH}/{templateId}/{token}{format_string}?{chars.LimitedSpan}";

            return url;
        }
        finally
        {
            chars.Dispose();
        }

    }

    internal static void CreateAndRenderUrlQueryString(CreateUrlImageRequest request, ref ArrayOrSpan<char> chars)
    {
        QueryStringEncoder.EncodeSafeKey("url", request.Url, ref chars);

        if (request.FullScreen == true)
        {
            QueryStringEncoder.EncodeSafeKeyValue("full_screen", "true", ref chars);
        }

        if (request.BlockConsentBanners == true)
        {
            QueryStringEncoder.EncodeSafeKeyValue("block_consent_banners", "true", ref chars);
        }

        if (request.DisableTwemoji == true)
        {
            QueryStringEncoder.EncodeSafeKeyValue("disable_twemoji", "true", ref chars);
        }

        if (request.MaxRenderOnce == true)
        {
            QueryStringEncoder.EncodeSafeKeyValue("max_render_once", "true", ref chars);
        }

        if (request.RenderWhenReady == true)
        {
            QueryStringEncoder.EncodeSafeKeyValue("render_when_ready", "true", ref chars);
        }

        if (request.ColorScheme != null)
        {
            QueryStringEncoder.EncodeSafeKeyValue("color_scheme", Helpers.EnumToString.ColorSchemeString(request.ColorScheme.Value), ref chars);
        }

        if (request.DeviceScale != null)
        {
            QueryStringEncoder.WriteSafeKey("device_scale", request.DeviceScale.Value, ref chars);
        }

        if (request.MaxWaitMs != null)
        {
            QueryStringEncoder.WriteSafeKey("max_wait_ms", request.MaxWaitMs.Value, ref chars);
        }

        if (request.MsDelay != null)
        {
            QueryStringEncoder.WriteSafeKey("ms_delay", request.MsDelay.Value, ref chars);
        }

        if (request.ViewportHeight != null)
        {
            QueryStringEncoder.WriteSafeKey("viewport_height", request.ViewportHeight.Value, ref chars);
        }

        if (request.ViewportWidth != null)
        {
            QueryStringEncoder.WriteSafeKey("viewport_width", request.ViewportWidth.Value, ref chars);
        }

        if (!string.IsNullOrWhiteSpace(request.Css))
        {
            QueryStringEncoder.EncodeSafeKey("css", request.Css, ref chars);
        }

        if (!string.IsNullOrWhiteSpace(request.Selector))
        {
            QueryStringEncoder.EncodeSafeKey("selector", request.Selector, ref chars);
        }

        if (!string.IsNullOrWhiteSpace(request.Timezone))
        {
            QueryStringEncoder.EncodeSafeKey("timezone", request.Timezone, ref chars);
        }
    }

    /// <inheritdoc />
    public string CreateAndRenderUrl(CreateUrlImageRequest request, RenderImageFormat format = RenderImageFormat.PNG)
    {

        ArrayOrSpan<char> chars = new(stackalloc char[512]);
        try
        {
            CreateAndRenderUrlQueryString(request, ref chars);
            var token = HmacToken.CreateToken(chars.LimitedSpan, _apiKey);

            var format_string = string.Empty;
            if (format != RenderImageFormat.PNG)
            {
                format_string = $"/{format.RenderFormatToExtensionWithoutDot()}";
            }

            var url = $"{CREATE_AND_RENDER_PATH}/{_apiId}/{token}{format_string}?{chars.LimitedSpan}";

            return url;
        }
        finally
        {
            chars.Dispose();
        }

    }
}