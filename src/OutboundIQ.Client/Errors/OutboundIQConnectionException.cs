namespace OutboundIQ;

/// <summary>
/// Raised when a request could not be completed: a network failure, a DNS failure, or the
/// per-attempt timeout elapsing. No response was received.
/// </summary>
/// <remarks>
/// A request cancelled through the caller's own <see cref="CancellationToken"/> throws
/// <see cref="OperationCanceledException"/> instead, not this type.
/// </remarks>
public sealed class OutboundIQConnectionException : OutboundIQException
{
    /// <summary>Initializes a new instance of the <see cref="OutboundIQConnectionException"/> class.</summary>
    public OutboundIQConnectionException()
    {
    }

    /// <summary>Initializes a new instance of the <see cref="OutboundIQConnectionException"/> class.</summary>
    /// <param name="message">The error message.</param>
    public OutboundIQConnectionException(string message)
        : base(message)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="OutboundIQConnectionException"/> class.</summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The underlying cause, if any.</param>
    public OutboundIQConnectionException(string message, Exception? innerException)
        : base(message, innerException)
    {
    }
}
