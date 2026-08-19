using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace OutboundIQ;

/// <summary>
/// Response from <c>POST /assignment</c>.
/// </summary>
/// <remarks>
/// <see cref="Ani"/> is populated only when <see cref="Success"/> is <see langword="true"/>. The
/// compiler knows this, so a null check after testing <see cref="Success"/> is unnecessary.
/// </remarks>
public sealed record AssignmentResponse
{
    /// <summary>Whether an ANI was assigned.</summary>
    [JsonPropertyName("success")]
    [MemberNotNullWhen(true, nameof(Ani))]
    public bool Success { get; init; }

    /// <summary>The assigned ANI. Present when <see cref="Success"/> is <see langword="true"/>.</summary>
    [JsonPropertyName("ani")]
    public string? Ani { get; init; }

    /// <summary>A human-readable description of the outcome.</summary>
    [JsonPropertyName("message")]
    public string Message { get; init; } = string.Empty;
}
