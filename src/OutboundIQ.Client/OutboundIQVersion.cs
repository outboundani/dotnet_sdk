using System.Reflection;

namespace OutboundIQ;

/// <summary>
/// The SDK version, as reported in the <c>User-Agent</c> header of every request.
/// </summary>
internal static class OutboundIQVersion
{
    /// <summary>
    /// The SemVer version of this assembly, with any build metadata removed.
    /// </summary>
    public static string Value { get; } = Compute();

    private static string Compute()
    {
        var informational = typeof(OutboundIQVersion).Assembly
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;

        if (string.IsNullOrEmpty(informational))
        {
            return "0.0.0";
        }

        // The SDK appends "+<commit sha>" via SourceRevisionId. Keep it in the assembly for
        // diagnostics, but never put it on the wire.
        var plus = informational.IndexOf('+');
        return plus < 0 ? informational : informational[..plus];
    }
}
