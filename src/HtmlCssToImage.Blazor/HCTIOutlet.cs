using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Sections;

namespace HtmlCssToImage.Blazor;

public class HCTIOutlet : ComponentBase {
    [Parameter] public string[] AdditionalMetaTypes { get; set; } = Array.Empty<string>();
    private static readonly string[] DefaultMetaTypes = ["og:image"];

    internal IEnumerable<string> SectionIds => DefaultMetaTypes.Union(AdditionalMetaTypes);

    protected override void BuildRenderTree(RenderTreeBuilder builder) {
        foreach (var t in SectionIds) {
            builder.OpenComponent<SectionOutlet>(0);
            builder.AddComponentParameter(1, nameof(SectionOutlet.SectionId), SectionId(t));
            builder.CloseComponent();
        }

    }

    internal static string SectionId(string metaType)=> $"hcti-meta-{metaType}";
}