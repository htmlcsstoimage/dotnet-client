using System.Numerics;
using HtmlCssToImage.Helpers;
using HtmlCssToImage.Models;
using HtmlCssToImage.Models.Requests;

namespace HtmlCssToImage;

public partial class HtmlCssToImageClient
{
    internal static void CreateAndRenderUrlQueryString(CreateUrlImageRequest request, ref QueryStringBuilder builder)
    {
        builder.EncodeSafeKey("url", request.Url);

        AppendBoolIfTrue("full_screen", request.FullScreen, ref builder);
        AppendBoolIfTrue("block_consent_banners", request.BlockConsentBanners, ref builder);
        AppendBoolIfTrue("disable_twemoji", request.DisableTwemoji, ref builder);
        AppendBoolIfTrue("max_render_once", request.MaxRenderOnce, ref builder);
        AppendBoolIfTrue("render_when_ready", request.RenderWhenReady, ref builder);

        if (request.ColorScheme != null)
        {
            builder.EncodeSafeKeyValue("color_scheme", request.ColorScheme.Value.ColorSchemeString());
        }

        AppendNumberIfNotNull("device_scale", request.DeviceScale, ref builder);
        AppendNumberIfNotNull("max_wait_ms", request.MaxWaitMs, ref builder);
        AppendNumberIfNotNull("ms_delay", request.MsDelay, ref builder);
        AppendNumberIfNotNull("viewport_height", request.ViewportHeight, ref builder);
        AppendNumberIfNotNull("viewport_width", request.ViewportWidth, ref builder);

        if (!string.IsNullOrWhiteSpace(request.Css))
        {
            builder.EncodeSafeKey("css", request.Css);
        }

        if (!string.IsNullOrWhiteSpace(request.Selector))
        {
            builder.EncodeSafeKey("selector", request.Selector);
        }

        if (!string.IsNullOrWhiteSpace(request.Timezone))
        {
            builder.EncodeSafeKey("timezone", request.Timezone);
        }

        AppendBoolIfTrue("viewport_mobile", request.ViewportMobile, ref builder);
        AppendBoolIfTrue("viewport_landscape", request.ViewportLandscape, ref builder);
        AppendBoolIfTrue("viewport_touch", request.ViewportTouch, ref builder);
        if (request.MediaType != null)
        {
            builder.EncodeSafeKeyValue("media_type", request.MediaType.Value.MediaTypeString());
        }

        if (!string.IsNullOrWhiteSpace(request.ProxyId))
        {
            builder.EncodeSafeKey("proxy_id", request.ProxyId);
        }

        AppendNumberIfNotNull("jumbo_max_width", request.JumboMaxWidth, ref builder);
        AppendNumberIfNotNull("jumbo_max_height", request.JumboMaxHeight, ref builder);
    }

    private static void AppendNumberIfNotNull<T>(ReadOnlySpan<char> key, T? value, ref QueryStringBuilder builder) where T : struct, INumber<T>
    {
        if (value != null)
        {
            builder.WriteSafeKey(key, value.Value);
        }
    }

    private static void AppendBoolIfTrue(ReadOnlySpan<char> key, bool? value, ref QueryStringBuilder builder)
    {
        if (value == true)
        {
            builder.EncodeSafeKeyValue(key, "true");
        }
    }

    /// <inheritdoc />
    public string CreateAndRenderUrl(CreateUrlImageRequest request, RenderImageFormat format = RenderImageFormat.PNG)
    {
        var pathFormat = format == RenderImageFormat.PNG ? null : (RenderImageFormat?)format;
        return CreateAndRenderUrl(request, pathFormat, null);
    }

    /// <inheritdoc />
    public string CreateAndRenderUrl(CreateUrlImageRequest request, RenderImageOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return CreateAndRenderUrl(request, options.Format, options);
    }

    private string CreateAndRenderUrl(
        CreateUrlImageRequest request,
        RenderImageFormat? pathFormat,
        RenderImageOptions? options)
    {
        QueryStringBuilder builder = new(stackalloc char[512]);
        try
        {
            builder.AppendLiteral(_apiHost);
            builder.AppendLiteral(CREATE_AND_RENDER_PATH);
            builder.AppendLiteral('/');
            builder.AppendLiteral(_apiId);
            builder.AppendLiteral('/');
            var tokenPosition = builder.ReserveLiteral(HmacToken.TokenLength);

            if (pathFormat.HasValue)
            {
                builder.AppendLiteral('/');
                builder.AppendLiteral(pathFormat.Value.RenderFormatToExtensionWithoutDot());
            }

            CreateAndRenderUrlQueryString(request, ref builder);
            options?.AppendToBuilder(ref builder, includeFormat: false);

            var queryString = builder.QueryString(false);
            HmacToken.WriteToken(
                queryString,
                _apiKey,
                builder.ReservedLiteral(tokenPosition, HmacToken.TokenLength));

            return builder.FullSpan.ToString();
        }
        finally
        {
            builder.Dispose();
        }
    }

    /// <inheritdoc />
    public string ImageUrl(string imageId, RenderImageOptions options) =>
        RenderImageOptions.ToUrl(_apiHost, imageId, options);
}
