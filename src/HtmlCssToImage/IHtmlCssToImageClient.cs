using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization.Metadata;
using HtmlCssToImage.Models;
using HtmlCssToImage.Models.Requests;
using HtmlCssToImage.Models.Responses;
using HtmlCssToImage.Models.Results;

namespace HtmlCssToImage;

/// <summary>
/// Provides methods to create and interact with the HTML/CSS to Image API. Supports operations for asynchronous image creation,
/// batch processing, and generating URLs for templated images with rendering format options.
/// </summary>
public interface IHtmlCssToImageClient
{
    /// <summary>
    /// Creates an image based on the provided request object and returns the result, including metadata such as the image URL and ID.
    /// </summary>
    /// <typeparam name="T">The type of the request object, which must inherit from <see cref="ICreateImageRequestBase"/>.</typeparam>
    /// <param name="request">The request object containing the parameters for image creation, such as dimensions, rendering preferences, or specific selectors.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation before completion.</param>
    /// <returns>An <see cref="ApiResult{T}"/> object containing the response with metadata such as the image's URL and ID, as well as the status of the operation.</returns>
    public Task<ApiResult<CreateImageResponse?>> CreateImageAsync<T>(T request, CancellationToken cancellationToken = default) where T : ICreateImageRequestBase;

    /// <summary>
    /// Sends a batch request to create multiple images using the specified parameters.
    /// </summary>
    /// <param name="request">The batch request containing default options and variations for creating images.</param>
    /// <param name="cancellationToken">An optional cancellation token to cancel the operation.</param>
    /// <returns>An <see cref="ApiResult{T}"/> containing an array of <see cref="CreateImageResponse"/> objects if successful, or error details if the operation fails.</returns>
    public Task<ApiResult<CreateImageResponse[]?>> CreateImageBatchAsync<T>(CreateImageBatchRequest<T> request,
        CancellationToken cancellationToken = default) where T:IBatchAllowedImageRequest;

    /// <summary>
    /// Creates a batch of images based on the provided default options and a collection of variations,
    /// and returns the batch processing results for each image, including metadata such as image URLs and IDs.
    /// </summary>
    /// <typeparam name="T">The type of the request object, which must implement <see cref="IBatchAllowedImageRequest"/>.</typeparam>
    /// <param name="defaultOptions">The default options to be applied to all images in the batch. This can be null if no default options are needed.</param>
    /// <param name="variations">A collection of request objects representing specific variations for individual images in the batch.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation before completion.</param>
    /// <returns>An <see cref="ApiResult{T}"/> object containing an array of <see cref="CreateImageResponse"/> objects for each image in the batch,
    /// along with the status of the operation.</returns>
    public Task<ApiResult<CreateImageResponse[]?>> CreateImageBatchAsync<T>(T? defaultOptions, IEnumerable<T> variations,
        CancellationToken cancellationToken = default) where T:IBatchAllowedImageRequest;

    /// <summary>
    /// Generates a URL for creating and rendering an image based on the specified request parameters and output format.
    /// </summary>
    /// <param name="request">
    /// The <see cref="CreateUrlImageRequest"/> object containing the URL and other optional parameters for rendering the image.
    /// </param>
    /// <param name="format">
    /// The desired image format for the output, such as PNG, JPG, or WEBP. The default value is <see cref="RenderImageFormat.PNG"/>.
    /// </param>
    /// <returns>
    /// A string representing the generated URL for the image rendering request.
    /// </returns>
    public string CreateAndRenderUrl(CreateUrlImageRequest request, RenderImageFormat format = RenderImageFormat.PNG);

    /// <summary>
    /// Generates a URL for creating an image using a predefined template, with customizable template values,
    /// optional template version, and specified output format.
    /// </summary>
    /// <param name="templateId">
    /// The unique identifier of the template to use for generating the image.
    /// </param>
    /// <param name="templateValues">
    /// A JSON object representing the key-value pairs for the template variables.
    /// </param>
    /// <param name="templateVersion">
    /// (Optional) The version of the template to use. If not specified, the latest version is used.
    /// </param>
    /// <param name="format">
    /// (Optional) The format of the output image (e.g., PNG, JPG, WEBP). Defaults to PNG.
    /// </param>
    /// <returns>
    /// A URL string that can be used to generate and retrieve the image based on the given template and values.
    /// </returns>
    public string CreateTemplatedImageUrl(string templateId, JsonObject templateValues, long? templateVersion = null, RenderImageFormat format = RenderImageFormat.PNG);

    /// <summary>
    /// Generates a templated image URL based on the specified template identifier and template values.
    /// </summary>
    /// <param name="templateId">The identifier of the template to be used for rendering the image.</param>
    /// <param name="templateValues">An object containing key-value pairs representing the values to substitute into the template.</param>
    /// <param name="templateVersion">The version of the template to be used. If null, the default version is used.</param>
    /// <param name="format">The desired image format for the rendered image (e.g., PNG, JPG, WEBP).</param>
    /// <returns>A string containing the URL of the generated templated image.</returns>
    [RequiresUnreferencedCode("If AOT is needed, use one of the overloads with explicit type information")]
    [RequiresDynamicCode("If AOT is needed, use one of the overloads with explicit type information")]
    public string CreateTemplatedImageUrl<T>(string templateId, T templateValues, long? templateVersion = null, RenderImageFormat format = RenderImageFormat.PNG);


    /// <summary>
    /// Generates a templated image URL based on the specified template identifier and template values.
    /// </summary>
    /// <param name="templateId">The identifier of the template to be used for rendering the image.</param>
    /// <param name="templateValues">An object containing key-value pairs representing the values to substitute into the template.</param>
    /// <param name="templateVersion">The version of the template to be used. If null, the default version is used.</param>
    /// <param name="typeInfo">The JsonTypeInfo object for the type of the template values.</param>
    /// <param name="format">The desired image format for the rendered image (e.g., PNG, JPG, WEBP).</param>
    /// <returns>A string containing the URL of the generated templated image.</returns>
    public string CreateTemplatedImageUrl<T>(string templateId, T templateValues, JsonTypeInfo<T> typeInfo, long? templateVersion = null, RenderImageFormat format = RenderImageFormat.PNG);

    /// <summary>
    /// Generates a URL for a templated image based on the provided template identifier, template values, and optional parameters.
    /// </summary>
    /// <param name="templateId">The unique identifier of the template to use for image generation.</param>
    /// <param name="templateValues">The dynamic values to populate the template during the image generation process.</param>
    /// <param name="templateVersion">An optional version of the template to use. If null, the latest version will be used.</param>
    /// <param name="jsonSerializerOptions">An optional JsonSerializerOptions object to use for serializing the template values.</param>
    /// <param name="format">Specifies the format of the generated image. Defaults to RenderImageFormat.PNG.</param>
    /// <returns>A string containing the URL of the generated templated image.</returns>
    public string CreateTemplatedImageUrl<T>(string templateId, T templateValues, JsonSerializerOptions jsonSerializerOptions, long? templateVersion = null, RenderImageFormat format = RenderImageFormat.PNG);


    /// <summary>
    /// Creates a new version of a template using the specified template ID and request parameters.
    /// </summary>
    /// <param name="templateId">The unique identifier of the template to create a version for.</param>
    /// <param name="request">The <see cref="CreateTemplateRequest"/> object containing the details for the new template version, such as updated configuration or settings.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation before it completes.</param>
    /// <returns>An <see cref="ApiResult{CreateTemplateResponse}"/> object containing the response data for the created template version
    /// and its associated metadata, or error details if the operation fails.</returns>
    public Task<ApiResult<CreateTemplateResponse?>> CreateTemplateVersionAsync(string templateId, CreateTemplateRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new template based on the provided request and returns the result, including metadata about the created template.
    /// </summary>
    /// <param name="request">The request object containing the parameters for creating a template, such as template properties or layout configurations.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation before completion.</param>
    /// <returns>An <see cref="ApiResult{T}"/> object containing the response with metadata about the newly created template and the status of the operation.</returns>
    public Task<ApiResult<CreateTemplateResponse?>> CreateTemplateAsync(CreateTemplateRequest request, CancellationToken cancellationToken = default);


    /// <summary>
    /// Retrieves a paginated list of template versions for a specified template ID.
    /// </summary>
    /// <param name="templateId">The unique identifier of the template for which versions are to be listed.</param>
    /// <param name="count">The maximum number of template versions to include in the response. Defaults to 10.</param>
    /// <param name="nextPageStart">An optional parameter indicating the starting point for the next page of results, if pagination is needed.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation before completion.</param>
    /// <returns>An <see cref="ApiResult{T}"/> object containing a <see cref="PaginatedResponse{Template}"/> with the retrieved template versions or null if no versions exist.</returns>
    public Task<ApiResult<PaginatedResponse<Template>?>> ListTemplateVersionsAsync(string templateId, int count = 10, long? nextPageStart = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a paginated list of templates, retrieving the most recent version for each.
    /// </summary>
    /// <param name="count">The maximum number of templates to retrieve in a single page. Defaults to 10 if not specified.</param>
    /// <param name="nextPageStart">An optional parameter specifying the starting point for the next page of results. Null indicates the first page.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation before completion.</param>
    /// <returns>An <see cref="ApiResult{T}"/> containing a <see cref="PaginatedResponse{T}"/> object, which holds the returned templates and pagination metadata.</returns>
    public Task<ApiResult<PaginatedResponse<Template>?>> ListTemplatesAsync(int count = 10, long? nextPageStart = null, CancellationToken cancellationToken = default);


}