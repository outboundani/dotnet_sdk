using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace OutboundIQ;

/// <summary>
/// Verifies outboundIQ webhook deliveries.
/// </summary>
/// <remarks>
/// <para>
/// The signature is an HMAC-SHA256 of the <em>raw</em> request body. Verify before deserializing,
/// and never re-serialize the payload first — any change in byte order or whitespace invalidates
/// the digest.
/// </para>
/// <para>
/// These methods need no client instance and make no network calls.
/// </para>
/// </remarks>
public static class OutboundIQWebhooks
{
    private const string SignaturePrefix = "sha256=";

    /// <summary>
    /// Checks a webhook signature against the raw request body.
    /// </summary>
    /// <param name="payload">The raw request body, exactly as received.</param>
    /// <param name="signature">
    /// The <c>x-outboundiq-signature</c> header value, with or without its <c>sha256=</c> prefix.
    /// </param>
    /// <param name="secret">The webhook signing secret.</param>
    /// <returns>
    /// <see langword="true"/> when the signature matches. A malformed or empty signature returns
    /// <see langword="false"/> rather than throwing.
    /// </returns>
    public static bool VerifySignature(ReadOnlySpan<byte> payload, string? signature, string secret)
    {
        ArgumentNullException.ThrowIfNull(secret);

        if (string.IsNullOrWhiteSpace(signature))
        {
            return false;
        }

        var hex = signature!.Trim();
        if (hex.StartsWith(SignaturePrefix, StringComparison.OrdinalIgnoreCase))
        {
            hex = hex[SignaturePrefix.Length..];
        }

        if (hex.Length == 0)
        {
            return false;
        }

        byte[] provided;
        try
        {
            provided = Convert.FromHexString(hex);
        }
        catch (FormatException)
        {
            // Odd length or non-hex characters. A bad signature is a verification failure, not an
            // exceptional condition.
            return false;
        }

        Span<byte> computed = stackalloc byte[32];
        var written = HMACSHA256.HashData(Encoding.UTF8.GetBytes(secret), payload, computed);

        return written == computed.Length && CryptographicOperations.FixedTimeEquals(computed, provided);
    }

    /// <summary>
    /// Checks a webhook signature against the raw request body.
    /// </summary>
    /// <param name="payload">The raw request body, exactly as received.</param>
    /// <param name="signature">
    /// The <c>x-outboundiq-signature</c> header value, with or without its <c>sha256=</c> prefix.
    /// </param>
    /// <param name="secret">The webhook signing secret.</param>
    /// <returns><see langword="true"/> when the signature matches.</returns>
    public static bool VerifySignature(string payload, string? signature, string secret)
    {
        ArgumentNullException.ThrowIfNull(payload);

        return VerifySignature(Encoding.UTF8.GetBytes(payload), signature, secret);
    }

    /// <summary>
    /// Verifies a webhook delivery and deserializes it.
    /// </summary>
    /// <param name="payload">The raw request body, exactly as received.</param>
    /// <param name="signature">The <c>x-outboundiq-signature</c> header value.</param>
    /// <param name="secret">The webhook signing secret.</param>
    /// <returns>The parsed payload.</returns>
    /// <exception cref="OutboundIQWebhookVerificationException">
    /// The signature did not match, or the body was not a valid <c>dial.batch</c> payload.
    /// </exception>
    public static DialBatchWebhookPayload ConstructEvent(ReadOnlySpan<byte> payload, string? signature, string secret)
    {
        if (!VerifySignature(payload, signature, secret))
        {
            throw new OutboundIQWebhookVerificationException(
                "The webhook signature did not match the payload. Verify the signing secret, and "
                + "make sure the raw request body is passed through unmodified.");
        }

        DialBatchWebhookPayload? parsed;
        try
        {
            parsed = JsonSerializer.Deserialize(payload, OutboundIQJsonContext.Default.DialBatchWebhookPayload);
        }
        catch (JsonException ex)
        {
            throw new OutboundIQWebhookVerificationException(
                "The webhook signature matched but the payload was not valid JSON.", ex);
        }

        return parsed
            ?? throw new OutboundIQWebhookVerificationException(
                "The webhook signature matched but the payload was null.");
    }

    /// <summary>
    /// Verifies a webhook delivery and deserializes it.
    /// </summary>
    /// <param name="payload">The raw request body, exactly as received.</param>
    /// <param name="signature">The <c>x-outboundiq-signature</c> header value.</param>
    /// <param name="secret">The webhook signing secret.</param>
    /// <returns>The parsed payload.</returns>
    /// <exception cref="OutboundIQWebhookVerificationException">
    /// The signature did not match, or the body was not a valid <c>dial.batch</c> payload.
    /// </exception>
    public static DialBatchWebhookPayload ConstructEvent(string payload, string? signature, string secret)
    {
        ArgumentNullException.ThrowIfNull(payload);

        return ConstructEvent(Encoding.UTF8.GetBytes(payload), signature, secret);
    }
}
