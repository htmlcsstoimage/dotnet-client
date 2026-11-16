using System.Text.Json.Serialization;

namespace HtmlCssToImage.Models.Recordings;

/// <summary>
/// Base type for recording start conditions. Use <see cref="RecordingImmediateTrigger"/>, <see cref="RecordingElementVisibleTrigger"/>, or <see cref="RecordingFunctionCallTrigger"/>.
/// <see href="https://docs.htmlcsstoimage.com/parameters/trigger/">Read the trigger documentation for more information.</see>
/// </summary>
[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(RecordingImmediateTrigger), "immediate")]
[JsonDerivedType(typeof(RecordingElementVisibleTrigger), "element_visible")]
[JsonDerivedType(typeof(RecordingFunctionCallTrigger), "function_call")]
public abstract class RecordingTrigger
{
}
