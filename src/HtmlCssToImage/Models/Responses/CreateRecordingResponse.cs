namespace HtmlCssToImage.Models.Responses;

/// <summary>A created or deduplicated recording and its status and media URLs.</summary>
public class CreateRecordingResponse
{
    /// <summary>Unique recording ID.</summary>
    public required string Id { get; set; }

    /// <summary>Authenticated endpoint for checking recording status.</summary>
    public required string StatusUrl { get; set; }

    /// <summary>Media URL with its format extension. Embed or request it directly: if the recording isn't finished, the request waits and then returns it (and starts rendering for a deferred recording). Media is hosted here only when HCTI storage is enabled.</summary>
    public required string RecordingUrl { get; set; }
}
