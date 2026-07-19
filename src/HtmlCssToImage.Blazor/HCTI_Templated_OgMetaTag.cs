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
            MetaUrl = RenderOptions is null
                ? HtmlCssToImageClient.CreateTemplatedImageUrl(TemplateId, jo, TemplateVersion)
                : HtmlCssToImageClient.CreateTemplatedImageUrl(
                    TemplateId,
                    jo,
                    TemplateVersion,
                    RenderOptions);
        }
        else if(TemplateValues!=null)
        {
            if (TypeInfo != null)
            {
                MetaUrl = RenderOptions is null
                    ? HtmlCssToImageClient.CreateTemplatedImageUrl(
                        TemplateId,
                        TemplateValues,
                        TypeInfo,
                        TemplateVersion)
                    : HtmlCssToImageClient.CreateTemplatedImageUrl(
                        TemplateId,
                        TemplateValues,
                        TypeInfo,
                        TemplateVersion,
                        RenderOptions);
            }else if (JsonSerializerOptions != null)
            {
                MetaUrl = RenderOptions is null
                    ? HtmlCssToImageClient.CreateTemplatedImageUrl(
                        TemplateId,
                        TemplateValues,
                        JsonSerializerOptions,
                        TemplateVersion)
                    : HtmlCssToImageClient.CreateTemplatedImageUrl(
                        TemplateId,
                        TemplateValues,
                        JsonSerializerOptions,
                        TemplateVersion,
                        RenderOptions);
            }
            else
            {
                MetaUrl = RenderOptions is null
                    ? HtmlCssToImageClient.CreateTemplatedImageUrl(
                        TemplateId,
                        TemplateValues,
                        TemplateVersion)
                    : HtmlCssToImageClient.CreateTemplatedImageUrl(
                        TemplateId,
                        TemplateValues,
                        TemplateVersion,
                        RenderOptions);
            }

        }
    }
}
