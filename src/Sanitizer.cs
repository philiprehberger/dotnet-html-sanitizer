using System.Text;
using System.Web;

namespace Philiprehberger.HtmlSanitizer;

/// <summary>
/// Provides whitelist-based HTML sanitization for XSS prevention.
/// </summary>
public static class Sanitizer
{
    /// <summary>
    /// Sanitizes the HTML string using default options.
    /// Removes disallowed tags and attributes while preserving safe content.
    /// </summary>
    /// <param name="html">The HTML string to sanitize.</param>
    /// <returns>The sanitized HTML string.</returns>
    public static string Sanitize(string html)
    {
        return Sanitize(html, SanitizerOptions.Default);
    }

    /// <summary>
    /// Sanitizes the HTML string using the specified options.
    /// Removes disallowed tags and attributes while preserving safe content.
    /// </summary>
    /// <param name="html">The HTML string to sanitize.</param>
    /// <param name="options">The sanitizer options defining allowed tags, attributes, and schemes.</param>
    /// <returns>The sanitized HTML string.</returns>
    public static string Sanitize(string html, SanitizerOptions options)
    {
        ArgumentNullException.ThrowIfNull(html);
        ArgumentNullException.ThrowIfNull(options);

        if (html.Length == 0)
        {
            return string.Empty;
        }

        var sb = new StringBuilder(html.Length);
        var tokens = HtmlTokenizer.Tokenize(html);

        foreach (var token in tokens)
        {
            switch (token.Type)
            {
                case TokenType.Text:
                    sb.Append(token.Content);
                    break;

                case TokenType.OpenTag:
                    if (options.AllowedTags.Contains(token.Content))
                    {
                        AppendOpenTag(sb, token, options);
                    }
                    break;

                case TokenType.SelfClosingTag:
                    if (options.AllowedTags.Contains(token.Content))
                    {
                        AppendSelfClosingTag(sb, token, options);
                    }
                    break;

                case TokenType.CloseTag:
                    if (options.AllowedTags.Contains(token.Content))
                    {
                        sb.Append($"</{token.Content}>");
                    }
                    break;
            }
        }

        return sb.ToString();
    }

    /// <summary>
    /// Strips all HTML tags from the string, returning only the text content.
    /// </summary>
    /// <param name="html">The HTML string to strip.</param>
    /// <returns>The plain text content without any HTML tags.</returns>
    public static string StripAll(string html)
    {
        ArgumentNullException.ThrowIfNull(html);

        if (html.Length == 0)
        {
            return string.Empty;
        }

        var sb = new StringBuilder(html.Length);
        var tokens = HtmlTokenizer.Tokenize(html);

        foreach (var token in tokens)
        {
            if (token.Type == TokenType.Text)
            {
                sb.Append(token.Content);
            }
        }

        return sb.ToString();
    }

    private static void AppendOpenTag(
        StringBuilder sb,
        HtmlToken token,
        SanitizerOptions options)
    {
        sb.Append('<');
        sb.Append(token.Content);

        AppendFilteredAttributes(sb, token, options);

        sb.Append('>');
    }

    private static void AppendSelfClosingTag(
        StringBuilder sb,
        HtmlToken token,
        SanitizerOptions options)
    {
        sb.Append('<');
        sb.Append(token.Content);

        AppendFilteredAttributes(sb, token, options);

        sb.Append(" />");
    }

    private static void AppendFilteredAttributes(
        StringBuilder sb,
        HtmlToken token,
        SanitizerOptions options)
    {
        foreach (var (name, value) in token.Attributes)
        {
            if (!options.AllowedAttributes.Contains(name))
            {
                continue;
            }

            // Validate URL schemes for href and src attributes
            if (IsUrlAttribute(name) && !IsAllowedUrl(value, options))
            {
                continue;
            }

            sb.Append(' ');
            sb.Append(name);
            sb.Append("=\"");
            sb.Append(HttpUtility.HtmlAttributeEncode(value));
            sb.Append('"');
        }
    }

    private static bool IsUrlAttribute(string attributeName)
    {
        return string.Equals(attributeName, "href", StringComparison.OrdinalIgnoreCase)
            || string.Equals(attributeName, "src", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsAllowedUrl(string url, SanitizerOptions options)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return false;
        }

        // Relative URLs are allowed
        if (url.StartsWith('/') || url.StartsWith('#'))
        {
            return true;
        }

        // Check for scheme
        var colonIndex = url.IndexOf(':');
        if (colonIndex < 0)
        {
            return true; // No scheme, relative URL
        }

        var scheme = url[..colonIndex];
        return options.AllowedSchemes.Contains(scheme);
    }
}
