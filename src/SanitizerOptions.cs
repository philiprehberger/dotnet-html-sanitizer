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
    /// Gets the default sanitizer options.
    /// </summary>
    public static SanitizerOptions Default => new();
}
