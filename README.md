# OutboundIQ.Client

The official .NET SDK for the [outboundIQ](https://outboundiq.cloud) platform.

[![NuGet](https://img.shields.io/nuget/v/OutboundIQ.Client)](https://www.nuget.org/packages/OutboundIQ.Client)
[![Downloads](https://img.shields.io/nuget/dt/OutboundIQ.Client)](https://www.nuget.org/packages/OutboundIQ.Client)
[![CI](https://github.com/outboundani/dotnet_sdk/actions/workflows/ci.yml/badge.svg)](https://github.com/outboundani/dotnet_sdk/actions/workflows/ci.yml)
[![License: MIT](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

- Typed coverage of the platform APIs: Assignment, Dials, Custom Dialer Integration, ANI Planner, NRM, and Live Feed
- Webhook signature verification with typed `dial.batch` payloads
- Zero dependencies
- Targets .NET 8, .NET 9, and .NET 10; trim- and Native AOT-compatible
- Automatic retries with exponential backoff, safe by default

The [API reference](https://docs.outboundiq.cloud) remains the source of truth.

## Install

```
dotnet add package OutboundIQ.Client
```

## Quick start

```csharp
using OutboundIQ;

using var client = new OutboundIQClient(Environment.GetEnvironmentVariable("OUTBOUNDIQ_API_KEY")!);

var result = await client.Assignment.NextAsync(new AssignmentRequest
{
    ProspectPhone = "5559876543",
    ProspectZip = "90210",
});

if (result.Success)
{
    Console.WriteLine($"Dial from {result.Ani}");
}
```

## Naming

Properties use .NET conventions and map to the wire format through `JsonPropertyName`. The
underlying field names are unchanged, so anything in the API reference maps predictably:

| API field | SDK property |
| --- | --- |
| `prospect_phone` | `ProspectPhone` |
| `page_size` | `PageSize` |
| `dateStart` | `DateStart` |
| `leadPhone` | `LeadPhone` |

The API mixes snake_case and camelCase depending on the endpoint. The SDK is PascalCase throughout,
so you never have to remember which is which.

## Authentication

Every request needs a `universal` API key, generated in the outboundIQ workspace dashboard. Pass it
explicitly or set the `OUTBOUNDIQ_API_KEY` environment variable:

```csharp
using var client = new OutboundIQClient("oiq_...");

// or, with OUTBOUNDIQ_API_KEY set in the environment:
using var client = new OutboundIQClient();
```

Keep the key server-side. Do not ship it in a desktop or mobile app.

## Assignment API

Get the next ANI to dial from, one prospect at a time or in batch.

```csharp
// Single prospect
var next = await client.Assignment.NextAsync(new AssignmentRequest
{
    ProspectPhone = "5559876543",
    ProspectZip = "90210",
    DialerCampaign = "q3-outbound",
    E164 = true,
});

// Batch, one call for a whole lead list
var batch = await client.Assignment.BatchAsync(new AssignmentBatchRequest
{
    Leads =
    [
        new AssignmentBatchLead { RowId = "lead-1", ProspectPhone = "5559876543", ProspectZip = "90210" },
        new AssignmentBatchLead { RowId = "lead-2", ProspectPhone = "5551112222" },
    ],
});

foreach (var row in batch.Results)
{
    if (!string.IsNullOrEmpty(row.Error))
    {
        Console.WriteLine($"{row.RowId}: {row.Error}");
    }
    else
    {
        Console.WriteLine($"{row.RowId} -> {row.OutboundAni}");
    }
}
```

A batch can partially succeed, so check `Error` on every row.

## Dials API

Post dial records for processing and enrichment.

```csharp
await client.Dials.CreateAsync(new DialRecord
{
    CampaignId = "abc-123",
    CampaignName = "Q2 Outbound Push",
    AgentName = "Jane Doe",
    FromNumber = "5551234567",
    ToNumber = "5559876543",
    DispositionName = "Sale",
    DateTime = "2026-04-10 14:32:15",
    CallDirection = CallDirections.Outbound,
    Zip = "90210",
    SysCreatedDateOriginal = "2026-04-01",
    TotalDialAttempts = 3,
    SkillName = "Sales Tier 1",
    LeadSource = "facebook-ads",
    DialId = "550e8400-e29b-41d4-a716-446655440000",
});
```

For outbound calls `FromNumber` is the caller ID and `ToNumber` is the prospect. For inbound calls
the two are reversed.

## Custom Dialer Integration API

Sync campaigns, dispositions, and ANIs from any dialer into outboundIQ.

```csharp
await client.Custom.Campaigns.CreateAsync(new CustomCampaign
{
    Id = "c-1",
    Name = "My Campaign",
    Type = CustomCampaignTypes.Outbound,
});
await client.Custom.Campaigns.UpdateAsync(new CustomCampaignUpdate { Id = "c-1", Name = "Renamed" });
await client.Custom.Campaigns.GetAsync("c-1");
await client.Custom.Campaigns.DeleteAsync("c-1");

await client.Custom.Dispos.CreateAsync(new CustomDispo
{
    Id = "d-1",
    Name = "Sale",
    Type = CustomDispoTypes.Agent,
    Contact = true,
    Success = true,
});

await client.Custom.Anis.CreateAsync(new CustomAni
{
    CountryCode = CountryCodes.Us,
    Number = "5551234567",
    InboundGroupId = "g-1",
});
var ani = await client.Custom.Anis.GetAsync("5551234567");
```

## ANI Planner API

Analyze recent dial volume against current ANI inventory and get a provisioning recommendation.

```csharp
var plan = await client.AniPlanner.GenerateAsync(new AniPlannerGenerateRequest
{
    DateStart = "2026-04-01",
    DateEnd = "2026-04-30",
    DailyDialsTarget = AniPlannerDailyDialsTargets.Better,
    GroupBy = AniPlannerGroupBys.AreaCode,
});

if (plan.Success)
{
    Console.WriteLine($"Recommended {plan.Data.TotalProposedAnis} ANIs, currently {plan.Data.TotalCurrentAnis}");

    foreach (var region in plan.Data.RegionStats.Where(r => r.Difference > 0))
    {
        Console.WriteLine($"{region.Region}, {region.State}: add {region.Difference}");
    }
}
```

Call `GenerateAsync()` with no argument to accept every default.

## NRM API

Number reputation management: inventory, remediation, and cooling-off.

```csharp
var page = await client.Nrm.ListAnisAsync(new NrmListAnisParams { Page = 1, PageSize = 500 });

foreach (var ani in page.Data)
{
    Console.WriteLine($"{ani.Phone}: {ani.StatusLabel}, contact rate {ani.ContactRate}%");
}

await client.Nrm.RemediateAsync(new NrmRemediateRequest { Ani = "5551234567", Carrier = "123456" });
await client.Nrm.PauseAsync(new NrmPauseRequest { Ani = "5551234567", Note = "high block rate" });
await client.Nrm.ActivateAsync(new NrmActivateRequest { Ani = "5551234567", Date = "2026-05-01" });
```

`ListAnisAsync` is page-based: keep going while `CanNextPage` is true, up to `PageSize` 1000.

Remediation has a 30-day cooldown per ANI. Inside that window the API answers HTTP 200 with
`Status` set to `"within cooldown"` — a refusal, not a success, so check it:

```csharp
var outcome = await client.Nrm.RemediateAsync(new NrmRemediateRequest { Ani = "5551234567" });
if (outcome.Status == "within cooldown")
{
    // Already remediated in the last 30 days.
}
```

## Live Feed API

Push leads into a running dialer campaign.

```csharp
await client.LiveFeed.RingCx.UploadAsync(new RingCxUploadRequest
{
    CampaignId = "camp-1",
    Lead = new RingCxLead
    {
        LeadPhone = "5559876543",
        FirstName = "Jane",
        LastName = "Doe",
        Zip = "90210",
    },
    Options = new RingCxUploadOptions
    {
        DialPriority = RingCxDialPriorities.Immediate,
        DuplicateHandling = RingCxDuplicateHandlings.RemoveFromList,
    },
});
```

The caller ID is assigned by outboundIQ; any value you supply is overwritten.

## Webhooks

outboundIQ signs each delivery with an HMAC-SHA256 of the **raw** request body. Verify before
deserializing, and never re-serialize the payload first — any change in byte order or whitespace
invalidates the digest.

```csharp
using OutboundIQ;

app.MapPost("/webhooks/outboundiq", async (HttpRequest request) =>
{
    using var reader = new StreamReader(request.Body);
    var rawBody = await reader.ReadToEndAsync();
    var signature = request.Headers[WebhookHeaders.Signature].ToString();

    try
    {
        var payload = OutboundIQWebhooks.ConstructEvent(rawBody, signature, webhookSecret);

        foreach (var dial in payload.Dials)
        {
            // handle the dial
        }

        return Results.Ok();
    }
    catch (OutboundIQWebhookVerificationException)
    {
        return Results.BadRequest("bad signature");
    }
});
```

Notes:

- Deliveries are batched: each `dial.batch` payload carries 1 to 100 dial events.
- Use the `x-outboundiq-delivery-id` header (`WebhookHeaders.DeliveryId`) as an idempotency key.
  Failed deliveries are retried once; reconcile longer gaps through the Dials API.
- `OutboundIQWebhooks.VerifySignature(payload, signature, secret)` is also available if you only
  want the boolean check.

## Error handling

All errors derive from `OutboundIQException`. Responses that failed carry `Status`, `Body`, and
`RequestId`.

```csharp
try
{
    await client.Dials.CreateAsync(dial);
}
catch (OutboundIQAuthenticationException)
{
    // 401 or 403: bad, inactive, or wrong-type API key
}
catch (OutboundIQRateLimitException ex)
{
    Console.WriteLine($"retry after {ex.RetryAfter?.TotalSeconds ?? 0}s");
}
catch (OutboundIQApiException ex)
{
    Console.WriteLine($"{ex.Status}: {ex.RawBody} (request {ex.RequestId})");
}
catch (OutboundIQConnectionException ex)
{
    // network failure or timeout; no response was received
}
```

| Exception | Meaning |
| --- | --- |
| `OutboundIQAuthenticationException` | 401 or 403. Check the key is valid, active, and of type `universal`. |
| `OutboundIQRateLimitException` | 429, after retries were exhausted. `RetryAfter` is set when the API sent it. |
| `OutboundIQApiException` | Any other non-2xx response. The base type of the two above. |
| `OutboundIQConnectionException` | Network failure or timeout. No response was received. |
| `OutboundIQWebhookVerificationException` | A webhook signature did not match the payload. |

Cancelling through your own `CancellationToken` throws `OperationCanceledException`, not
`OutboundIQConnectionException`.

Quote `RequestId` when contacting support: it identifies the exact request in outboundIQ's logs.

## Retries and timeouts

The client retries with exponential backoff and jitter, up to `MaxRetries` times (default 2, so
three attempts at most):

- **GET** retries on network errors, 429, and 5xx responses.
- **POST, PUT, and DELETE** retry only on 429, where the request is known not to have been
  processed. This avoids duplicating writes like dial records when the outcome of a failed request
  is unknown.

Set `MaxRetries = 0` to disable retries. `Timeout` (default 30 seconds) applies **per attempt**, not
to the call as a whole, and surfaces as `OutboundIQConnectionException`.

## Configuration

```csharp
using var client = new OutboundIQClient(new OutboundIQClientOptions
{
    ApiKey = "oiq_...",                                  // or OUTBOUNDIQ_API_KEY
    BaseUrl = new Uri("https://api.outboundiq.cloud"),
    Timeout = TimeSpan.FromSeconds(30),                  // per attempt
    MaxRetries = 2,
    RetryBaseDelay = TimeSpan.FromMilliseconds(500),
    CaptureRequestId = true,
    UserAgentSuffix = "acme-crm/2.1",                    // identifies your integration
});
```

## Dependency injection and IHttpClientFactory

The client is stateless once constructed and safe to share across threads. **Register it as a
singleton** — creating one per request will exhaust sockets.

```csharp
builder.Services.AddSingleton<IOutboundIQClient>(_ =>
    new OutboundIQClient(builder.Configuration["OutboundIQ:ApiKey"]!));
```

To route through `IHttpClientFactory`, pass the `HttpClient` in. The SDK will not dispose it:

```csharp
builder.Services.AddHttpClient("outboundiq");

builder.Services.AddSingleton<IOutboundIQClient>(sp => new OutboundIQClient(
    new OutboundIQClientOptions { ApiKey = builder.Configuration["OutboundIQ:ApiKey"] },
    sp.GetRequiredService<IHttpClientFactory>().CreateClient("outboundiq")));
```

Set that client's `Timeout` to `Timeout.InfiniteTimeSpan` if you want `OutboundIQClientOptions.Timeout`
to be the only timeout in effect; otherwise the shorter of the two wins.

Depend on `IOutboundIQClient` rather than the concrete type where you want to substitute a fake in
tests.

## Trimming and Native AOT

The package is annotated `IsTrimmable` and `IsAotCompatible`, and serialization runs entirely
through a source-generated `JsonSerializerContext`. Publishing with `PublishTrimmed` or
`PublishAot` produces no warnings from this assembly.

## Requirements

.NET 8.0, .NET 9.0, or .NET 10.0.

## Contributing

Bug reports and pull requests are welcome.

```bash
dotnet build -c Release
dotnet test -c Release
dotnet format --verify-no-changes
```

Building requires the .NET 10 SDK — `global.json` pins the 10.0.1xx band. The test project
targets all three frameworks; run `dotnet test -f net10.0` if you do not have the .NET 8 and 9
runtimes installed locally, and let CI cover the rest.

## License

MIT
