namespace OutboundIQ;

/// <summary>Values for <see cref="RingCxUploadOptions.DialPriority"/>.</summary>
public static class RingCxDialPriorities
{
    /// <summary>Dial as soon as possible.</summary>
    public const string Immediate = "IMMEDIATE";

    /// <summary>Dial on the first pass through the list.</summary>
    public const string FirstPass = "FIRST_PASS";

    /// <summary>Dial on the second pass through the list.</summary>
    public const string SecondPass = "SECOND_PASS";

    /// <summary>Dial on the third pass through the list.</summary>
    public const string ThirdPass = "THIRD_PASS";
}

/// <summary>Values for <see cref="RingCxUploadOptions.DuplicateHandling"/>.</summary>
public static class RingCxDuplicateHandlings
{
    /// <summary>Remove existing duplicates from this list.</summary>
    public const string RemoveFromList = "REMOVE_FROM_LIST";

    /// <summary>Keep every copy.</summary>
    public const string RetainAll = "RETAIN_ALL";

    /// <summary>Remove all existing copies across lists.</summary>
    public const string RemoveAllExisting = "REMOVE_ALL_EXISTING";
}

/// <summary>Values for <see cref="RingCxUploadOptions.ListState"/>.</summary>
public static class RingCxListStates
{
    /// <summary>The list is dialable.</summary>
    public const string Active = "ACTIVE";

    /// <summary>The list is loaded but not dialed.</summary>
    public const string Inactive = "INACTIVE";
}

/// <summary>Values for <see cref="RingCxUploadOptions.TimeZoneOption"/>.</summary>
public static class RingCxTimeZoneOptions
{
    /// <summary>Do not derive a time zone.</summary>
    public const string NotApplicable = "NOT_APPLICABLE";

    /// <summary>Derive the time zone from the area code and exchange.</summary>
    public const string NpaNxx = "NPA_NXX";

    /// <summary>Derive the time zone from the ZIP code.</summary>
    public const string ZipCode = "ZIPCODE";

    /// <summary>Use an explicitly supplied time zone.</summary>
    public const string Explicit = "EXPLICIT";
}
