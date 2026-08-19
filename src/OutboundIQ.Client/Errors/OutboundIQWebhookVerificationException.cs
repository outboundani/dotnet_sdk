namespace OutboundIQ;

/// <summary>
/// Raised when a webhook signature does not match the payload.
/// </summary>
/// <remarks>
/// Treat this as a rejected delivery: respond with 400 and do not process the payload.
/// </remarks>
public sealed class OutboundIQWebhookVerificationException : OutboundIQException
{
    /// <summary>Initializes a new instance of the <see cref="OutboundIQWebhookVerificationException"/> class.</summary>
    public OutboundIQWebhookVerificationException()
    {
    }

    /// <summary>Initializes a new instance of the <see cref="OutboundIQWebhookVerificationException"/> class.</summary>
    /// <param name="message">The error message.</param>
    public OutboundIQWebhookVerificationException(string message)
        : base(message)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="OutboundIQWebhookVerificationException"/> class.</summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The underlying cause, if any.</param>
    public OutboundIQWebhookVerificationException(string message, Exception? innerException)
        : base(message, innerException)
    {
    }
}
