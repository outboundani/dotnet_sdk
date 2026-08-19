using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace OutboundIQ;

/// <summary>
/// Response from <c>POST /ani-planner/generate</c>.
/// </summary>
/// <remarks>
/// <see cref="Data"/> is populated only when <see cref="Success"/> is <see langword="true"/>.
/// </remarks>
public sealed record AniPlannerGenerateResponse
{
    /// <summary>Whether a plan was produced.</summary>
    [JsonPropertyName("success")]
    [MemberNotNullWhen(true, nameof(Data))]
    public bool Success { get; init; }

    /// <summary>The plan. Present when <see cref="Success"/> is <see langword="true"/>.</summary>
    [JsonPropertyName("data")]
    public AniPlannerPlan? Data { get; init; }

    /// <summary>The failure reason, when <see cref="Success"/> is <see langword="false"/>.</summary>
    [JsonPropertyName("message")]
    public string? Message { get; init; }
}
