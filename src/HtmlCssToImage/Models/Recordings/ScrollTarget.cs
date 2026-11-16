using System.Text.Json.Serialization;

namespace HtmlCssToImage.Models.Recordings;

/// <summary>
/// Base type for a scroll destination. Use <see cref="PositionScrollTarget"/>, <see cref="CoordinatesScrollTarget"/>, or <see cref="SelectorScrollTarget"/>.
/// <see href="https://docs.htmlcsstoimage.com/parameters/actions/">Read the actions documentation for more information.</see>
/// </summary>
[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(PositionScrollTarget), "position")]
[JsonDerivedType(typeof(CoordinatesScrollTarget), "coordinates")]
[JsonDerivedType(typeof(SelectorScrollTarget), "selector")]
public abstract class ScrollTarget
{
}
