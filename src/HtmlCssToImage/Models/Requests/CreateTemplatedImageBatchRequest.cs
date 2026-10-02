namespace HtmlCssToImage.Models.Requests;

/// <summary>Creates a batch from one or more templates.</summary>
public class CreateTemplatedImageBatchRequest
{
    /// <summary>Optional shared template options inherited by every variation.</summary>
    public TemplatedBatchImageOptions? DefaultOptions { get; set; }
    /// <summary>One entry per image, in response order.</summary>
    public List<TemplatedBatchImageOptions> Variations { get; set; } = [];
}
