using System.Text.Json;

namespace OutboundIQ;

/// <summary>
/// Raised when the API returns a non-success status code.
/// </summary>
/// <remarks>
/// <see cref="OutboundIQAuthenticationException"/> and <see cref="OutboundIQRateLimitException"/>
/// both derive from this type, so catching it handles every HTTP error from the API.
/// </remarks>
public class OutboundIQApiException : OutboundIQException
{
    /// <summary>Initializes a new instance of the <see cref="OutboundIQApiException"/> class.</summary>
    public OutboundIQApiException()
    {
    }

    /// <summary>Initializes a new instance of the <see cref="OutboundIQApiException"/> class.</summary>
    /// <param name="message">The error message.</param>
    public OutboundIQApiException(string message)
        : base(message)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="OutboundIQApiException"/> class.</summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The underlying cause, if any.</param>
    public OutboundIQApiException(string message, Exception? innerException)
        : base(message, innerException)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="OutboundIQApiException"/> class.</summary>
    /// <param name="message">The error message.</param>
    /// <param name="status">The HTTP status code of the response.</param>
    /// <param name="rawBody">The response body as received.</param>
    /// <param name="body">The response body parsed as JSON, when it was valid JSON.</param>
    /// <param name="requestId">The value of the <c>x-request-id</c> response header.</param>
    /// <param name="innerException">The underlying cause, if any.</param>
    public OutboundIQApiException(
        string message,
        int? status,
        string? rawBody,
        JsonElement? body,
        string? requestId,
        Exception? innerException = null)
        : base(message, status, rawBody, body, requestId, innerException)
    {
    }
}
