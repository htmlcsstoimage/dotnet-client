namespace HtmlCssToImage.Models.Requests;

/// <summary>
/// Represents a request to create a batch of images with configurable options.
/// </summary>
/// <remarks>
/// This class is used to specify the default options for image generation and
/// a collection of variations, where each variation can override the default options.
/// </remarks>
public class CreateImageBatchRequest<T> where T:IBatchAllowedImageRequest
{
    /// <summary>
    /// Gets or sets the default options to be applied to a batch of image creation requests.
    /// This property allows specifying common configuration settings that will be inherited by
    /// individual requests within the batch, unless explicitly overridden by those specific requests.
    /// </summary>
    public T? DefaultOptions { get; set; }


    /// <summary>
    /// Gets the collection of image request variations to be included in the batch.
    /// Each variation represents an individual configuration and customization of the image
    /// creation process within the batch request.
    /// </summary>
    public List<T> Variations { get; private set; } = [];
}