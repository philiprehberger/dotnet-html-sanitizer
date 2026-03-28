namespace Philiprehberger.HtmlSanitizer.Tests;

public class CssClassWhitelistTests
{
    [Fact]
    public void Sanitize_WithAllowedClasses_KeepsOnlyPermittedClasses()
    {
        var options = new SanitizerOptions
        {
            AllowedAttributes = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "class" },
            AllowedClasses = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "safe", "info" }
        };

        var result = Sanitizer.Sanitize("<p class=\"safe danger info\">text</p>", options);
        Assert.Equal("<p class=\"safe info\">text</p>", result);
    }

    [Fact]
    public void Sanitize_WithAllowedClasses_RemovesClassAttributeWhenNoClassesMatch()
    {
        var options = new SanitizerOptions
        {
            AllowedAttributes = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "class" },
            AllowedClasses = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "safe" }
        };

        var result = Sanitizer.Sanitize("<p class=\"danger evil\">text</p>", options);
        Assert.Equal("<p>text</p>", result);
    }

    [Fact]
    public void Sanitize_WithEmptyAllowedClasses_PassesAllClassesThrough()
    {
        var options = new SanitizerOptions
        {
            AllowedAttributes = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "class" },
            AllowedClasses = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        };

        var result = Sanitizer.Sanitize("<p class=\"anything goes\">text</p>", options);
        Assert.Equal("<p class=\"anything goes\">text</p>", result);
    }

    [Fact]
    public void Sanitize_WithAllowedClasses_IsCaseInsensitive()
    {
        var options = new SanitizerOptions
        {
            AllowedAttributes = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "class" },
            AllowedClasses = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "Safe" }
        };

        var result = Sanitizer.Sanitize("<p class=\"safe SAFE Safe\">text</p>", options);
        Assert.Equal("<p class=\"safe SAFE Safe\">text</p>", result);
    }
}
