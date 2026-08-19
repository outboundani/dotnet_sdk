using System.Text.Json;

namespace OutboundIQ;

/// <summary>
/// Raised on a 401 or 403 response. Check that the API key is valid, active, and of type
/// <c>universal</c>.
/// </summary>
public sealed class OutboundIQAuthenticationException : OutboundIQApiException
{
    /// <summary>Initializes a new instance of the <see cref="OutboundIQAuthenticationException"/> class.</summary>
    public OutboundIQAuthenticationException()
    {
    }

    /// <summary>Initializes a new instance of the <see cref="OutboundIQAuthenticationException"/> class.</summary>
    /// <param name="message">The error message.</param>
    public OutboundIQAuthenticationException(string message)
        : base(message)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="OutboundIQAuthenticationException"/> class.</summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The underlying cause, if any.</param>
    public OutboundIQAuthenticationException(string message, Exception? innerException)
        : base(message, innerException)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="OutboundIQAuthenticationException"/> class.</summary>
    /// <param name="message">The error message.</param>
    /// <param name="status">The HTTP status code of the response.</param>
    /// <param name="rawBody">The response body as received.</param>
    /// <param name="body">The response body parsed as JSON, when it was valid JSON.</param>
    /// <param name="requestId">The value of the <c>x-request-id</c> response header.</param>
    public OutboundIQAuthenticationException(
        string message,
        int? status,
        string? rawBody,
        JsonElement? body,
        string? requestId)
        : base(message, status, rawBody, body, requestId)
    {
    }
}
