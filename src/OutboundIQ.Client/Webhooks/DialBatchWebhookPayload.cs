using System.Text.Json.Serialization;

namespace OutboundIQ;

/// <summary>
/// The body of a <c>dial.batch</c> webhook delivery.
/// </summary>
/// <remarks>
/// Deliveries are batched: each payload carries 1 to 100 dial events. Use
/// <see cref="DeliveryId"/> as an idempotency key — a failed delivery is retried once, then
/// dropped, so reconcile longer gaps through the Dials API.
/// </remarks>
public sealed record DialBatchWebhookPayload
{
    /// <summary>The event type. Currently always <see cref="WebhookEventTypes.DialBatch"/>.</summary>
    [JsonPropertyName("event")]
    public string? Event { get; init; }

    /// <summary>A unique identifier for this delivery. Use it to deduplicate retries.</summary>
    [JsonPropertyName("deliveryId")]
    public string? DeliveryId { get; init; }

    /// <summary>When the batch was delivered, as an ISO 8601 timestamp.</summary>
    [JsonPropertyName("deliveredAt")]
    public string? DeliveredAt { get; init; }

    /// <summary>The dials in this batch.</summary>
    [JsonPropertyName("dials")]
    public IReadOnlyList<DialEvent> Dials { get; init; } = [];
}
