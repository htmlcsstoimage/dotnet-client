using System.Numerics;
using System.Runtime.CompilerServices;

namespace HtmlCssToImage.Helpers;

internal static class NumberHelpers
{
    private static readonly ulong[] _powersOf10 =
    [
        1u, 10u, 100u, 1000u, 10000u, 100000u, 1000000u, 10000000u, 100000000u,
        1000000000u, 10000000000u, 100000000000u, 1000000000000u,
        10000000000000u, 100000000000000u, 1000000000000000u,
        10000000000000000u, 100000000000000000u, 1000000000000000000u,
        10000000000000000000u
    ];
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int GetDigitsCount(ulong value)
    {
        if (value == 0)
        {
            return 1;
        }

        int log2 = BitOperations.Log2(value);
        int log10 = (log2 * 1233) >> 12;
        return value >= _powersOf10[log10 + 1] ? log10 + 2 : log10 + 1;
    }
}