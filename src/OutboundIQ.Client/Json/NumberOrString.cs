using System.Globalization;
using System.Text.Json.Serialization;

namespace OutboundIQ;

/// <summary>
/// A value the API accepts, or returns, as either a JSON number or a JSON string.
/// </summary>
/// <remarks>
/// <para>
/// A few fields are loosely typed on the wire — <c>total_dial_attempts</c> on a dial record, and
/// <c>id</c> and <c>status</c> on an NRM ANI. This type preserves whichever form was used, so a
/// value read as a string is written back as a string.
/// </para>
/// <para>
/// Implicit conversions mean you rarely name the type directly:
/// <c>new DialRecord { TotalDialAttempts = 3 }</c> and <c>= "3"</c> both compile.
/// </para>
/// </remarks>
[JsonConverter(typeof(NumberOrStringConverter))]
public readonly struct NumberOrString : IEquatable<NumberOrString>
{
    // The exact token text is kept so numbers round-trip losslessly, including values outside the
    // range of double.
    private readonly string? _text;
    private readonly bool _isNumber;

    private NumberOrString(string? text, bool isNumber)
    {
        _text = text;
        _isNumber = isNumber;
    }

    /// <summary>Whether this holds a value at all. <see langword="false"/> for the default.</summary>
    public bool HasValue => _text is not null;

    /// <summary>Whether the value was carried as a JSON number rather than a JSON string.</summary>
    public bool IsNumber => _isNumber;

    /// <summary>The value as text, or <see langword="null"/> when it was a number or absent.</summary>
    public string? Text => _isNumber ? null : _text;

    /// <summary>The value as a number, or <see langword="null"/> when it was a string or absent.</summary>
    public double? Number =>
        _isNumber && double.TryParse(_text, NumberStyles.Float, CultureInfo.InvariantCulture, out var value)
            ? value
            : null;

    /// <summary>Creates a value carried as a JSON number.</summary>
    /// <param name="value">The number.</param>
    public static NumberOrString FromNumber(long value) =>
        new(value.ToString(CultureInfo.InvariantCulture), isNumber: true);

    /// <summary>Creates a value carried as a JSON number.</summary>
    /// <param name="value">The number.</param>
    public static NumberOrString FromNumber(double value) =>
        new(value.ToString("R", CultureInfo.InvariantCulture), isNumber: true);

    /// <summary>Creates a value carried as a JSON string.</summary>
    /// <param name="value">The string.</param>
    public static NumberOrString FromString(string? value) =>
        value is null ? default : new(value, isNumber: false);

    /// <summary>Converts an <see cref="int"/> to a JSON number value.</summary>
    /// <param name="value">The number.</param>
    public static implicit operator NumberOrString(int value) => FromNumber(value);

    /// <summary>Converts a <see cref="long"/> to a JSON number value.</summary>
    /// <param name="value">The number.</param>
    public static implicit operator NumberOrString(long value) => FromNumber(value);

    /// <summary>Converts a <see cref="double"/> to a JSON number value.</summary>
    /// <param name="value">The number.</param>
    public static implicit operator NumberOrString(double value) => FromNumber(value);

    /// <summary>Converts a <see cref="string"/> to a JSON string value.</summary>
    /// <param name="value">The string.</param>
    public static implicit operator NumberOrString(string? value) => FromString(value);

    /// <summary>
    /// Reads the value as a 64-bit integer, whether it arrived as a number or as a string.
    /// </summary>
    /// <param name="value">The parsed value, when this returns <see langword="true"/>.</param>
    /// <returns><see langword="true"/> when the value parses as an integer.</returns>
    public bool TryGetInt64(out long value) =>
        long.TryParse(_text, NumberStyles.Integer, CultureInfo.InvariantCulture, out value);

    /// <inheritdoc/>
    public bool Equals(NumberOrString other) =>
        _isNumber == other._isNumber && string.Equals(_text, other._text, StringComparison.Ordinal);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is NumberOrString other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(_text, _isNumber);

    /// <summary>Compares two values for equality.</summary>
    /// <param name="left">The first value.</param>
    /// <param name="right">The second value.</param>
    public static bool operator ==(NumberOrString left, NumberOrString right) => left.Equals(right);

    /// <summary>Compares two values for inequality.</summary>
    /// <param name="left">The first value.</param>
    /// <param name="right">The second value.</param>
    public static bool operator !=(NumberOrString left, NumberOrString right) => !left.Equals(right);

    /// <summary>Returns the underlying text, or an empty string when there is no value.</summary>
    public override string ToString() => _text ?? string.Empty;

    // Used by the converter, which needs to rebuild a value from a raw JSON token.
    internal static NumberOrString FromRawNumber(string text) => new(text, isNumber: true);

    internal string? RawText => _text;
}
