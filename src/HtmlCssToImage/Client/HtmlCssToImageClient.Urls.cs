using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
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

            var url = $"{_apiHost}{CREATE_PATH}/{templateId}/{token}{format_string}?{chars.LimitedSpan}";

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

        AppendBoolIfTrue("full_screen",request.FullScreen, ref chars);
        AppendBoolIfTrue("block_consent_banners",request.BlockConsentBanners, ref chars);
        AppendBoolIfTrue("disable_twemoji",request.DisableTwemoji, ref chars);
        AppendBoolIfTrue("max_render_once",request.MaxRenderOnce, ref chars);
        AppendBoolIfTrue("render_when_ready",request.RenderWhenReady, ref chars);

        if (request.ColorScheme != null)
        {
            QueryStringEncoder.EncodeSafeKeyValue("color_scheme", request.ColorScheme.Value.ColorSchemeString(), ref chars);
        }
        AppendNumberIfNotNull("device_scale", request.DeviceScale, ref chars);
        AppendNumberIfNotNull("max_wait_ms", request.MaxWaitMs, ref chars);
        AppendNumberIfNotNull("ms_delay", request.MsDelay, ref chars);
        AppendNumberIfNotNull("viewport_height", request.ViewportHeight, ref chars);
        AppendNumberIfNotNull("viewport_width", request.ViewportWidth, ref chars);

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
        AppendBoolIfTrue("viewport_mobile", request.ViewportMobile, ref chars);
        AppendBoolIfTrue("viewport_landscape", request.ViewportLandscape, ref chars);
        AppendBoolIfTrue("viewport_touch", request.ViewportTouch, ref chars);
        if (request.MediaType != null)
        {
            QueryStringEncoder.EncodeSafeKeyValue("media_type", request.MediaType.Value.MediaTypeString(), ref chars);
        }

        if (!string.IsNullOrWhiteSpace(request.ProxyId))
        {
            QueryStringEncoder.EncodeSafeKey("proxy_id", request.ProxyId, ref chars);
        }

        AppendNumberIfNotNull("jumbo_max_width", request.JumboMaxWidth, ref chars);
        AppendNumberIfNotNull("jumbo_max_height", request.JumboMaxHeight, ref chars);

    }

    private static void AppendNumberIfNotNull<T>(ReadOnlySpan<char> key, T? value, ref ArrayOrSpan<char> chars) where T : struct, INumber<T>
    {
        if (value != null)
        {
            QueryStringEncoder.WriteSafeKey(key, value.Value, ref chars);
        }
    }

    private static void AppendBoolIfTrue(ReadOnlySpan<char> key, bool? value, ref ArrayOrSpan<char> chars)
    {
        if (value == true)
        {
            QueryStringEncoder.EncodeSafeKeyValue(key, "true", ref chars);
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

            var url = $"{_apiHost}{CREATE_AND_RENDER_PATH}/{_apiId}/{token}{format_string}?{chars.LimitedSpan}";

            return url;
        }
        finally
        {
            chars.Dispose();
        }

    }
}