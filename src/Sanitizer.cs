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
            ProcessToken(sb, token, options, report: null);
        }

        return sb.ToString();
    }

    /// <summary>
    /// Sanitizes the HTML string using default options and returns a detailed report
    /// of every removed tag, attribute, and URL.
    /// </summary>
    /// <param name="html">The HTML string to sanitize.</param>
    /// <returns>A <see cref="SanitizationResult"/> containing the cleaned HTML and the removal report.</returns>
    public static SanitizationResult SanitizeWithReport(string html)
    {
        return SanitizeWithReport(html, SanitizerOptions.Default);
    }

    /// <summary>
    /// Sanitizes the HTML string using the specified options and returns a detailed report
    /// of every removed tag, attribute, and URL.
    /// </summary>
    /// <param name="html">The HTML string to sanitize.</param>
    /// <param name="options">The sanitizer options defining allowed tags, attributes, and schemes.</param>
    /// <returns>A <see cref="SanitizationResult"/> containing the cleaned HTML and the removal report.</returns>
    public static SanitizationResult SanitizeWithReport(string html, SanitizerOptions options)
    {
        ArgumentNullException.ThrowIfNull(html);
        ArgumentNullException.ThrowIfNull(options);

        var report = new SanitizationReport();

        if (html.Length == 0)
        {
            return new SanitizationResult { SanitizedHtml = string.Empty, Report = report };
        }

        var sb = new StringBuilder(html.Length);
        var tokens = HtmlTokenizer.Tokenize(html);

        foreach (var token in tokens)
        {
            ProcessToken(sb, token, options, report);
        }

        return new SanitizationResult { SanitizedHtml = sb.ToString(), Report = report };
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

    private static void ProcessToken(
        StringBuilder sb,
        HtmlToken token,
        SanitizerOptions options,
        SanitizationReport? report)
    {
        switch (token.Type)
        {
            case TokenType.Text:
                sb.Append(token.Content);
                break;

            case TokenType.OpenTag:
                if (options.AllowedTags.Contains(token.Content))
                {
                    AppendOpenTag(sb, token, options, report);
                }
                else
                {
                    report?.Removals.Add(new SanitizationRemoval
                    {
                        Kind = "tag",
                        Name = token.Content,
                        Reason = "Tag not in allowed tags list"
                    });
                }
                break;

            case TokenType.SelfClosingTag:
                if (options.AllowedTags.Contains(token.Content))
                {
                    AppendSelfClosingTag(sb, token, options, report);
                }
                else
                {
                    report?.Removals.Add(new SanitizationRemoval
                    {
                        Kind = "tag",
                        Name = token.Content,
                        Reason = "Tag not in allowed tags list"
                    });
                }
                break;

            case TokenType.CloseTag:
                if (options.AllowedTags.Contains(token.Content))
                {
                    sb.Append($"</{token.Content}>");
                }
                else
                {
                    report?.Removals.Add(new SanitizationRemoval
                    {
                        Kind = "tag",
                        Name = $"/{token.Content}",
                        Reason = "Tag not in allowed tags list"
                    });
                }
                break;
        }
    }

    private static void AppendOpenTag(
        StringBuilder sb,
        HtmlToken token,
        SanitizerOptions options,
        SanitizationReport? report)
    {
        sb.Append('<');
        sb.Append(token.Content);

        AppendFilteredAttributes(sb, token, options, report);

        if (options.ForceExternalLinkSafety
            && string.Equals(token.Content, "a", StringComparison.OrdinalIgnoreCase))
        {
            AppendExternalLinkSafetyAttributes(sb, token);
        }

        sb.Append('>');
    }

    private static void AppendSelfClosingTag(
        StringBuilder sb,
        HtmlToken token,
        SanitizerOptions options,
        SanitizationReport? report)
    {
        sb.Append('<');
        sb.Append(token.Content);

        AppendFilteredAttributes(sb, token, options, report);

        sb.Append(" />");
    }

    private static void AppendFilteredAttributes(
        StringBuilder sb,
        HtmlToken token,
        SanitizerOptions options,
        SanitizationReport? report)
    {
        foreach (var (name, value) in token.Attributes)
        {
            // Check data-* attributes against AllowedDataAttributes
            if (IsDataAttribute(name))
            {
                if (!options.AllowedDataAttributes.Contains(name))
                {
                    report?.Removals.Add(new SanitizationRemoval
                    {
                        Kind = "attribute",
                        Name = name,
                        Reason = "Data attribute not in allowed data attributes list"
                    });
                    continue;
                }

                AppendAttribute(sb, name, value);
                continue;
            }

            if (!options.AllowedAttributes.Contains(name))
            {
                report?.Removals.Add(new SanitizationRemoval
                {
                    Kind = "attribute",
                    Name = name,
                    Reason = "Attribute not in allowed attributes list"
                });
                continue;
            }

            // Validate URL schemes for href and src attributes
            if (IsUrlAttribute(name) && !IsAllowedUrl(value, options))
            {
                report?.Removals.Add(new SanitizationRemoval
                {
                    Kind = "url",
                    Name = value,
                    Reason = $"URL scheme not in allowed schemes list"
                });
                continue;
            }

            // Filter class attribute values
            if (string.Equals(name, "class", StringComparison.OrdinalIgnoreCase)
                && options.AllowedClasses.Count > 0)
            {
                var filteredClasses = FilterClasses(value, options);
                if (filteredClasses.Length == 0)
                {
                    report?.Removals.Add(new SanitizationRemoval
                    {
                        Kind = "attribute",
                        Name = "class",
                        Reason = "No class values matched the allowed classes list"
                    });
                    continue;
                }

                AppendAttribute(sb, name, filteredClasses);
                continue;
            }

            AppendAttribute(sb, name, value);
        }
    }

    private static void AppendAttribute(StringBuilder sb, string name, string value)
    {
        sb.Append(' ');
        sb.Append(name);
        sb.Append("=\"");
        sb.Append(HttpUtility.HtmlAttributeEncode(value));
        sb.Append('"');
    }

    private static void AppendExternalLinkSafetyAttributes(StringBuilder sb, HtmlToken token)
    {
        // Only append if not already present in the original attributes
        if (!token.Attributes.ContainsKey("target"))
        {
            sb.Append(" target=\"_blank\"");
        }

        if (!token.Attributes.ContainsKey("rel"))
        {
            sb.Append(" rel=\"noopener noreferrer\"");
        }
    }

    private static bool IsDataAttribute(string attributeName)
    {
        return attributeName.StartsWith("data-", StringComparison.OrdinalIgnoreCase)
            && attributeName.Length > 5;
    }

    private static string FilterClasses(string classValue, SanitizerOptions options)
    {
        var classes = classValue.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var allowed = new List<string>();

        foreach (var cls in classes)
        {
            if (options.AllowedClasses.Contains(cls))
            {
                allowed.Add(cls);
            }
        }

        return string.Join(' ', allowed);
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
