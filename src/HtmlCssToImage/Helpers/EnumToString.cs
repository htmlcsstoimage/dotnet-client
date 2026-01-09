using System.Runtime.CompilerServices;
using HtmlCssToImage.Models;

namespace HtmlCssToImage.Helpers;

internal static class EnumToString
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static string ColorSchemeString(ColorSchemeType type) => type switch
    {
        ColorSchemeType.dark  => nameof(ColorSchemeType.dark),
        ColorSchemeType.light => nameof(ColorSchemeType.light),
        _                     => throw new ArgumentOutOfRangeException(nameof(type), type, null)
    };
}