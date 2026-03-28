namespace Philiprehberger.HtmlSanitizer.Tests;

public class SanitizerTests
{
    [Fact]
    public void Sanitize_WithDefaultOptions_RemovesDisallowedTags()
    {
        var result = Sanitizer.Sanitize("<b>Hello</b><script>alert('xss')</script>");
        Assert.Equal("<b>Hello</b>", result);
    }

    [Fact]
    public void Sanitize_WithEmptyString_ReturnsEmpty()
    {
        var result = Sanitizer.Sanitize("");
        Assert.Equal("", result);
    }

    [Fact]
    public void Sanitize_WithNull_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => Sanitizer.Sanitize(null!));
    }

    [Fact]
    public void StripAll_RemovesAllTags()
    {
        var result = Sanitizer.StripAll("<p>Hello <b>world</b></p>");
        Assert.Equal("Hello world", result);
    }

    [Fact]
    public void Sanitize_WithCustomOptions_AllowsOnlySpecifiedTags()
    {
        var options = new SanitizerOptions
        {
            AllowedTags = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "p" }
        };

        var result = Sanitizer.Sanitize("<p>Hello</p><b>world</b>", options);
        Assert.Equal("<p>Hello</p>world", result);
    }

    [Fact]
    public void Sanitize_WithJavascriptUrl_RemovesHref()
    {
        var result = Sanitizer.Sanitize("<a href=\"javascript:alert('xss')\">click</a>");
        Assert.Equal("<a>click</a>", result);
    }

    [Fact]
    public void Sanitize_WithAllowedScheme_KeepsHref()
    {
        var result = Sanitizer.Sanitize("<a href=\"https://example.com\">click</a>");
        Assert.Equal("<a href=\"https://example.com\">click</a>", result);
    }
}
