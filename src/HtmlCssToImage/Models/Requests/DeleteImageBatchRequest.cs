namespace HtmlCssToImage.Models.Requests;

/// <summary>
/// Represents a request to delete multiple images in one API call.
/// </summary>
public sealed class DeleteImageBatchRequest
{
    /// <summary>
    /// Gets or sets the IDs of the images to delete.
    /// </summary>
    public string[] Ids { get; set; } = [];
}
