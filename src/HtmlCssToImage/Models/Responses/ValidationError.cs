namespace HtmlCssToImage.Models.Responses;

/// <summary>
/// Represents a validation error encountered during the processing of a request.
/// </summary>
/// <remarks>
/// The <see cref="ValidationError" /> record provides details about validation issues,
/// specifically the property that triggered the error and the corresponding error message.
/// </remarks>
/// <param name="Path">
/// The path or context indicating where the validation error occurred,
/// typically pointing to the relevant property or field.
/// </param>
/// <param name="Message">
/// A descriptive message explaining the nature of the validation error.
/// </param>
public record ValidationError(string Path, string Message);