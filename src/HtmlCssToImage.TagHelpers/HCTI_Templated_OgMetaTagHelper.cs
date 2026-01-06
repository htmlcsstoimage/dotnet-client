using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization.Metadata;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace HtmlCssToImage.TagHelpers;

[HtmlTargetElement("hcti-og-templated", TagStructure = TagStructure.WithoutEndTag)]
public class HCTI_Templated_OgMetaTagHelper:HCTI_OgMetaTagHelperBase
{
    public HCTI_Templated_OgMetaTagHelper(IHtmlCssToImageClient htmlCssToImageClient) : base(htmlCssToImageClient)
    {

    }

    [HtmlAttributeName("template-id")]
    public string TemplateId { get; set; } = null!;

    [HtmlAttributeName("template-version")]
    public long? TemplateVersion { get; set; }

    [HtmlAttributeName("template-values")]
    public object? TemplateValues { get; set; }



    [HtmlAttributeName("json-options")]
    public JsonSerializerOptions? JsonSerializerOptions { get; set; }

    protected override void SetMetaUrl()
    {
        if (TemplateValues is JsonObject jo)
        {
            MetaUrl= _htmlCssToImageClient.CreateTemplatedImageUrl(TemplateId, jo, TemplateVersion);
        }
        else if(TemplateValues!=null)
        {
            if (JsonSerializerOptions != null)
            {
                MetaUrl= _htmlCssToImageClient.CreateTemplatedImageUrl(TemplateId, TemplateValues, JsonSerializerOptions, TemplateVersion);
            }
            else
            {
                MetaUrl= _htmlCssToImageClient.CreateTemplatedImageUrl(TemplateId, TemplateValues, TemplateVersion);
            }

        }
    }

}