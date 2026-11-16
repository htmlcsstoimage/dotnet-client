using HtmlCssToImage.Models.Recordings;

namespace HtmlCssToImage.Models.Responses;

/// <summary>Current recording status and attempt information, without source content or rendering settings.</summary>
public class RecordingStatusResponse
{
    /// <summary>Duration of the saved media in milliseconds, or null until measured. This is the output duration, not processing time.</summary>
    public long? StoredDurationMs { get; set; }

    /// <summary>When true, media is saved only to the configured customer storage destination and is not hosted at recording_url.</summary>
    public bool? StorageDestinationHctiStorageDisabled { get; set; }

    /// <summary>URL for retrieving the recorded media, including its format extension.</summary>
    public string? RecordingUrl { get; set; }

    /// <summary>Unique recording ID.</summary>
    public string? Id { get; set; }

    /// <summary>Current recording status. Processing includes preparation and capture.</summary>
    public RecordingStatus? Status { get; set; }

    /// <summary>Date and time of the most recent processing-state transition, in UTC. This can change while the public status remains processing.</summary>
    public DateTimeOffset? StatusEnteredAt { get; set; }

    /// <summary>Number of recording attempts started, including retries.</summary>
    public uint? AttemptCount { get; set; }

    /// <summary>Most recently recorded error, or null when none is recorded. An error can describe a retryable attempt; use status to determine the recording's current state.</summary>
    public RecordingError? Error { get; set; }
}
