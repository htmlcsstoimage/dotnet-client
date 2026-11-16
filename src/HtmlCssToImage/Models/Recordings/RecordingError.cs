namespace HtmlCssToImage.Models.Recordings;

/// <summary>Error information from a recording attempt.</summary>
public class RecordingError
{
    /// <summary>Machine-readable error code identifying the failure.</summary>
    public string? Code { get; set; }

    /// <summary>Human-readable explanation of the failure.</summary>
    public string? Message { get; set; }

    /// <summary>Date and time the error occurred, in UTC.</summary>
    public DateTimeOffset? OccurredAt { get; set; }

    /// <summary>Whether this kind of error is eligible for retry. A further attempt still depends on the remaining retry budget and the recording's status.</summary>
    public bool? Retryable { get; set; }

    /// <summary>Path to the action associated with the failure, such as actions[1].if_found[0], or null when the error is not associated with an action.</summary>
    public string? ActionPath { get; set; }
}
