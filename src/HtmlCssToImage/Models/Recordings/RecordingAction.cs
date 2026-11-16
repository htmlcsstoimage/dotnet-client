using System.Text.Json.Serialization;

namespace HtmlCssToImage.Models.Recordings;

/// <summary>
/// Base type for interactions performed while recording. Actions run in order; each action in a nested branch supplies its own selectors.
/// <see href="https://docs.htmlcsstoimage.com/parameters/actions/">Read the actions documentation for more information.</see>
/// </summary>
[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(RecordingScrollAction), "scroll")]
[JsonDerivedType(typeof(RecordingStallAction), "stall")]
[JsonDerivedType(typeof(RecordingLocateSelectorAction), "locate_selector")]
[JsonDerivedType(typeof(RecordingClickAction), "click")]
[JsonDerivedType(typeof(RecordingErrorAction), "error")]
[JsonDerivedType(typeof(RecordingSimpleScrollAction), "simple_scroll")]
[JsonDerivedType(typeof(RecordingExecuteJavaScriptAction), "execute_javascript")]
public abstract class RecordingAction
{
}
