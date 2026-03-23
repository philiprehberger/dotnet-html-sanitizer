using System.Text;

namespace Philiprehberger.HtmlSanitizer;

/// <summary>
/// The type of HTML token produced by the tokenizer.
/// </summary>
internal enum TokenType
{
    /// <summary>Plain text content.</summary>
    Text,
    /// <summary>An opening tag (e.g., &lt;p&gt; or &lt;a href="..."&gt;).</summary>
    OpenTag,
    /// <summary>A closing tag (e.g., &lt;/p&gt;).</summary>
    CloseTag,
    /// <summary>A self-closing tag (e.g., &lt;br /&gt;).</summary>
    SelfClosingTag
}

/// <summary>
/// Represents a parsed HTML token.
/// </summary>
internal sealed class HtmlToken
{
    /// <summary>Gets the type of this token.</summary>
    public required TokenType Type { get; init; }

    /// <summary>Gets the tag name (for tag tokens) or text content (for text tokens).</summary>
    public required string Content { get; init; }

    /// <summary>Gets the parsed attributes for tag tokens.</summary>
    public Dictionary<string, string> Attributes { get; init; } = [];
}

/// <summary>
/// Simple state machine tokenizer that parses HTML into tokens.
/// Not a full HTML parser — handles opening tags with attributes, closing tags,
/// self-closing tags, and text nodes. Strips script and style tag contents entirely.
/// </summary>
internal static class HtmlTokenizer
{
    private static readonly HashSet<string> StrippedTags = new(StringComparer.OrdinalIgnoreCase)
    {
        "script", "style"
    };

    /// <summary>
    /// Tokenizes an HTML string into a sequence of <see cref="HtmlToken"/> instances.
    /// </summary>
    /// <param name="html">The HTML string to tokenize.</param>
    /// <returns>An enumerable of HTML tokens.</returns>
    public static IEnumerable<HtmlToken> Tokenize(string html)
    {
        var pos = 0;
        var length = html.Length;

        while (pos < length)
        {
            if (html[pos] == '<')
            {
                // Check for comment
                if (pos + 3 < length && html[pos + 1] == '!' && html[pos + 2] == '-' && html[pos + 3] == '-')
                {
                    var commentEnd = html.IndexOf("-->", pos + 4, StringComparison.Ordinal);
                    pos = commentEnd < 0 ? length : commentEnd + 3;
                    continue;
                }

                var tagEnd = FindTagEnd(html, pos);
                if (tagEnd < 0)
                {
                    // Malformed tag, treat rest as text
                    yield return new HtmlToken { Type = TokenType.Text, Content = html[pos..] };
                    yield break;
                }

                var tagContent = html[(pos + 1)..tagEnd];
                pos = tagEnd + 1;

                if (tagContent.Length == 0)
                {
                    continue;
                }

                // Closing tag
                if (tagContent[0] == '/')
                {
                    var tagName = tagContent[1..].Trim().TrimEnd('>');
                    yield return new HtmlToken { Type = TokenType.CloseTag, Content = tagName.ToLowerInvariant() };
                    continue;
                }

                // Parse tag
                var token = ParseOpenTag(tagContent);

                // Handle stripped tags — skip all content until closing tag
                if (StrippedTags.Contains(token.Content))
                {
                    var closingTag = $"</{token.Content}>";
                    var closingPos = html.IndexOf(closingTag, pos, StringComparison.OrdinalIgnoreCase);
                    pos = closingPos < 0 ? length : closingPos + closingTag.Length;
                    continue;
                }

                yield return token;
            }
            else
            {
                var nextTag = html.IndexOf('<', pos);
                var end = nextTag < 0 ? length : nextTag;
                var text = html[pos..end];
                if (text.Length > 0)
                {
                    yield return new HtmlToken { Type = TokenType.Text, Content = text };
                }
                pos = end;
            }
        }
    }

    private static int FindTagEnd(string html, int start)
    {
        var inQuote = false;
        var quoteChar = '\0';

        for (var i = start + 1; i < html.Length; i++)
        {
            var c = html[i];

            if (inQuote)
            {
                if (c == quoteChar)
                {
                    inQuote = false;
                }
                continue;
            }

            if (c == '"' || c == '\'')
            {
                inQuote = true;
                quoteChar = c;
                continue;
            }

            if (c == '>')
            {
                return i;
            }
        }

        return -1;
    }

    private static HtmlToken ParseOpenTag(string tagContent)
    {
        var isSelfClosing = tagContent.EndsWith('/');
        if (isSelfClosing)
        {
            tagContent = tagContent[..^1].TrimEnd();
        }

        var spaceIndex = IndexOfWhitespace(tagContent);
        string tagName;
        var attributes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        if (spaceIndex < 0)
        {
            tagName = tagContent.ToLowerInvariant();
        }
        else
        {
            tagName = tagContent[..spaceIndex].ToLowerInvariant();
            ParseAttributes(tagContent[spaceIndex..], attributes);
        }

        return new HtmlToken
        {
            Type = isSelfClosing ? TokenType.SelfClosingTag : TokenType.OpenTag,
            Content = tagName,
            Attributes = attributes
        };
    }

    private static int IndexOfWhitespace(string s)
    {
        for (var i = 0; i < s.Length; i++)
        {
            if (char.IsWhiteSpace(s[i]))
            {
                return i;
            }
        }
        return -1;
    }

    private static void ParseAttributes(string attrString, Dictionary<string, string> attributes)
    {
        var pos = 0;
        var length = attrString.Length;

        while (pos < length)
        {
            // Skip whitespace
            while (pos < length && char.IsWhiteSpace(attrString[pos]))
            {
                pos++;
            }

            if (pos >= length)
            {
                break;
            }

            // Read attribute name
            var nameStart = pos;
            while (pos < length && attrString[pos] != '=' && !char.IsWhiteSpace(attrString[pos]))
            {
                pos++;
            }

            var name = attrString[nameStart..pos].Trim();
            if (name.Length == 0)
            {
                break;
            }

            // Skip whitespace
            while (pos < length && char.IsWhiteSpace(attrString[pos]))
            {
                pos++;
            }

            // Check for = sign
            if (pos >= length || attrString[pos] != '=')
            {
                attributes[name.ToLowerInvariant()] = string.Empty;
                continue;
            }

            pos++; // skip =

            // Skip whitespace
            while (pos < length && char.IsWhiteSpace(attrString[pos]))
            {
                pos++;
            }

            if (pos >= length)
            {
                attributes[name.ToLowerInvariant()] = string.Empty;
                break;
            }

            // Read value
            string value;
            if (attrString[pos] == '"' || attrString[pos] == '\'')
            {
                var quote = attrString[pos];
                pos++;
                var valueStart = pos;
                while (pos < length && attrString[pos] != quote)
                {
                    pos++;
                }
                value = attrString[valueStart..pos];
                if (pos < length)
                {
                    pos++; // skip closing quote
                }
            }
            else
            {
                var valueStart = pos;
                while (pos < length && !char.IsWhiteSpace(attrString[pos]))
                {
                    pos++;
                }
                value = attrString[valueStart..pos];
            }

            attributes[name.ToLowerInvariant()] = value;
        }
    }
}
