using System.Runtime.CompilerServices;
using HtmlCssToImage.Models;

namespace HtmlCssToImage.Helpers;

internal static class EnumToString
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static string ColorSchemeString(this ColorSchemeType type) => type switch
    {
        ColorSchemeType.dark  => nameof(ColorSchemeType.dark),
        ColorSchemeType.light => nameof(ColorSchemeType.light),
        _                     => throw new ArgumentOutOfRangeException(nameof(type), type, null)
    };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static string MediaTypeString(this MediaType type) => type switch
    {
        MediaType.screen => nameof(MediaType.screen),
        MediaType.print  => nameof(MediaType.print),
        _                => throw new ArgumentOutOfRangeException(nameof(type), type, null)
    };
}