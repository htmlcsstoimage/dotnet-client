namespace HtmlCssToImage.Models.Responses;

/// <summary>
/// Represents the response returned after creating or updating a template.
/// </summary>
/// <remarks>
/// The response includes the unique identifier for the template and the version number of the template.
/// </remarks>
public record CreateTemplateResponse(string TemplateId, long TemplateVersion);