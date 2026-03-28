namespace Philiprehberger.HtmlSanitizer;

/// <summary>
/// Configuration options for the HTML sanitizer, defining which tags, attributes,
/// and URL schemes are permitted in sanitized output.
/// </summary>
public sealed class SanitizerOptions
{
    /// <summary>
    /// Gets or sets the set of HTML tag names allowed in sanitized output.
    /// Tags not in this set are stripped (their content is preserved unless they are script/style).
    /// </summary>
    public HashSet<string> AllowedTags { get; set; } = new(StringComparer.OrdinalIgnoreCase)
    {
        "b", "i", "em", "strong", "a", "p", "br",
        "ul", "ol", "li",
        "h1", "h2", "h3", "h4", "h5", "h6",
        "blockquote", "pre", "code"
    };

    /// <summary>
    /// Gets or sets the set of HTML attribute names allowed in sanitized output.
    /// Attributes not in this set are removed from tags.
    /// </summary>
    public HashSet<string> AllowedAttributes { get; set; } = new(StringComparer.OrdinalIgnoreCase)
    {
        "href", "src", "alt", "title"
    };

    /// <summary>
    /// Gets or sets the set of URL schemes allowed in href and src attributes.
    /// Attributes with URLs using other schemes are removed.
    /// </summary>
    public HashSet<string> AllowedSchemes { get; set; } = new(StringComparer.OrdinalIgnoreCase)
    {
        "http", "https", "mailto"
    };

    /// <summary>
    /// Gets or sets the set of CSS class names allowed in the <c>class</c> attribute.
    /// When non-empty, only class values present in this set are retained; others are removed.
    /// When empty, all class values are passed through (if <c>class</c> is in <see cref="AllowedAttributes"/>).
    /// </summary>
    public HashSet<string> AllowedClasses { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Gets or sets the set of allowed <c>data-*</c> attribute names.
    /// When non-empty, only <c>data-*</c> attributes whose full name (e.g. <c>data-id</c>) is in this set are retained.
    /// When empty, no <c>data-*</c> attributes are allowed unless they are explicitly listed in <see cref="AllowedAttributes"/>.
    /// </summary>
    public HashSet<string> AllowedDataAttributes { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Gets or sets a value indicating whether external link safety attributes are automatically
    /// added to all <c>&lt;a&gt;</c> tags. When <see langword="true"/>, <c>target="_blank"</c> and
    /// <c>rel="noopener noreferrer"</c> are appended to every anchor tag in the output.
    /// </summary>
    public bool ForceExternalLinkSafety { get; set; }

    /// <summary>
    /// Gets the default sanitizer options.
    /// </summary>
    public static SanitizerOptions Default => new();
}
