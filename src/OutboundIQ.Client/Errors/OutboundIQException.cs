using System.Text.Json;

namespace OutboundIQ;

/// <summary>
/// Base class for every error raised by the outboundIQ SDK.
/// </summary>
/// <remarks>
/// Errors that came from an HTTP response carry <see cref="Status"/>, <see cref="Body"/>, and
/// <see cref="RequestId"/>. Errors raised before a request was sent — a missing API key, for
/// example — carry none of them.
/// </remarks>
public class OutboundIQException : Exception
{
    /// <summary>Initializes a new instance of the <see cref="OutboundIQException"/> class.</summary>
    public OutboundIQException()
    {
    }

    /// <summary>Initializes a new instance of the <see cref="OutboundIQException"/> class.</summary>
    /// <param name="message">The error message.</param>
    public OutboundIQException(string message)
        : base(message)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="OutboundIQException"/> class.</summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The underlying cause, if any.</param>
    public OutboundIQException(string message, Exception? innerException)
        : base(message, innerException)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="OutboundIQException"/> class.</summary>
    /// <param name="message">The error message.</param>
    /// <param name="status">The HTTP status code of the response.</param>
    /// <param name="rawBody">The response body as received.</param>
    /// <param name="body">The response body parsed as JSON, when it was valid JSON.</param>
    /// <param name="requestId">The value of the <c>x-request-id</c> response header.</param>
    /// <param name="innerException">The underlying cause, if any.</param>
    public OutboundIQException(
        string message,
        int? status,
        string? rawBody,
        JsonElement? body,
        string? requestId,
        Exception? innerException = null)
        : base(message, innerException)
    {
        Status = status;
        RawBody = rawBody;
        Body = body;
        RequestId = requestId;
    }

    /// <summary>
    /// The HTTP status code, when this error came from a response.
    /// </summary>
    public int? Status { get; }

    /// <summary>
    /// The response body exactly as received. May be JSON, plain text, or <see langword="null"/>
    /// when the response had no body.
    /// </summary>
    public string? RawBody { get; }

    /// <summary>
    /// The response body parsed as JSON, or <see langword="null"/> when the body was empty or was
    /// not valid JSON. Inspect <see cref="RawBody"/> in that case.
    /// </summary>
    public JsonElement? Body { get; }

    /// <summary>
    /// The value of the <c>x-request-id</c> response header. Quote this when contacting
    /// outboundIQ support about a failed request.
    /// </summary>
    public string? RequestId { get; }
}
