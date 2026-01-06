using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization.Metadata;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Sections;
using Microsoft.AspNetCore.Components.Web;

namespace HtmlCssToImage.Blazor;


public class HCTI_Templated_OgMetaTag<T>:HCTI_OgMetaTagBase
{
    [Parameter, EditorRequired] public string TemplateId { get; set; } = null!;
    [Parameter] public long? TemplateVersion { get; set; }
    [Parameter] public T? TemplateValues { get; set; }
    [Parameter] public JsonTypeInfo<T>? TypeInfo { get; set; }
    [Parameter] public JsonSerializerOptions? JsonSerializerOptions { get; set; }



    protected override void SetMetaUrl()
    {
        if (TemplateValues is JsonObject jo)
        {
            MetaUrl= HtmlCssToImageClient.CreateTemplatedImageUrl(TemplateId, jo, TemplateVersion);
        }
        else if(TemplateValues!=null)
        {
            if (TypeInfo != null)
            {
                MetaUrl= HtmlCssToImageClient.CreateTemplatedImageUrl(TemplateId, TemplateValues, TypeInfo, TemplateVersion);
            }else if (JsonSerializerOptions != null)
            {
                MetaUrl= HtmlCssToImageClient.CreateTemplatedImageUrl(TemplateId, TemplateValues, JsonSerializerOptions, TemplateVersion);
            }
            else
            {
                MetaUrl= HtmlCssToImageClient.CreateTemplatedImageUrl(TemplateId, TemplateValues, TemplateVersion);
            }

        }
    }
}