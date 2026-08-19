namespace OutboundIQ;

/// <summary>
/// The headers outboundIQ sends on every webhook delivery.
/// </summary>
public static class WebhookHeaders
{
    /// <summary>
    /// The signature header, <c>x-outboundiq-signature</c>. Its value is <c>sha256=</c> followed
    /// by a lowercase hex HMAC-SHA256 digest of the raw request body.
    /// </summary>
    public const string Signature = "x-outboundiq-signature";

    /// <summary>
    /// The event type header, <c>x-outboundiq-event</c>. Currently always
    /// <see cref="WebhookEventTypes.DialBatch"/>.
    /// </summary>
    public const string Event = "x-outboundiq-event";

    /// <summary>
    /// The delivery identifier header, <c>x-outboundiq-delivery-id</c>. Use it as an idempotency
    /// key: a failed delivery is retried once.
    /// </summary>
    public const string DeliveryId = "x-outboundiq-delivery-id";
}

/// <summary>
/// The webhook event types outboundIQ sends.
/// </summary>
public static class WebhookEventTypes
{
    /// <summary>A batch of 1 to 100 dial events.</summary>
    public const string DialBatch = "dial.batch";
}
