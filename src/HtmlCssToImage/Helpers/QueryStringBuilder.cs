using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace HtmlCssToImage.Helpers;

internal ref struct QueryStringBuilder : IDisposable
{
    private ArrayOrSpan<char> _chars;
    private int _parameterCount;
    private int _questionMarkPosition;

    public QueryStringBuilder(Span<char> initial)
    {
        _chars = new(initial);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendLiteral(ReadOnlySpan<char> value)
    {
        _chars.EnsureCapacity(value.Length);
        value.CopyTo(_chars.RemainingSpan);
        _chars.Advance(value.Length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendLiteral(char value)
    {
        _chars.EnsureCapacity(1);
        _chars.RemainingSpan[0] = value;
        _chars.Advance(1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int ReserveLiteral(int length)
    {
        _chars.EnsureCapacity(length);
        var position = _chars.Position;
        _chars.Advance(length);
        return position;
    }

    public Span<char> ReservedLiteral(int position, int length)
    {
        return _chars.Span.Slice(position, length);
    }

    public ReadOnlySpan<char> FullSpan => _chars.LimitedSpan;

    public ReadOnlySpan<char> QueryString(bool includeQuestionMark)
    {
        if (_parameterCount == 0)
        {
            return ReadOnlySpan<char>.Empty;
        }

        var starter = includeQuestionMark ? _questionMarkPosition : _questionMarkPosition + 1;
        return _chars.LimitedSpan[starter..];
    }

    public void Encode(ReadOnlySpan<char> key, ReadOnlySpan<char> value)
    {
        var required = key.Length + value.Length + 2;
        _chars.EnsureCapacity(required);
        var span = _chars.Span;
        var position = AppendQOrAmp(span, _chars.Position);

        position = QueryStringEncoder.EncodeCore(key, ref _chars, position);
        span = _chars.Span;
        span[position++] = '=';
        position = QueryStringEncoder.EncodeCore(value, ref _chars, position);

        _chars.Position = position;
        _parameterCount++;
    }

    public void EncodeSafeKeyValue(ReadOnlySpan<char> key, scoped ReadOnlySpan<char> value)
    {
        var required = key.Length + value.Length + 2;
        _chars.EnsureCapacity(required);
        var span = _chars.Span;
        var position = AppendQOrAmp(span, _chars.Position);

        key.CopyTo(span[position..]);
        position += key.Length;
        span[position++] = '=';

        value.CopyTo(span[position..]);
        position += value.Length;

        _chars.Position = position;
        _parameterCount++;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void EncodeSafeKey(ReadOnlySpan<char> key, scoped ReadOnlySpan<char> value)
    {
        var required = key.Length + value.Length + 2;
        _chars.EnsureCapacity(required);
        var span = _chars.Span;
        var position = AppendQOrAmp(span, _chars.Position);

        key.CopyTo(span[position..]);
        position += key.Length;
        span[position++] = '=';
        position = QueryStringEncoder.EncodeCore(value, ref _chars, position);

        _chars.Position = position;
        _parameterCount++;
    }

    public void WriteSafeKey<T>(ReadOnlySpan<char> key, T value) where T : INumber<T>
    {
        var maximumValueLength = QueryStringEncoder.GetMaxChars<T>();
        var required = key.Length + maximumValueLength + 2;

        _chars.EnsureCapacity(required);
        var span = _chars.Span;
        var position = AppendQOrAmp(span, _chars.Position);

        key.CopyTo(span[position..]);
        position += key.Length;
        span[position++] = '=';

        if (!value.TryFormat(span[position..], out var charsWritten, "R", CultureInfo.InvariantCulture))
        {
            throw new InvalidOperationException($"Could not format value {value} into query string for {key}");
        }

        position += charsWritten;
        _chars.Position = position;
        _parameterCount++;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int AppendQOrAmp(Span<char> span, int position)
    {
        if (_parameterCount == 0)
        {
            _questionMarkPosition = position;
            span[position++] = '?';
        }
        else
        {
            span[position++] = '&';
        }

        return position;
    }

    public void Dispose()
    {
        _chars.Dispose();
    }
}
