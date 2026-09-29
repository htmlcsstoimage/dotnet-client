using FancyEnumGenerator.Attributes;
using System.Text.Json.Serialization;

namespace HtmlCssToImage.Models.Requests;

/// <summary>Browser resource types that a request override can match.</summary>
[FancyEnum(AllowNoUnknown = true, DefaultToStringBehavior = FancyEnumDefaultToStringBehavior.JsonStringEnumMemberNameAttribute)]
public enum RequestOverrideResourceType
{
    /// <summary>Beacon request.</summary>
    [JsonStringEnumMemberName("beacon")]
    Beacon,
    /// <summary>Document request.</summary>
    [JsonStringEnumMemberName("document")]
    Document,
    /// <summary>Stylesheet request.</summary>
    [JsonStringEnumMemberName("stylesheet")]
    Stylesheet,
    /// <summary>Image request.</summary>
    [JsonStringEnumMemberName("image")]
    Image,
    /// <summary>Image set request.</summary>
    [JsonStringEnumMemberName("image_set")]
    ImageSet,
    /// <summary>Media request.</summary>
    [JsonStringEnumMemberName("media")]
    Media,
    /// <summary>Font request.</summary>
    [JsonStringEnumMemberName("font")]
    Font,
    /// <summary>Script request.</summary>
    [JsonStringEnumMemberName("script")]
    Script,
    /// <summary>Text track request.</summary>
    [JsonStringEnumMemberName("text_track")]
    TextTrack,
    /// <summary>XML HTTP request.</summary>
    [JsonStringEnumMemberName("xhr")]
    Xhr,
    /// <summary>Fetch request.</summary>
    [JsonStringEnumMemberName("fetch")]
    Fetch,
    /// <summary>Event source request.</summary>
    [JsonStringEnumMemberName("event_source")]
    EventSource,
    /// <summary>Manifest request.</summary>
    [JsonStringEnumMemberName("manifest")]
    Manifest,
    /// <summary>Ping request.</summary>
    [JsonStringEnumMemberName("ping")]
    Ping,
    /// <summary>Legacy img request type.</summary>
    [JsonStringEnumMemberName("img")]
    Img,
    /// <summary>Other request type.</summary>
    [JsonStringEnumMemberName("other")]
    Other
}
