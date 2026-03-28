namespace Philiprehberger.HtmlSanitizer;

/// <summary>
/// Describes a single removal action performed during sanitization.
/// </summary>
public sealed class SanitizationRemoval
{
    /// <summary>
    /// Gets the type of element that was removed (e.g. "tag", "attribute", "url").
    /// </summary>
    public required string Kind { get; init; }

    /// <summary>
    /// Gets the name of the removed element (tag name, attribute name, or URL value).
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Gets the human-readable reason the element was removed.
    /// </summary>
    public required string Reason { get; init; }
}

/// <summary>
/// A report detailing every removal performed during a sanitization pass.
/// </summary>
public sealed class SanitizationReport
{
    /// <summary>
    /// Gets the list of removals that occurred during sanitization.
    /// </summary>
    public List<SanitizationRemoval> Removals { get; } = [];
}

/// <summary>
/// The result of <see cref="Sanitizer.SanitizeWithReport(string, SanitizerOptions)"/>,
/// containing both the cleaned HTML and a detailed report of all removals.
/// </summary>
public sealed class SanitizationResult
{
    /// <summary>
    /// Gets the sanitized HTML string.
    /// </summary>
    public required string SanitizedHtml { get; init; }

    /// <summary>
    /// Gets the report listing every tag, attribute, and URL that was removed and why.
    /// </summary>
    public required SanitizationReport Report { get; init; }
}
