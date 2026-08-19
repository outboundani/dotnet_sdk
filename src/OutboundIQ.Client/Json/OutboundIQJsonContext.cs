using System.Text.Json.Serialization;

namespace OutboundIQ;

/// <summary>
/// The source-generated serializer metadata for every type on the wire.
/// </summary>
/// <remarks>
/// <para>
/// Using a generated context rather than reflection is what makes this package genuinely
/// trim- and AOT-safe, and is why <c>IsAotCompatible</c> holds with no suppressions.
/// </para>
/// <para>
/// <c>PropertyNamingPolicy</c> is pinned to <c>Unspecified</c> on purpose. A global policy cannot
/// work here: the API is snake_case on Assignment, Dials, Custom, and the NRM list envelope, but
/// camelCase on ANI Planner, Live Feed, NRM rows, and webhooks. Every property therefore carries
/// an explicit <c>JsonPropertyName</c>, and a test enforces that.
/// </para>
/// </remarks>
[JsonSourceGenerationOptions(
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    NumberHandling = JsonNumberHandling.AllowReadingFromString,
    PropertyNamingPolicy = JsonKnownNamingPolicy.Unspecified)]

// Assignment
[JsonSerializable(typeof(AssignmentRequest))]
[JsonSerializable(typeof(AssignmentResponse))]
[JsonSerializable(typeof(AssignmentBatchRequest))]
[JsonSerializable(typeof(AssignmentBatchResponse))]

// Dials
[JsonSerializable(typeof(DialRecord))]
[JsonSerializable(typeof(DialQueuedResponse))]

// Custom dialer integration
[JsonSerializable(typeof(CustomCampaign))]
[JsonSerializable(typeof(CustomCampaignUpdate))]
[JsonSerializable(typeof(CustomDispo))]
[JsonSerializable(typeof(CustomDispoUpdate))]
[JsonSerializable(typeof(CustomAni))]
[JsonSerializable(typeof(CustomAniUpdate))]
[JsonSerializable(typeof(CustomApiResponse))]
[JsonSerializable(typeof(GetCustomAniResponse))]
[JsonSerializable(typeof(CustomIdBody))]
[JsonSerializable(typeof(CustomNumberBody))]

// NRM
[JsonSerializable(typeof(NrmListAnisResponse))]
[JsonSerializable(typeof(NrmRemediateRequest))]
[JsonSerializable(typeof(NrmPauseRequest))]
[JsonSerializable(typeof(NrmActivateRequest))]
[JsonSerializable(typeof(NrmActionResponse))]

// ANI Planner
[JsonSerializable(typeof(AniPlannerGenerateRequest))]
[JsonSerializable(typeof(AniPlannerGenerateResponse))]

// Live Feed
[JsonSerializable(typeof(RingCxUploadRequest))]
[JsonSerializable(typeof(RingCxUploadResponse))]

// Webhooks
[JsonSerializable(typeof(DialBatchWebhookPayload))]
internal sealed partial class OutboundIQJsonContext : JsonSerializerContext;
