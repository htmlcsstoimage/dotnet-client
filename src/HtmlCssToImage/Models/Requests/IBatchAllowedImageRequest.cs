namespace HtmlCssToImage.Models.Requests;

/// <summary>
/// Represents a marker interface for image requests that are supported in batch operations.
/// </summary>
/// <remarks>
/// Implementations of this interface define the structure and parameters for image requests
/// that can be processed together in batch processing scenarios. It provides a mechanism
/// to ensure type consistency among various image request types that share this capability.
/// </remarks>
public interface IBatchAllowedImageRequest
{

}