using System.Text.Json.Nodes;

namespace HtmlCssToImage.Models.Requests;

/// <summary>Shared defaults or one templated batch variation.</summary>
public class TemplatedBatchImageOptions
{
    /// <summary>Omit to inherit the default template. Supplying an ID resets the inherited version.</summary>
    public string? TemplateId { get; set; }
    /// <summary>Inherit the default version, or use latest when supplying a template ID.</summary>
    public long? TemplateVersion { get; set; }
    /// <summary>Objects merge recursively; arrays, scalars and explicit null values replace defaults.</summary>
    public JsonObject? TemplateValues { get; set; }
    /// <summary>Returned URL format. Omit to inherit the default format.</summary>
    public RenderImageFormat? Format { get; set; }
}
