using System.Collections.Frozen;

namespace BlazorStaticServerSample;

public record ItemDetail(long Id, string Name, int Quantity);

public static class ItemDetails
{
    public static readonly FrozenDictionary<long, ItemDetail> Items = Enumerable.Range(0,100).ToDictionary(i=> (long)i, i=> new ItemDetail(i, $"Item {i}", 1+i%10)).ToFrozenDictionary();
}