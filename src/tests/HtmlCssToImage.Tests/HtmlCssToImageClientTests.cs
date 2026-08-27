using System.Net;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization.Metadata;
using System.Web;
using HtmlCssToImage.Models;
using HtmlCssToImage.Models.Requests;
using Moq;
using Moq.Protected;

namespace HtmlCssToImage.Tests;

public class HtmlCssToImageClientTests
{
    private readonly HtmlCssToImageOptions _options;
    private readonly Mock<HttpMessageHandler> _handlerMock;
    public HtmlCssToImageClientTests()
    {
        _options = new HtmlCssToImageOptions
        {
            ApiKey = "test_key",
            ApiId = "test_id"
        };
        _handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
    }


    private const string DefaultHost = "https://hcti.io";

    private HtmlCssToImageClient CreateClient()
    {
        var httpClient = new HttpClient(_handlerMock.Object);
        return new HtmlCssToImageClient(httpClient, _options);
    }

    private HtmlCssToImageClient CreateClient(string apiHost)
    {
        var httpClient = new HttpClient(_handlerMock.Object);
        return new HtmlCssToImageClient(httpClient, _options, apiHost);
    }

    public enum QueryStringType
    {
        None,
        Numbers,
        Emoji,
        Long,
        Space,
        NonUtf8
    }

    [Theory]
    [InlineData(QueryStringType.None,null)]
    [InlineData(QueryStringType.None,RenderImageFormat.PNG)]
    [InlineData(QueryStringType.None,RenderImageFormat.JPG)]
    [InlineData(QueryStringType.None,RenderImageFormat.WEBP)]
    [InlineData(QueryStringType.None,RenderImageFormat.PDF)]
    [InlineData(QueryStringType.Numbers,null)]
    [InlineData(QueryStringType.Numbers,RenderImageFormat.WEBP)]
    [InlineData(QueryStringType.Emoji,null)]
    [InlineData(QueryStringType.NonUtf8,null)]
    [InlineData(QueryStringType.Long,null)]
    [InlineData(QueryStringType.Space,null)]
    public void CreateUrl_GeneratesValidSignedUrl(QueryStringType type, RenderImageFormat? format  )
    {
        var qs = type switch
        {
            QueryStringType.None => "",
            QueryStringType.Numbers => "?abc=123",
            QueryStringType.Emoji => "?abc=👀",
            QueryStringType.NonUtf8=>$"?abc={new string('漢',100)}",
            QueryStringType.Long => $"?abc={new string('a', 600)}",
            QueryStringType.Space => "?abc=a b",
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
        var client = CreateClient();
        var request = new CreateUrlImageRequest { Url = $"https://google.com{qs}" };
        var result = client.CreateAndRenderUrl(request, format?? RenderImageFormat.PNG);
        var uri = new Uri(result);
        Assert.StartsWith("https://hcti.io/v1/image/create-and-render/test_id/", result);
        Assert.Equal($"?url={Uri.EscapeDataString(request.Url)}", uri.Query);

        var parts = uri.AbsolutePath.Split('/');
        var token_part = parts[^1];
        if (format != null && format != RenderImageFormat.PNG)
        {
            token_part = parts[^2];
            Assert.Equal(parts.Last(), format.ToString()!.ToLower());
        }
        Assert.Equal(HexLowerHmac(_options.ApiKey, uri.Query[1..]), token_part);
    }

    [Fact]
    public void CreateAndRenderUrl_WithRenderOptions_AppendsFormatAndOutputOptions()
    {
        var client = CreateClient();
        var request = new CreateUrlImageRequest { Url = "https://google.com" };
        var options = new RenderImageOptions
        {
            Format = RenderImageFormat.PNG,
            Dpi = 144,
            Height = 400,
            Width = 600
        };

        var result = client.CreateAndRenderUrl(request, options);
        var uri = new Uri(result);

        Assert.EndsWith("/png", uri.AbsolutePath);
        Assert.Equal(
            "?url=https%3A%2F%2Fgoogle.com&dpi=144&height=400&width=600",
            uri.Query);
        Assert.Equal(
            HexLowerHmac(_options.ApiKey, uri.Query[1..]),
            uri.AbsolutePath.Split('/')[^2]);
    }

    [Fact]
    public void CreateAndRenderUrl_UsesRequestFormatWhenNoOverrideIsProvided()
    {
        var client = CreateClient();
        var request = new CreateUrlImageRequest
        {
            Url = "https://google.com",
            Format = RenderImageFormat.WEBP
        };

        var uri = new Uri(client.CreateAndRenderUrl(request));

        Assert.EndsWith("/webp", uri.AbsolutePath);
    }

    [Fact]
    public void CreateAndRenderUrl_ExplicitFormatOverridesRequestFormat()
    {
        var client = CreateClient();
        var request = new CreateUrlImageRequest
        {
            Url = "https://google.com",
            Format = RenderImageFormat.WEBP
        };

        var uri = new Uri(client.CreateAndRenderUrl(request, RenderImageFormat.PNG));

        Assert.DoesNotContain("/webp", uri.AbsolutePath);
        Assert.Equal(
            HexLowerHmac(_options.ApiKey, uri.Query[1..]),
            uri.AbsolutePath.Split('/')[^1]);
    }

    [Fact]
    public void ImageCreationRequests_SerializeFormat()
    {
        var htmlRequest = new CreateHtmlCssImageRequest
        {
            Html = "<b>Test</b>",
            Format = RenderImageFormat.WEBP
        };
        var templatedRequest = new CreateTemplatedImageRequest
        {
            TemplateId = "tpl_123",
            TemplateValues = new JsonObject { ["title"] = "Hello" },
            Format = RenderImageFormat.PDF
        };
        var batchRequest = new CreateImageBatchRequest<CreateHtmlCssImageRequest>
        {
            DefaultOptions = new CreateHtmlCssImageRequest
            {
                Html = "<b>Default</b>",
                Format = RenderImageFormat.PNG
            }
        };
        batchRequest.Variations.Add(htmlRequest);

        using var htmlJson = JsonDocument.Parse(
            JsonSerializer.Serialize(htmlRequest, JsonContext.Default.CreateHtmlCssImageRequest));
        using var templatedJson = JsonDocument.Parse(
            JsonSerializer.Serialize(templatedRequest, JsonContext.Default.CreateTemplatedImageRequest));
        using var batchJson = JsonDocument.Parse(
            JsonSerializer.Serialize(
                batchRequest,
                JsonContext.Default.CreateImageBatchRequestCreateHtmlCssImageRequest));

        Assert.Equal("webp", htmlJson.RootElement.GetProperty("format").GetString()!.ToLowerInvariant());
        Assert.Equal("pdf", templatedJson.RootElement.GetProperty("format").GetString()!.ToLowerInvariant());
        Assert.Equal(
            "png",
            batchJson.RootElement.GetProperty("default_options").GetProperty("format").GetString()!.ToLowerInvariant());
        Assert.Equal(
            "webp",
            batchJson.RootElement.GetProperty("variations")[0].GetProperty("format").GetString()!.ToLowerInvariant());
    }

    [Theory]
    [InlineData(true, "true")]
    [InlineData(false, "false")]
    [InlineData(null, null)]
    public void CreateAndRenderUrl_PreservesTransparentBackground(
        bool? transparentBackground,
        string? expected)
    {
        var client = CreateClient();
        var request = new CreateUrlImageRequest
        {
            Url = "https://google.com",
            TransparentBackground = transparentBackground
        };

        var result = client.CreateAndRenderUrl(request);
        var query = HttpUtility.ParseQueryString(new Uri(result).Query);

        Assert.Equal(expected, query["transparent_background"]);
    }

    [Fact]
    public void CreateAndRenderUrl_IncludesStorageDestinationId()
    {
        var client = CreateClient();
        var request = new CreateUrlImageRequest
        {
            Url = "https://google.com",
            StorageDestinationId = "storage-destination-id"
        };

        var result = client.CreateAndRenderUrl(request);
        var query = HttpUtility.ParseQueryString(new Uri(result).Query);

        Assert.Equal("storage-destination-id", query["storage_destination_id"]);
    }

    [Fact]
    public void CreateAndRenderUrl_IncludesRepeatedHeadersAndHeaderOptions()
    {
        var client = CreateClient();
        var request = new CreateUrlImageRequest
        {
            Url = "https://example.com/private",
            Headers = new Dictionary<string, string>
            {
                ["Authorization"] = "Bearer test:value",
                ["X-Preview-Mode"] = "enabled"
            },
            AdditionalHeaderOrigins =
            [
                "https://api.example.com",
                "https://assets.example.com:8443"
            ],
            IncludeHeadersOnSubrequests = true,
            IdentifyAsHcti = true
        };

        var result = client.CreateAndRenderUrl(request);
        var uri = new Uri(result);
        var query = HttpUtility.ParseQueryString(uri.Query);
        var headerValues = query.GetValues("headers");
        var additionalHeaderOrigins = query.GetValues("additional_header_origins");

        Assert.NotNull(headerValues);
        Assert.Equal(
            ["Authorization:Bearer test:value", "X-Preview-Mode:enabled"],
            headerValues);
        Assert.NotNull(additionalHeaderOrigins);
        Assert.Equal(
            ["https://api.example.com", "https://assets.example.com:8443"],
            additionalHeaderOrigins);
        Assert.Equal("true", query["include_headers_on_subrequests"]);
        Assert.Equal("true", query["identify_as_hcti"]);
        Assert.Equal(
            HexLowerHmac(_options.ApiKey, uri.Query[1..]),
            uri.AbsolutePath.Split('/')[^1]);
    }

    [Fact]
    public void CreateTemplatedImageUrl_WithVersion_IncludesTemplateVersionInQuery()
    {
        var client = CreateClient();
        var values = new JsonObject { ["title"] = "Hello" };

        var url = client.CreateTemplatedImageUrl("tpl_123", values, templateVersion: 5);
        var uri = new Uri(url);

        Assert.Equal("?template_version=5&title=%22Hello%22", uri.Query);
        Assert.Equal(
            HexLowerHmac(_options.ApiKey, uri.Query[1..]),
            uri.AbsolutePath.Split('/')[^1]);
    }

    [Fact]
    public void CreateTemplatedImageUrl_WithoutValues_PreservesEmptyQuery()
    {
        var client = CreateClient();

        var url = client.CreateTemplatedImageUrl("tpl_123", new JsonObject());
        var token = HexLowerHmac(_options.ApiKey, string.Empty);

        Assert.Equal($"https://hcti.io/v1/image/tpl_123/{token}?", url);
    }

    [Fact]
    public void CreateTemplatedImageUrl_WithRenderOptions_UsesFallbackKeysForTemplateCollisions()
    {
        var client = CreateClient();
        var values = new JsonObject
        {
            ["crop_width"] = "template crop width",
            ["dpi"] = "template dpi",
            ["height"] = "template height",
            ["width"] = "template width",
            ["x_1"] = "template x"
        };
        var options = new RenderImageOptions
        {
            Format = RenderImageFormat.WEBP,
            Dpi = 144,
            Height = 400,
            Width = 600,
            Crop = RenderImageCrop.Rectangle(
                horizontal: RenderImageCropSpan.SizedFrom(
                    RenderImageCropPosition.Pixels(10),
                    RenderImageCropSize.Pixels(200)))
        };

        var result = client.CreateTemplatedImageUrl(
            "tpl_123",
            values,
            templateVersion: null,
            options: options);
        var uri = new Uri(result);
        var query = HttpUtility.ParseQueryString(uri.Query);

        Assert.EndsWith("/webp", uri.AbsolutePath);
        Assert.Equal("template crop width", ParseTemplateValue<string>(query["crop_width"]));
        Assert.Equal("template dpi", ParseTemplateValue<string>(query["dpi"]));
        Assert.Equal("template height", ParseTemplateValue<string>(query["height"]));
        Assert.Equal("template width", ParseTemplateValue<string>(query["width"]));
        Assert.Equal("template x", ParseTemplateValue<string>(query["x_1"]));
        Assert.Equal("144", query["__ro_dpi"]);
        Assert.Equal("400", query["__ro_height"]);
        Assert.Equal("600", query["__ro_width"]);
        Assert.Equal("10px", query["__ro_x_1"]);
        Assert.Equal("200px", query["__ro_crop_width"]);
        Assert.Equal(
            HexLowerHmac(_options.ApiKey, uri.Query[1..]),
            uri.AbsolutePath.Split('/')[^2]);
    }

    [Fact]
    public void CreateTemplatedImageUrl_GenericRenderOptionsOverloadsSerializeValuesAndUseFallbacks()
    {
        IHtmlCssToImageClient client = CreateClient();
        var values = new Dictionary<string, string>
        {
            ["height"] = "template height",
            ["title"] = "Hello"
        };
        var options = new RenderImageOptions
        {
            Format = RenderImageFormat.JPG,
            Height = 400
        };
        var serializerOptions = new JsonSerializerOptions
        {
            TypeInfoResolver = new DefaultJsonTypeInfoResolver()
        };
        var typeInfo =
            (JsonTypeInfo<Dictionary<string, string>>)serializerOptions.GetTypeInfo(
                typeof(Dictionary<string, string>));

        var urls = new[]
        {
            client.CreateTemplatedImageUrl(
                "tpl_123",
                values,
                templateVersion: null,
                options),
            client.CreateTemplatedImageUrl(
                "tpl_123",
                values,
                typeInfo,
                templateVersion: null,
                options),
            client.CreateTemplatedImageUrl(
                "tpl_123",
                values,
                serializerOptions,
                templateVersion: null,
                options)
        };

        foreach (var url in urls)
        {
            var uri = new Uri(url);
            var query = HttpUtility.ParseQueryString(uri.Query);

            Assert.EndsWith("/jpg", uri.AbsolutePath);
            Assert.Equal("template height", ParseTemplateValue<string>(query["height"]));
            Assert.Equal("Hello", ParseTemplateValue<string>(query["title"]));
            Assert.Equal("400", query["__ro_height"]);
            Assert.Equal(
                HexLowerHmac(_options.ApiKey, uri.Query[1..]),
                uri.AbsolutePath.Split('/')[^2]);
        }
    }

    [Theory]
    [InlineData("crop_w", "__ro_crop_w", true)]
    [InlineData("crop_h", "__ro_crop_h", false)]
    public void CreateTemplatedImageUrl_WithCropAliasCollision_PreservesTemplateValue(
        string templateKey,
        string fallbackKey,
        bool horizontal)
    {
        var client = CreateClient();
        var values = new JsonObject { [templateKey] = "template crop" };
        var cropSpan = RenderImageCropSpan.Sized(RenderImageCropSize.Percent(25));
        var options = new RenderImageOptions
        {
            Crop = horizontal
                ? RenderImageCrop.Rectangle(horizontal: cropSpan)
                : RenderImageCrop.Rectangle(vertical: cropSpan)
        };

        var result = client.CreateTemplatedImageUrl(
            "tpl_123",
            values,
            templateVersion: null,
            options);
        var query = HttpUtility.ParseQueryString(new Uri(result).Query);

        Assert.Equal("template crop", ParseTemplateValue<string>(query[templateKey]));
        Assert.Equal("25%", query[fallbackKey]);
    }

    [Fact]
    public void CreateTemplatedImageUrl_WithBothCropWidthAliases_PreservesBothTemplateValues()
    {
        var client = CreateClient();
        var values = new JsonObject
        {
            ["crop_width"] = "canonical template value",
            ["crop_w"] = "alias template value"
        };
        var options = new RenderImageOptions
        {
            Crop = RenderImageCrop.Rectangle(
                horizontal: RenderImageCropSpan.Sized(RenderImageCropSize.Pixels(200)))
        };

        var result = client.CreateTemplatedImageUrl(
            "tpl_123",
            values,
            templateVersion: null,
            options);
        var query = HttpUtility.ParseQueryString(new Uri(result).Query);

        Assert.Equal(
            "canonical template value",
            ParseTemplateValue<string>(query["crop_width"]));
        Assert.Equal(
            "alias template value",
            ParseTemplateValue<string>(query["crop_w"]));
        Assert.Equal("200px", query["__ro_crop_width"]);
        Assert.Equal("200px", query["__ro_crop_w"]);
    }

    [Fact]
    public async Task CreateImageAsync_WhenSuccess_ReturnsResponseData()
    {
        var client = CreateClient();
        var request = new CreateHtmlCssImageRequest { Html = "<b>Test</b>" };

        _handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = JsonContent.Create(new { url = "https://hcti.io/v1/image/img_1", id = "img_1" })
            });

        using var result = await client.CreateImageAsync(request, TestContext.Current.CancellationToken);

        Assert.True(result.Success);
        Assert.NotNull(result.Response);
        Assert.Equal("img_1", result.Response.Id);
    }

    [Fact]
    public async Task CreateImageAsync_SerializesUrlHeadersAndHeaderOptions()
    {
        HttpRequestMessage? capturedRequest = null;
        _handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .Callback<HttpRequestMessage, CancellationToken>((request, _) => capturedRequest = request)
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = JsonContent.Create(new { url = "https://hcti.io/v1/image/img_1", id = "img_1" })
            });

        var client = CreateClient();
        var request = new CreateUrlImageRequest
        {
            Url = "https://example.com/private",
            Headers = new Dictionary<string, string>
            {
                ["Cookie"] = "session=test",
                ["Authorization"] = "Bearer test"
            },
            AdditionalHeaderOrigins =
            [
                "https://api.example.com",
                "https://assets.example.com:8443"
            ],
            IncludeHeadersOnSubrequests = true,
            IdentifyAsHcti = true
        };

        using var result = await client.CreateImageAsync(request, TestContext.Current.CancellationToken);

        Assert.NotNull(capturedRequest);
        var json = await capturedRequest.Content!.ReadAsStringAsync(TestContext.Current.CancellationToken);
        using var document = JsonDocument.Parse(json);
        var root = document.RootElement;
        var headers = root.GetProperty("headers");

        Assert.Equal("session=test", headers.GetProperty("Cookie").GetString());
        Assert.Equal("Bearer test", headers.GetProperty("Authorization").GetString());
        var additionalHeaderOrigins = root.GetProperty("additional_header_origins");
        Assert.Equal(2, additionalHeaderOrigins.GetArrayLength());
        Assert.Equal("https://api.example.com", additionalHeaderOrigins[0].GetString());
        Assert.Equal("https://assets.example.com:8443", additionalHeaderOrigins[1].GetString());
        Assert.True(root.GetProperty("include_headers_on_subrequests").GetBoolean());
        Assert.True(root.GetProperty("identify_as_hcti").GetBoolean());
    }

    [Fact]
    public async Task DeleteImageAsync_WhenAccepted_SendsDeleteAndReturnsTrue()
    {
        HttpRequestMessage? capturedRequest = null;
        _handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .Callback<HttpRequestMessage, CancellationToken>((request, _) => capturedRequest = request)
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.Accepted
            });

        var client = CreateClient();
        using var result = await client.DeleteImageAsync("image/id", TestContext.Current.CancellationToken);

        Assert.NotNull(capturedRequest);
        Assert.Equal(HttpMethod.Delete, capturedRequest.Method);
        Assert.Equal(new Uri("https://hcti.io/v1/image/image%2Fid"), capturedRequest.RequestUri);
        Assert.True(result.Success);
        Assert.Equal(202, result.StatusCode);
        Assert.True(result.Response);
    }

    [Fact]
    public async Task DeleteImageAsync_WhenError_PopulatesErrorDetails()
    {
        _handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NotFound,
                Content = JsonContent.Create(new { message = "Image not found", error = "not_found" })
            });

        var client = CreateClient();
        using var result = await client.DeleteImageAsync("missing-image", TestContext.Current.CancellationToken);

        Assert.False(result.Success);
        Assert.Equal(404, result.StatusCode);
        Assert.Null(result.Response);
        Assert.NotNull(result.ErrorDetails);
        Assert.Equal("Image not found", result.ErrorDetails.Message);
        Assert.Equal("not_found", result.ErrorDetails.ErrorType);
    }

    [Fact]
    public async Task DeleteImageBatchAsync_SendsIdsInDeleteRequest()
    {
        HttpRequestMessage? capturedRequest = null;
        _handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .Callback<HttpRequestMessage, CancellationToken>((request, _) => capturedRequest = request)
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.Accepted
            });

        var client = CreateClient();
        using var result = await client.DeleteImageBatchAsync(
            ["image-1", "image-2"],
            TestContext.Current.CancellationToken);

        Assert.NotNull(capturedRequest);
        Assert.Equal(HttpMethod.Delete, capturedRequest.Method);
        Assert.Equal(new Uri("https://hcti.io/v1/image/batch"), capturedRequest.RequestUri);
        var request = await capturedRequest.Content!.ReadFromJsonAsync<DeleteImageBatchRequest>(
            JsonContext.Default.DeleteImageBatchRequest,
            TestContext.Current.CancellationToken);
        Assert.NotNull(request);
        Assert.Equal(["image-1", "image-2"], request.Ids);
        Assert.True(result.Success);
        Assert.Equal(202, result.StatusCode);
        Assert.True(result.Response);
    }

    [Fact]
    public async Task CreateImageAsync_UsesDefaultHost()
    {
        HttpRequestMessage? capturedRequest = null;
        _handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .Callback<HttpRequestMessage, CancellationToken>((request, _) => capturedRequest = request)
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = JsonContent.Create(new { url = "https://hcti.io/v1/image/img_1", id = "img_1" })
            });

        var client = CreateClient(DefaultHost);
        var request = new CreateHtmlCssImageRequest { Html = "<b>Test</b>" };

        using var result = await client.CreateImageAsync(request, TestContext.Current.CancellationToken);

        Assert.NotNull(capturedRequest);
        Assert.Equal(new Uri("https://hcti.io/v1/image?includeId=true"), capturedRequest.RequestUri);
    }

    [Fact]
    public async Task CreateImageAsync_UsesConfiguredHost()
    {
        const string configuredBaseUrl = "https://example.test";
        HttpRequestMessage? capturedRequest = null;
        _handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .Callback<HttpRequestMessage, CancellationToken>((request, _) => capturedRequest = request)
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = JsonContent.Create(new { url = "https://hcti.io/v1/image/img_1", id = "img_1" })
            });

        var client = CreateClient(configuredBaseUrl);
        var request = new CreateHtmlCssImageRequest { Html = "<b>Test</b>" };

        using var result = await client.CreateImageAsync(request, TestContext.Current.CancellationToken);

        Assert.NotNull(capturedRequest);
        Assert.Equal(new Uri("https://example.test/v1/image?includeId=true"), capturedRequest.RequestUri);
    }

    [Fact]
    public void CreateAndRenderUrl_UsesConfiguredHost()
    {
        var client = CreateClient("https://example.test");
        var request = new CreateUrlImageRequest { Url = "https://google.com" };

        var result = client.CreateAndRenderUrl(request);

        Assert.StartsWith("https://example.test/v1/image/create-and-render/test_id/", result);
    }

    [Fact]
    public void CreateAndRenderUrl_DoesNotIncludePostOnlyDedupeDuration()
    {
        var client = CreateClient();
        var request = new CreateUrlImageRequest { Url = "https://google.com", DedupeDurationS = 3600 };

        var result = client.CreateAndRenderUrl(request);
        var query = HttpUtility.ParseQueryString(new Uri(result).Query);

        Assert.Null(query["dedupe_duration_s"]);
    }

    [Fact]
    public async Task CreateImageBatchAsync_WhenError_PopulatesErrorDetails()
    {
        var client = CreateClient();
        var request = new CreateImageBatchRequest<CreateHtmlCssImageRequest>();

        _handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest,
                Content = JsonContent.Create(new { message = "Invalid request", error = "missing_params" })
            });

        using var result = await client.CreateImageBatchAsync(request, TestContext.Current.CancellationToken);

        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.NotNull(result.ErrorDetails);
        Assert.Equal("Invalid request", result.ErrorDetails.Message);
    }



    private static string HexLowerHmac(string key, string value)
    {
        return Convert.ToHexStringLower(
            HMACSHA256.HashData(
                Encoding.UTF8.GetBytes(key),
                Encoding.UTF8.GetBytes(value)));
    }

    private static T ParseTemplateValue<T>(string? json)
    {
        Assert.NotNull(json);
        var value = JsonNode.Parse(json);
        Assert.NotNull(value);
        return value.GetValue<T>();
    }
}
