using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using HtmlCssToImage.Models;
using HtmlCssToImage.Models.Recordings;
using HtmlCssToImage.Models.Requests;
using HtmlCssToImage.Models.Responses;
using HtmlCssToImage.Models.Results;
using Moq;
using Moq.Protected;

namespace HtmlCssToImage.Tests;

public class HtmlCssToImageClientRecordingsTests
{
    private readonly Mock<HttpMessageHandler> _handlerMock = new(MockBehavior.Strict);

    private IHtmlCssToImageClient CreateClient() => new HtmlCssToImageClient(
        new HttpClient(_handlerMock.Object), new HtmlCssToImageOptions("test_id", "test_key"), "https://example.test");

    private void Respond(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> response) =>
        _handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .Returns((HttpRequestMessage request, CancellationToken ct) => response(request, ct));

    private static HttpResponseMessage JsonResponse(HttpStatusCode status, string json) => new(status)
    {
        Content = new StringContent(json, Encoding.UTF8, "application/json")
    };

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task CreateRecordingAsync_DispatchesSourceTypeAndMapsResponse(bool fromUrl)
    {
        Respond(async (request, ct) =>
        {
            Assert.Equal(HttpMethod.Post, request.Method);
            Assert.Equal(new Uri("https://example.test/v1/recording"), request.RequestUri);
            var json = JsonNode.Parse(await request.Content!.ReadAsStringAsync(ct))!.AsObject();
            Assert.Equal(2000, json["duration_ms"]!.GetValue<int>());
            Assert.False(json.ContainsKey("$type"));
            Assert.False(json.ContainsKey("output"));
            Assert.False(json.ContainsKey("actions"));
            if (fromUrl)
            {
                Assert.Equal("https://example.com", json["url"]!.GetValue<string>());
                Assert.Equal("Bearer test", json["headers"]!["Authorization"]!.GetValue<string>());
                Assert.Equal("https://assets.example.com", json["additional_header_origins"]![0]!.GetValue<string>());
                Assert.True(json["include_headers_on_subrequests"]!.GetValue<bool>());
                Assert.False(json["identify_as_hcti"]!.GetValue<bool>());
                Assert.True(json["block_consent_banners"]!.GetValue<bool>());
                Assert.False(json.ContainsKey("html"));
            }
            else
            {
                Assert.Equal("<h1>Hello</h1>", json["html"]!.GetValue<string>());
                Assert.Equal("Roboto|Open+Sans", json["google_fonts"]!.GetValue<string>());
                Assert.False(json.ContainsKey("url"));
            }
            return JsonResponse(HttpStatusCode.OK, """
                {"id":"rec_1","status_url":"https://example.test/v1/recordings/rec_1/status","recording_url":"https://example.test/v1/recording/rec_1.mp4"}
                """);
        });
        ICreateRecordingRequestBase request = fromUrl
            ? new CreateUrlRecordingRequest
            {
                Url = "https://example.com", DurationMs = 2000,
                Headers = new() { ["Authorization"] = "Bearer test" },
                AdditionalHeaderOrigins = ["https://assets.example.com"],
                IncludeHeadersOnSubrequests = true, IdentifyAsHcti = false, BlockConsentBanners = true
            }
            : new CreateHtmlCssRecordingRequest { Html = "<h1>Hello</h1>", GoogleFonts = ["Roboto", "Open Sans"], DurationMs = 2000 };

        using var result = await CreateClient().CreateRecordingAsync(request, TestContext.Current.CancellationToken);
        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);
        Assert.Equal("rec_1", result.Response.Id);
        Assert.Equal("https://example.test/v1/recording/rec_1.mp4", result.Response.RecordingUrl);
        Assert.Equal("https://example.test/v1/recordings/rec_1/status", result.Response.StatusUrl);
        Assert.NotNull(result.HttpResponseMessage);
    }

    [Fact]
    public void RecordingOptions_SerializeNestedActionsAndPreserveEmptyBranches()
    {
        var request = new CreateHtmlCssRecordingRequest
        {
            Html = "<div id='demo'>Demo</div>", DurationMs = 5000, Css = "div { color: red; }",
            ViewportWidth = 1280, ViewportHeight = 720, DeviceScale = 2,
            DedupeDurationS = 0, DisableTwemoji = false, ColorScheme = ColorSchemeType.dark,
            Timezone = "America/New_York", MediaType = MediaType.screen,
            ViewportMobile = false, ViewportLandscape = true, ViewportTouch = false,
            ProxyId = "proxy_1", StorageDestinationId = "store_1",
            RequestOverrides = [new() { Url = "*analytics*" }],
            DurationMode = RecordingDurationMode.AtLeast, DeferRender = false, SkipScrollPreload = true,
            Output = new() { Format = RecordingFormat.Webm, Fps = 60, Scale = 0.5 },
            Trigger = new RecordingElementVisibleTrigger { Selector = "#demo", TimeoutMs = 1000 },
            Actions =
            [
                new RecordingLocateSelectorAction
                {
                    Selector = "#demo", TimeoutMs = 0,
                    IfFound = [new RecordingClickAction { Selector = "#demo", Button = RecordingMouseButton.Right, Count = 2, DelayMs = 0, OffsetX = 0, OffsetY = -1, IfNotFound = [new RecordingErrorAction()] }],
                    IfNotFound = []
                },
                new RecordingExecuteJavaScriptAction { Script = "await Promise.resolve();", IfError = [] },
                new RecordingScrollAction
                {
                    ContainerSelector = "main", IfNotFound = [new RecordingStallAction { DurationMs = 500 }],
                    Steps =
                    [
                        new() { To = new PositionScrollTarget { Position = ScrollPosition.Bottom }, DurationMs = 1000, Easing = ScrollEasing.EaseInOutCubic },
                        new() { To = new CoordinatesScrollTarget { X = 0, Y = 200 }, DurationMs = 500 },
                        new() { To = new SelectorScrollTarget { Selector = "footer", OffsetY = -40, IfNotFound = [] }, DurationMs = 500 }
                    ]
                },
                new RecordingSimpleScrollAction { ScrollDistance = "90vh", ScrollDurationMs = 800, MsBetweenScrolls = 0, StallAtEndMs = 1000, Easing = ScrollEasing.EaseInOutExpo }
            ]
        };
        var json = JsonNode.Parse(JsonSerializer.Serialize(request, JsonContext.Default.CreateHtmlCssRecordingRequest))!;
        Assert.Equal(1280, json["viewport_width"]!.GetValue<int>());
        Assert.Equal(720, json["viewport_height"]!.GetValue<int>());
        Assert.Equal(2, json["device_scale"]!.GetValue<double>());
        Assert.Equal(0, json["dedupe_duration_s"]!.GetValue<int>());
        Assert.False(json["disable_twemoji"]!.GetValue<bool>());
        Assert.Equal("dark", json["color_scheme"]!.GetValue<string>());
        Assert.Equal("America/New_York", json["timezone"]!.GetValue<string>());
        Assert.Equal("screen", json["media_type"]!.GetValue<string>());
        Assert.False(json["viewport_mobile"]!.GetValue<bool>());
        Assert.True(json["viewport_landscape"]!.GetValue<bool>());
        Assert.False(json["viewport_touch"]!.GetValue<bool>());
        Assert.Equal("proxy_1", json["proxy_id"]!.GetValue<string>());
        Assert.Equal("store_1", json["storage_destination_id"]!.GetValue<string>());
        Assert.Equal("*analytics*", json["request_overrides"]![0]!["url"]!.GetValue<string>());
        Assert.Equal("at_least", json["duration_mode"]!.GetValue<string>());
        Assert.False(json["defer_render"]!.GetValue<bool>());
        Assert.True(json["skip_scroll_preload"]!.GetValue<bool>());
        Assert.Equal("webm", json["output"]!["format"]!.GetValue<string>());
        Assert.Equal(60, json["output"]!["fps"]!.GetValue<int>());
        Assert.Equal(0.5, json["output"]!["scale"]!.GetValue<double>());
        Assert.Equal("element_visible", json["trigger"]!["type"]!.GetValue<string>());
        var actions = json["actions"]!.AsArray();
        Assert.Equal("locate_selector", actions[0]!["type"]!.GetValue<string>());
        Assert.Empty(actions[0]!["if_not_found"]!.AsArray());
        Assert.Equal("click", actions[0]!["if_found"]![0]!["type"]!.GetValue<string>());
        Assert.Equal("right", actions[0]!["if_found"]![0]!["button"]!.GetValue<string>());
        Assert.Equal("error", actions[0]!["if_found"]![0]!["if_not_found"]![0]!["type"]!.GetValue<string>());
        Assert.Empty(actions[1]!["if_error"]!.AsArray());
        Assert.Equal("execute_javascript", actions[1]!["type"]!.GetValue<string>());
        Assert.Equal("scroll", actions[2]!["type"]!.GetValue<string>());
        Assert.Equal("stall", actions[2]!["if_not_found"]![0]!["type"]!.GetValue<string>());
        Assert.Equal("position", actions[2]!["steps"]![0]!["to"]!["type"]!.GetValue<string>());
        Assert.Equal("bottom", actions[2]!["steps"]![0]!["to"]!["position"]!.GetValue<string>());
        Assert.Equal("ease_in_out_cubic", actions[2]!["steps"]![0]!["easing"]!.GetValue<string>());
        Assert.Equal("coordinates", actions[2]!["steps"]![1]!["to"]!["type"]!.GetValue<string>());
        Assert.Equal("selector", actions[2]!["steps"]![2]!["to"]!["type"]!.GetValue<string>());
        Assert.Equal(-40, actions[2]!["steps"]![2]!["to"]!["offset_y"]!.GetValue<double>());
        Assert.Equal("simple_scroll", actions[3]!["type"]!.GetValue<string>());
        Assert.Equal("90vh", actions[3]!["scroll_distance"]!.GetValue<string>());
        var roundTrip = JsonSerializer.Deserialize(json.ToJsonString(), JsonContext.Default.CreateHtmlCssRecordingRequest)!;
        Assert.IsType<RecordingElementVisibleTrigger>(roundTrip.Trigger);
        Assert.IsType<RecordingClickAction>(Assert.IsType<RecordingLocateSelectorAction>(roundTrip.Actions![0]).IfFound![0]);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void RecordingTriggers_SerializeImmediateAndFunctionCall(bool functionCall)
    {
        var request = new CreateUrlRecordingRequest
        {
            Url = "https://example.com", DurationMs = 1000,
            Trigger = functionCall ? new RecordingFunctionCallTrigger { TimeoutMs = 1500 } : new RecordingImmediateTrigger(),
            Actions = [new RecordingSimpleScrollAction { ScrollDistance = 900 }]
        };
        var json = JsonNode.Parse(JsonSerializer.Serialize(request, JsonContext.Default.CreateUrlRecordingRequest))!;
        Assert.Equal(functionCall ? "function_call" : "immediate", json["trigger"]!["type"]!.GetValue<string>());
        Assert.Equal(900, json["actions"]![0]!["scroll_distance"]!.GetValue<int>());
    }

    [Theory]
    [InlineData("processing", RecordingStatus.Processing, 202)]
    [InlineData("pending_retry", RecordingStatus.PendingRetry, 202)]
    [InlineData("awaiting_render", RecordingStatus.AwaitingRender, 202)]
    [InlineData("ready", RecordingStatus.Ready, 200)]
    [InlineData("failed", RecordingStatus.Failed, 200)]
    [InlineData("cancelled", RecordingStatus.Cancelled, 200)]
    public async Task FetchRecordingStatusAsync_MapsStatusAndAttemptDetails(string status, RecordingStatus expected, int statusCode)
    {
        Respond((request, _) =>
        {
            Assert.Equal(HttpMethod.Get, request.Method);
            Assert.Equal(new Uri("https://example.test/v1/recordings/rec_1/status"), request.RequestUri);
            return Task.FromResult(JsonResponse((HttpStatusCode)statusCode, $$$"""
                {"id":"rec_1","recording_url":"https://example.test/rec.mp4","status":"{{{status}}}",
                 "status_entered_at":"2026-11-16T10:00:00Z","attempt_count":2,"stored_duration_ms":1500,
                 "storage_destination_hcti_storage_disabled":true,
                 "error":{"code":"action_error","message":"Missing element","occurred_at":"2026-11-16T09:59:00Z","retryable":true,"action_path":"actions[1].if_found[0]"}}
                """));
        });
        using var result = await CreateClient().FetchRecordingStatusAsync("rec_1", TestContext.Current.CancellationToken);
        Assert.True(result.Success);
        Assert.Equal(statusCode, result.StatusCode);
        Assert.Equal(expected, result.Response.Status);
        Assert.Equal("rec_1", result.Response.Id);
        Assert.Equal(2u, result.Response.AttemptCount);
        Assert.Equal(1500L, result.Response.StoredDurationMs);
        Assert.True(result.Response.StorageDestinationHctiStorageDisabled);
        Assert.Equal(DateTimeOffset.Parse("2026-11-16T10:00:00Z"), result.Response.StatusEnteredAt);
        Assert.Equal("action_error", result.Response.Error!.Code);
        Assert.Equal("Missing element", result.Response.Error.Message);
        Assert.Equal("actions[1].if_found[0]", result.Response.Error.ActionPath);
        Assert.True(result.Response.Error.Retryable);
        Assert.Equal(DateTimeOffset.Parse("2026-11-16T09:59:00Z"), result.Response.Error.OccurredAt);
    }

    [Fact]
    public async Task FetchRecordingStatusAsync_AllowsNullMediaAndErrorDetails()
    {
        Respond((_, _) => Task.FromResult(JsonResponse(HttpStatusCode.Accepted, """
            {"id":"rec_1","recording_url":"https://example.test/rec.mp4","status":"processing",
             "status_entered_at":"2026-11-16T10:00:00Z","attempt_count":1,"stored_duration_ms":null,
             "storage_destination_hcti_storage_disabled":null,"error":null}
            """)));
        using var result = await CreateClient().FetchRecordingStatusAsync("rec_1", TestContext.Current.CancellationToken);
        Assert.True(result.Success);
        Assert.Null(result.Response.StoredDurationMs);
        Assert.Null(result.Response.Error);
        Assert.Null(result.Response.StorageDestinationHctiStorageDisabled);
    }

    [Fact]
    public async Task FetchRecordingStatusAsync_MapsUnrecognizedStatusToUnknown()
    {
        Respond((_, _) => Task.FromResult(JsonResponse(HttpStatusCode.Accepted, """
            {"id":"rec_1","recording_url":"https://example.test/rec.mp4","status":"some_future_status",
             "status_entered_at":"2026-11-16T10:00:00Z","attempt_count":1}
            """)));
        using var result = await CreateClient().FetchRecordingStatusAsync("rec_1", TestContext.Current.CancellationToken);
        Assert.True(result.Success);
        Assert.Equal(RecordingStatus.Unknown, result.Response.Status);
        Assert.Equal("rec_1", result.Response.Id);
    }

    [Theory]
    [InlineData(RecordingStatus.Processing, "processing")]
    [InlineData(RecordingStatus.PendingRetry, "pending_retry")]
    [InlineData(RecordingStatus.Ready, "ready")]
    [InlineData(RecordingStatus.Failed, "failed")]
    [InlineData(RecordingStatus.Cancelled, "cancelled")]
    [InlineData(RecordingStatus.AwaitingRender, "awaiting_render")]
    public void RecordingStatus_SerializesKnownValuesWithApiNames(RecordingStatus status, string expected)
    {
        var json = JsonNode.Parse(JsonSerializer.Serialize(new RecordingStatusResponse { Status = status }, JsonContext.Default.RecordingStatusResponse))!;
        Assert.Equal(expected, json["status"]!.GetValue<string>());
        var roundTrip = JsonSerializer.Deserialize(json.ToJsonString(), JsonContext.Default.RecordingStatusResponse)!;
        Assert.Equal(status, roundTrip.Status);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task DeleteRecordingAsync_SendsSingleOrBatchAndAcceptsEmptyResponse(bool batch)
    {
        Respond(async (request, ct) =>
        {
            Assert.Equal(HttpMethod.Delete, request.Method);
            Assert.Equal(new Uri(batch ? "https://example.test/v1/recordings/batch" : "https://example.test/v1/recordings/rec_1"), request.RequestUri);
            if (batch)
            {
                var json = JsonNode.Parse(await request.Content!.ReadAsStringAsync(ct))!;
                Assert.Equal(new[] { "rec_1", "rec_2" }, json["ids"]!.AsArray().Select(id => id!.GetValue<string>()));
            }
            else { Assert.Null(request.Content); }
            return new HttpResponseMessage(HttpStatusCode.Accepted);
        });
        var client = CreateClient();
        using var result = batch
            ? await client.DeleteRecordingBatchAsync(["rec_1", "rec_2"], TestContext.Current.CancellationToken)
            : await client.DeleteRecordingAsync("rec_1", TestContext.Current.CancellationToken);
        Assert.True(result.Success);
        Assert.True(result.Response);
        Assert.Equal(202, result.StatusCode);
    }

    [Fact]
    public async Task DeleteRecordingBatchAsync_WithNoIds_SucceedsWithoutSendingRequest()
    {
        using var result = await CreateClient().DeleteRecordingBatchAsync([], TestContext.Current.CancellationToken);
        Assert.True(result.Success);
        Assert.True(result.Response);
        Assert.Null(result.HttpResponseMessage);
        _handlerMock.Protected().Verify("SendAsync", Times.Never(), ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public async Task RecordingOperations_MapApiErrors(int operation)
    {
        Respond((_, _) => Task.FromResult(JsonResponse(HttpStatusCode.Forbidden, """
            {"error":"Forbidden","message":"Missing permission"}
            """)));
        using var result = await CallOperation(CreateClient(), operation, TestContext.Current.CancellationToken);
        Assert.NotNull(result);
    }

    private static async Task<IDisposable> CallOperation(IHtmlCssToImageClient client, int operation, CancellationToken ct)
    {
        switch (operation)
        {
            case 0:
                return CheckError(await client.CreateRecordingAsync(new CreateUrlRecordingRequest { Url = "https://example.com", DurationMs = 1000 }, ct));
            case 1: return CheckError(await client.FetchRecordingStatusAsync("rec_1", ct));
            case 2: return CheckError(await client.DeleteRecordingAsync("rec_1", ct));
            default: return CheckError(await client.DeleteRecordingBatchAsync(["rec_1"], ct));
        }
    }

    private static ApiResult<T> CheckError<T>(ApiResult<T> result)
    {
        Assert.False(result.Success);
        Assert.Equal(403, result.StatusCode);
        Assert.Null(result.Response);
        Assert.Equal("Forbidden", result.ErrorDetails!.ErrorType);
        Assert.Equal("Missing permission", result.ErrorDetails.Message);
        Assert.NotNull(result.HttpResponseMessage);
        return result;
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public async Task RecordingOperations_PropagateCancellation(int operation)
    {
        using var cts = new CancellationTokenSource();
        Respond((_, ct) =>
        {
            cts.Cancel();
            ct.ThrowIfCancellationRequested();
            throw new InvalidOperationException("The request token should have been cancelled.");
        });
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => CallOperation(CreateClient(), operation, cts.Token));
    }
}
