using OutboundIQ;

// A read-mostly tour of the SDK, safe to run against production.
//
//   OUTBOUNDIQ_API_KEY=oiq_... dotnet run --project samples/OutboundIQ.Client.Samples
//
// This deliberately does not post dial records, create campaigns, or change ANI state.

var apiKey = Environment.GetEnvironmentVariable(OutboundIQClientOptions.ApiKeyEnvironmentVariable);

if (string.IsNullOrWhiteSpace(apiKey))
{
    Console.Error.WriteLine($"Set {OutboundIQClientOptions.ApiKeyEnvironmentVariable} to a universal API key first.");
    return 1;
}

using var client = new OutboundIQClient(new OutboundIQClientOptions
{
    ApiKey = apiKey,
    UserAgentSuffix = "outboundiq-dotnet-sample",
});

Console.WriteLine($"outboundIQ .NET SDK {OutboundIQClient.Version}");
Console.WriteLine();

try
{
    // 1. Ask for the next ANI to dial a prospect from.
    var assignment = await client.Assignment.NextAsync(new AssignmentRequest
    {
        ProspectPhone = "5559876543",
        ProspectZip = "90210",
    });

    Console.WriteLine(assignment.Success
        ? $"Assignment: dial from {assignment.Ani}"
        : $"Assignment declined: {assignment.Message}");

    // 2. Read the first page of the ANI inventory.
    var inventory = await client.Nrm.ListAnisAsync(new NrmListAnisParams { Page = 1, PageSize = 5 });

    Console.WriteLine();
    Console.WriteLine($"NRM inventory: {inventory.TotalAnis} ANIs across {inventory.TotalPages} pages");

    foreach (var ani in inventory.Data)
    {
        Console.WriteLine($"  {ani.Phone,-14} {ani.StatusLabel,-12} contact rate {ani.ContactRate}%");
    }

    return 0;
}
catch (OutboundIQAuthenticationException ex)
{
    Console.Error.WriteLine($"Authentication failed: {ex.Message}");
    Console.Error.WriteLine("The key must be valid, active, and of type universal.");
    return 1;
}
catch (OutboundIQApiException ex)
{
    Console.Error.WriteLine($"API error {ex.Status}: {ex.Message}");
    return 1;
}
catch (OutboundIQConnectionException ex)
{
    Console.Error.WriteLine($"Could not reach the API: {ex.Message}");
    return 1;
}
