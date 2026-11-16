namespace HtmlCssToImage.Models.Requests;

/// <summary>A request to delete multiple recordings in one API call.</summary>
public sealed class DeleteRecordingBatchRequest
{
    /// <summary>IDs of the recordings to delete.</summary>
    public string[] Ids { get; set; } = [];
}
