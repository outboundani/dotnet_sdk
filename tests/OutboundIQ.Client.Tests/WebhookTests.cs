namespace OutboundIQ.Tests;

/// <summary>
/// Webhook signature verification.
/// </summary>
/// <remarks>
/// The secret, payload, and digest below are byte-identical to the ones in the TypeScript SDK's
/// <c>test/webhooks.test.ts</c>. Sharing the vector is what proves the two SDKs accept exactly the
/// same signatures — a divergence here would mean a delivery that one SDK trusts and the other
/// rejects.
/// </remarks>
public class WebhookTests
{
    private const string Secret = "whsec_test_secret";

    private const string RawBody =
        """{"event":"dial.batch","deliveryId":"3f6d1c1e-9d1f-4a5b-8f3a-1c2d3e4f5a6b","deliveredAt":"2026-08-18T18:00:00Z","dials":[{"companySlug":"acme","callId":"call-1","callDirection":"Outbound","timestamp":"2026-08-18T17:59:58Z","ani":"2345678901","phone":"5559876543","campaign":"Q3 Push","campaignInternalId":"c-9","agent":"Jane Doe","disposition":"Sale","dispositionId":"d-1","contact":true,"success":true,"isSystemDispo":false,"totalDialAttempts":3}]}""";

    private const string ValidHex = "5af1f8a303ae628009001363a2bb70f6e2c58e296edde15ae81e7dcf4efd89ce";

    [Fact]
    public void Accepts_a_valid_signature_with_the_prefix() =>
        Assert.True(OutboundIQWebhooks.VerifySignature(RawBody, $"sha256={ValidHex}", Secret));

    [Fact]
    public void Accepts_a_valid_signature_without_the_prefix() =>
        Assert.True(OutboundIQWebhooks.VerifySignature(RawBody, ValidHex, Secret));

    [Fact]
    public void Accepts_the_raw_body_as_bytes() =>
        Assert.True(OutboundIQWebhooks.VerifySignature(
            System.Text.Encoding.UTF8.GetBytes(RawBody), $"sha256={ValidHex}", Secret));

    [Fact]
    public void Accepts_an_uppercase_hex_digest() =>
        Assert.True(OutboundIQWebhooks.VerifySignature(RawBody, ValidHex.ToUpperInvariant(), Secret));

    [Fact]
    public void Accepts_a_signature_with_surrounding_whitespace() =>
        Assert.True(OutboundIQWebhooks.VerifySignature(RawBody, $"  sha256={ValidHex}  ", Secret));

    [Fact]
    public void Rejects_a_tampered_payload() =>
        Assert.False(OutboundIQWebhooks.VerifySignature(
            RawBody.Replace("Sale", "No Sale", StringComparison.Ordinal), $"sha256={ValidHex}", Secret));

    [Fact]
    public void Rejects_a_signature_made_with_the_wrong_secret() =>
        Assert.False(OutboundIQWebhooks.VerifySignature(RawBody, $"sha256={ValidHex}", "whsec_wrong"));

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    [InlineData("sha256=")]
    [InlineData("sha256=nothex!")]
    [InlineData("sha256=abc")]
    [InlineData("zz")]
    public void Malformed_signatures_return_false_rather_than_throwing(string? signature) =>
        Assert.False(OutboundIQWebhooks.VerifySignature(RawBody, signature, Secret));

    [Fact]
    public void Rejects_a_digest_of_the_wrong_length() =>
        Assert.False(OutboundIQWebhooks.VerifySignature(RawBody, ValidHex[..62], Secret));

    [Fact]
    public void ConstructEvent_parses_a_verified_payload()
    {
        var payload = OutboundIQWebhooks.ConstructEvent(RawBody, $"sha256={ValidHex}", Secret);

        Assert.Equal(WebhookEventTypes.DialBatch, payload.Event);
        Assert.Equal("3f6d1c1e-9d1f-4a5b-8f3a-1c2d3e4f5a6b", payload.DeliveryId);
        Assert.Equal("2026-08-18T18:00:00Z", payload.DeliveredAt);

        var dial = Assert.Single(payload.Dials);
        Assert.Equal("acme", dial.CompanySlug);
        Assert.Equal("call-1", dial.CallId);
        Assert.Equal(CallDirections.Outbound, dial.CallDirection);
        Assert.Equal("2345678901", dial.Ani);
        Assert.Equal("5559876543", dial.Phone);
        Assert.Equal("Q3 Push", dial.Campaign);
        Assert.Equal("c-9", dial.CampaignInternalId);
        Assert.Equal("Jane Doe", dial.Agent);
        Assert.Equal("Sale", dial.Disposition);
        Assert.Equal("d-1", dial.DispositionId);
        Assert.True(dial.Contact);
        Assert.True(dial.Success);
        Assert.False(dial.IsSystemDispo);
        Assert.Equal(3, dial.TotalDialAttempts);
    }

    [Fact]
    public void ConstructEvent_throws_on_a_bad_signature()
    {
        var exception = Assert.Throws<OutboundIQWebhookVerificationException>(
            () => OutboundIQWebhooks.ConstructEvent(RawBody, "sha256=deadbeef", Secret));

        Assert.Contains("did not match", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void ConstructEvent_throws_when_the_body_is_not_json()
    {
        const string body = "not json";
        var signature = SignatureFor(body);

        var exception = Assert.Throws<OutboundIQWebhookVerificationException>(
            () => OutboundIQWebhooks.ConstructEvent(body, signature, Secret));

        Assert.Contains("not valid JSON", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void Verification_requires_a_secret() =>
        Assert.Throws<ArgumentNullException>(() => OutboundIQWebhooks.VerifySignature(RawBody, ValidHex, null!));

    [Fact]
    public void Header_names_match_what_the_platform_sends()
    {
        Assert.Equal("x-outboundiq-signature", WebhookHeaders.Signature);
        Assert.Equal("x-outboundiq-event", WebhookHeaders.Event);
        Assert.Equal("x-outboundiq-delivery-id", WebhookHeaders.DeliveryId);
        Assert.Equal("dial.batch", WebhookEventTypes.DialBatch);
    }

    private static string SignatureFor(string body)
    {
        var digest = System.Security.Cryptography.HMACSHA256.HashData(
            System.Text.Encoding.UTF8.GetBytes(Secret),
            System.Text.Encoding.UTF8.GetBytes(body));

        return "sha256=" + Convert.ToHexString(digest).ToLowerInvariant();
    }
}
