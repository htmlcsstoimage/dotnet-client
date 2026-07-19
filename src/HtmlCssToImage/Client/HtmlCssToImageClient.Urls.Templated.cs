using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization.Metadata;
using HtmlCssToImage.Helpers;
using HtmlCssToImage.Models;

namespace HtmlCssToImage;

public partial class HtmlCssToImageClient
{
    /// <inheritdoc />
    public string CreateTemplatedImageUrl<T>(string templateId, T templateValues,
        JsonTypeInfo<T> typeInfo, long? templateVersion = null, RenderImageFormat format = RenderImageFormat.PNG) =>
        CreateTemplatedImageUrl(
            templateId,
            SerializeTemplateValues(templateValues, typeInfo),
            templateVersion,
            format);

    /// <inheritdoc />
    public string CreateTemplatedImageUrl<T>(
        string templateId,
        T templateValues,
        JsonTypeInfo<T> typeInfo,
        long? templateVersion,
        RenderImageOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return CreateTemplatedImageUrl(
            templateId,
            SerializeTemplateValues(templateValues, typeInfo),
            templateVersion,
            options);
    }

    /// <inheritdoc />
    [RequiresUnreferencedCode("If AOT is needed, use one of the overloads with explicit type information")]
    [RequiresDynamicCode("If AOT is needed, use one of the overloads with explicit type information")]
    public string CreateTemplatedImageUrl<T>(string templateId, T templateValues, long? templateVersion = null, RenderImageFormat format = RenderImageFormat.PNG) =>
        CreateTemplatedImageUrl(
            templateId,
            SerializeTemplateValues(templateValues),
            templateVersion,
            format);

    /// <inheritdoc />
    [RequiresUnreferencedCode("If AOT is needed, use one of the overloads with explicit type information")]
    [RequiresDynamicCode("If AOT is needed, use one of the overloads with explicit type information")]
    public string CreateTemplatedImageUrl<T>(
        string templateId,
        T templateValues,
        long? templateVersion,
        RenderImageOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return CreateTemplatedImageUrl(
            templateId,
            SerializeTemplateValues(templateValues),
            templateVersion,
            options);
    }

    /// <inheritdoc />
    public string CreateTemplatedImageUrl<T>(string templateId, T templateValues,
        JsonSerializerOptions jsonSerializerOptions, long? templateVersion = null, RenderImageFormat format = RenderImageFormat.PNG) =>
        CreateTemplatedImageUrl(
            templateId,
            SerializeTemplateValues(templateValues, jsonSerializerOptions),
            templateVersion,
            format);

    /// <inheritdoc />
    public string CreateTemplatedImageUrl<T>(
        string templateId,
        T templateValues,
        JsonSerializerOptions jsonSerializerOptions,
        long? templateVersion,
        RenderImageOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return CreateTemplatedImageUrl(
            templateId,
            SerializeTemplateValues(templateValues, jsonSerializerOptions),
            templateVersion,
            options);
    }

    [RequiresUnreferencedCode("If AOT is needed, use one of the overloads with explicit type information")]
    [RequiresDynamicCode("If AOT is needed, use one of the overloads with explicit type information")]
    private static JsonObject SerializeTemplateValues<T>(T templateValues)
    {
        return RequireJsonObject(JsonSerializer.SerializeToNode(templateValues));
    }

    private static JsonObject SerializeTemplateValues<T>(T templateValues, JsonTypeInfo<T> typeInfo)
    {
        ArgumentNullException.ThrowIfNull(typeInfo);
        return RequireJsonObject(JsonSerializer.SerializeToNode(templateValues, typeInfo));
    }

    private static JsonObject SerializeTemplateValues<T>(
        T templateValues,
        JsonSerializerOptions jsonSerializerOptions)
    {
        ArgumentNullException.ThrowIfNull(jsonSerializerOptions);
        return RequireJsonObject(
            JsonSerializer.SerializeToNode(
                templateValues,
                jsonSerializerOptions.GetTypeInfo(typeof(T))));
    }

    private static JsonObject RequireJsonObject(JsonNode? serializedValues)
    {
        if (serializedValues is not JsonObject jsonObject)
        {
            throw new ArgumentException("Template values must serialize to a JSON object.");
        }

        return jsonObject;
    }

    /// <inheritdoc />
    public string CreateTemplatedImageUrl(string templateId, JsonObject templateValues, long? templateVersion = null, RenderImageFormat format = RenderImageFormat.PNG)
    {
        var pathFormat = format == RenderImageFormat.PNG ? null : (RenderImageFormat?)format;
        return CreateTemplatedImageUrl(templateId, templateValues, templateVersion, pathFormat, null);
    }

    /// <inheritdoc />
    public string CreateTemplatedImageUrl(
        string templateId,
        JsonObject templateValues,
        long? templateVersion,
        RenderImageOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return CreateTemplatedImageUrl(
            templateId,
            templateValues,
            templateVersion,
            options.Format,
            options);
    }

    private string CreateTemplatedImageUrl(
        string templateId,
        JsonObject templateValues,
        long? templateVersion,
        RenderImageFormat? pathFormat,
        RenderImageOptions? options)
    {
        QueryStringBuilder builder = new(stackalloc char[512]);
        try
        {
            builder.AppendLiteral(_apiHost);
            builder.AppendLiteral(CREATE_OR_GET_PATH);
            builder.AppendLiteral('/');
            builder.AppendLiteral(templateId);
            builder.AppendLiteral('/');
            var tokenPosition = builder.ReserveLiteral(HmacToken.TokenLength);

            if (pathFormat.HasValue)
            {
                builder.AppendLiteral('/');
                builder.AppendLiteral(pathFormat.Value.RenderFormatToExtensionWithoutDot());
            }

            if (templateVersion.HasValue)
            {
                builder.WriteSafeKey(TEMPLATE_VERSION_QUERY_PARAM, templateVersion.Value);
            }

            foreach (var (key, value) in templateValues.OrderBy(x => x.Key))
            {
                if (value is not null)
                {
                    builder.Encode(key, value.ToJsonString());
                }
            }

            options?.AppendToBuilder(
                ref builder,
                includeFormat: false,
                templateValues: templateValues);

            var queryString = builder.QueryString(false);
            HmacToken.WriteToken(
                queryString,
                _apiKey,
                builder.ReservedLiteral(tokenPosition, HmacToken.TokenLength));

            if (queryString.IsEmpty)
            {
                builder.AppendLiteral('?');
            }

            return builder.FullSpan.ToString();
        }
        finally
        {
            builder.Dispose();
        }
    }
}
