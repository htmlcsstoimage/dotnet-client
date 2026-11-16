using FancyEnumGenerator.Attributes;
using System.Text.Json.Serialization;

namespace HtmlCssToImage.Models.Recordings;

/// <summary>Easing curve used for a scroll movement.</summary>
[FancyEnum(AllowNoUnknown = true, DefaultToStringBehavior = FancyEnumDefaultToStringBehavior.JsonStringEnumMemberNameAttribute)]
public enum ScrollEasing
{
    /// <summary>Linear.</summary>
    [JsonStringEnumMemberName("linear")]
    Linear,
    /// <summary>Ease in cubic.</summary>
    [JsonStringEnumMemberName("ease_in_cubic")]
    EaseInCubic,
    /// <summary>Ease out cubic.</summary>
    [JsonStringEnumMemberName("ease_out_cubic")]
    EaseOutCubic,
    /// <summary>Ease in out cubic.</summary>
    [JsonStringEnumMemberName("ease_in_out_cubic")]
    EaseInOutCubic,
    /// <summary>Ease in quad.</summary>
    [JsonStringEnumMemberName("ease_in_quad")]
    EaseInQuad,
    /// <summary>Ease out quad.</summary>
    [JsonStringEnumMemberName("ease_out_quad")]
    EaseOutQuad,
    /// <summary>Ease in out quad.</summary>
    [JsonStringEnumMemberName("ease_in_out_quad")]
    EaseInOutQuad,
    /// <summary>Ease in quart.</summary>
    [JsonStringEnumMemberName("ease_in_quart")]
    EaseInQuart,
    /// <summary>Ease out quart.</summary>
    [JsonStringEnumMemberName("ease_out_quart")]
    EaseOutQuart,
    /// <summary>Ease in out quart.</summary>
    [JsonStringEnumMemberName("ease_in_out_quart")]
    EaseInOutQuart,
    /// <summary>Ease in quint.</summary>
    [JsonStringEnumMemberName("ease_in_quint")]
    EaseInQuint,
    /// <summary>Ease out quint.</summary>
    [JsonStringEnumMemberName("ease_out_quint")]
    EaseOutQuint,
    /// <summary>Ease in out quint.</summary>
    [JsonStringEnumMemberName("ease_in_out_quint")]
    EaseInOutQuint,
    /// <summary>Ease in sine.</summary>
    [JsonStringEnumMemberName("ease_in_sine")]
    EaseInSine,
    /// <summary>Ease out sine.</summary>
    [JsonStringEnumMemberName("ease_out_sine")]
    EaseOutSine,
    /// <summary>Ease in out sine.</summary>
    [JsonStringEnumMemberName("ease_in_out_sine")]
    EaseInOutSine,
    /// <summary>Ease in expo.</summary>
    [JsonStringEnumMemberName("ease_in_expo")]
    EaseInExpo,
    /// <summary>Ease out expo.</summary>
    [JsonStringEnumMemberName("ease_out_expo")]
    EaseOutExpo,
    /// <summary>Ease in out expo.</summary>
    [JsonStringEnumMemberName("ease_in_out_expo")]
    EaseInOutExpo,
    /// <summary>Ease in circ.</summary>
    [JsonStringEnumMemberName("ease_in_circ")]
    EaseInCirc,
    /// <summary>Ease out circ.</summary>
    [JsonStringEnumMemberName("ease_out_circ")]
    EaseOutCirc,
    /// <summary>Ease in out circ.</summary>
    [JsonStringEnumMemberName("ease_in_out_circ")]
    EaseInOutCirc,
    /// <summary>Ease in back.</summary>
    [JsonStringEnumMemberName("ease_in_back")]
    EaseInBack,
    /// <summary>Ease out back.</summary>
    [JsonStringEnumMemberName("ease_out_back")]
    EaseOutBack,
    /// <summary>Ease in out back.</summary>
    [JsonStringEnumMemberName("ease_in_out_back")]
    EaseInOutBack,
    /// <summary>Ease in elastic.</summary>
    [JsonStringEnumMemberName("ease_in_elastic")]
    EaseInElastic,
    /// <summary>Ease out elastic.</summary>
    [JsonStringEnumMemberName("ease_out_elastic")]
    EaseOutElastic,
    /// <summary>Ease in out elastic.</summary>
    [JsonStringEnumMemberName("ease_in_out_elastic")]
    EaseInOutElastic,
    /// <summary>Ease in bounce.</summary>
    [JsonStringEnumMemberName("ease_in_bounce")]
    EaseInBounce,
    /// <summary>Ease out bounce.</summary>
    [JsonStringEnumMemberName("ease_out_bounce")]
    EaseOutBounce,
    /// <summary>Ease in out bounce.</summary>
    [JsonStringEnumMemberName("ease_in_out_bounce")]
    EaseInOutBounce,
}
