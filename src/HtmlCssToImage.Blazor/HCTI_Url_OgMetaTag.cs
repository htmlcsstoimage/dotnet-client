using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization.Metadata;
using HtmlCssToImage.Models.Requests;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Sections;
using Microsoft.AspNetCore.Components.Web;

namespace HtmlCssToImage.Blazor;

public class HCTI_Url_OgMetaTag:HCTI_OgMetaTagBase
{

    [Parameter, EditorRequired] public CreateUrlImageRequest ImageRequest { get; set; } = null!;

    protected override void SetMetaUrl()
    {
        MetaUrl= HtmlCssToImageClient.CreateAndRenderUrl(ImageRequest);
    }
    
}