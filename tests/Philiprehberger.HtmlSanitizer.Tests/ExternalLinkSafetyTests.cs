namespace Philiprehberger.HtmlSanitizer.Tests;

public class ExternalLinkSafetyTests
{
    [Fact]
    public void Sanitize_WithForceExternalLinkSafety_AddsTargetAndRel()
    {
        var options = new SanitizerOptions
        {
            ForceExternalLinkSafety = true
        };

        var result = Sanitizer.Sanitize("<a href=\"https://example.com\">link</a>", options);
        Assert.Equal("<a href=\"https://example.com\" target=\"_blank\" rel=\"noopener noreferrer\">link</a>", result);
    }

    [Fact]
    public void Sanitize_WithForceExternalLinkSafety_DoesNotAffectNonAnchorTags()
    {
        var options = new SanitizerOptions
        {
            ForceExternalLinkSafety = true
        };

        var result = Sanitizer.Sanitize("<p>text</p>", options);
        Assert.Equal("<p>text</p>", result);
    }

    [Fact]
    public void Sanitize_WithForceExternalLinkSafety_DoesNotDuplicateExistingTargetAndRel()
    {
        var options = new SanitizerOptions
        {
            ForceExternalLinkSafety = true,
            AllowedAttributes = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "href", "target", "rel" }
        };

        var result = Sanitizer.Sanitize("<a href=\"https://example.com\" target=\"_self\" rel=\"nofollow\">link</a>", options);
        Assert.Contains("target=\"_self\"", result);
        Assert.Contains("rel=\"nofollow\"", result);
        Assert.DoesNotContain("target=\"_blank\"", result);
        Assert.DoesNotContain("noopener noreferrer", result);
    }

    [Fact]
    public void Sanitize_WithoutForceExternalLinkSafety_DoesNotAddAttributes()
    {
        var options = new SanitizerOptions
        {
            ForceExternalLinkSafety = false
        };

        var result = Sanitizer.Sanitize("<a href=\"https://example.com\">link</a>", options);
        Assert.Equal("<a href=\"https://example.com\">link</a>", result);
    }
}
