using System.Buffers;
using System.Runtime.CompilerServices;

namespace HtmlCssToImage.Helpers;

internal ref struct ArrayOrSpan<T>:IDisposable
{
    private T[]? _rented;
    public Span<T> Span;

    public Span<T> RemainingSpan
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Span[Position..];
    }


    public Span<T> LimitedSpan => Span[..Position];

    public int Position;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ArrayOrSpan(Span<T> initialBuffer)
    {
        Span = initialBuffer;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ArrayOrSpan(int size)
    {
        _rented=ArrayPool<T>.Shared.Rent(size);
        Span = _rented.AsSpan();
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose()
    {
        if (_rented != null)
        {
            ArrayPool<T>.Shared.Return(_rented);
            _rented = null;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Advance(int count)
    {
        Position += count;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void EnsureCapacity(int additionalCapacity)
    {
        int requiredCapacity = Position + additionalCapacity;
        if (requiredCapacity > Span.Length)
        {
            var newSize = requiredCapacity + 128;
            var newRented = ArrayPool<T>.Shared.Rent(newSize);

            Span[..Position].CopyTo(newRented);

            if (_rented != null)
            {
                ArrayPool<T>.Shared.Return(_rented);
            }

            _rented = newRented;
            Span = _rented;
        }
    }
}