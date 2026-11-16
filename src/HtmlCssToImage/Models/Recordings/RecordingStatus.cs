using FancyEnumGenerator.Attributes;
using System.Text.Json.Serialization;
using HtmlCssToImage.Models.Converters;

namespace HtmlCssToImage.Models.Recordings;

/// <summary>Current recording lifecycle state. Ready, Failed, and Cancelled are terminal states.</summary>
[JsonConverter(typeof(RecordingStatusJsonConverter))]
[FancyEnum(DefaultToStringBehavior = FancyEnumDefaultToStringBehavior.JsonStringEnumMemberNameAttribute)]
public enum RecordingStatus
{
    /// <summary>An unrecognized status, such as one added in a newer API version.</summary>
    [JsonStringEnumMemberName("unknown")]
    Unknown = 0,
    /// <summary>The recording is being prepared, captured, or encoded.</summary>
    [JsonStringEnumMemberName("processing")]
    Processing,
    /// <summary>A recording attempt failed and is waiting to retry.</summary>
    [JsonStringEnumMemberName("pending_retry")]
    PendingRetry,
    /// <summary>The recorded media is ready.</summary>
    [JsonStringEnumMemberName("ready")]
    Ready,
    /// <summary>The recording failed and will not retry.</summary>
    [JsonStringEnumMemberName("failed")]
    Failed,
    /// <summary>The recording was cancelled.</summary>
    [JsonStringEnumMemberName("cancelled")]
    Cancelled,
    /// <summary>A deferred recording is waiting for a media or store request to start rendering.</summary>
    [JsonStringEnumMemberName("awaiting_render")]
    AwaitingRender,
}
