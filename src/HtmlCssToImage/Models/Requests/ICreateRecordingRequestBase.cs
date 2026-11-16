using System.Text.Json.Serialization;

namespace HtmlCssToImage.Models.Requests;

/// <summary>Defines the base interface for HTML/CSS and URL recording creation requests.</summary>
[JsonDerivedType(typeof(CreateHtmlCssRecordingRequest))]
[JsonDerivedType(typeof(CreateUrlRecordingRequest))]
public interface ICreateRecordingRequestBase
{
}
