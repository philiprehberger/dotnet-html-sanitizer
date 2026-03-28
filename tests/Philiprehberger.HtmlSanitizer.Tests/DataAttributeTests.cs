namespace Philiprehberger.HtmlSanitizer.Tests;

public class DataAttributeTests
{
    [Fact]
    public void Sanitize_WithAllowedDataAttributes_KeepsPermittedDataAttributes()
    {
        var options = new SanitizerOptions
        {
            AllowedDataAttributes = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "data-id", "data-name" }
        };

        var result = Sanitizer.Sanitize("<p data-id=\"42\" data-name=\"test\" data-evil=\"bad\">text</p>", options);
        Assert.Equal("<p data-id=\"42\" data-name=\"test\">text</p>", result);
    }

    [Fact]
    public void Sanitize_WithEmptyAllowedDataAttributes_RemovesAllDataAttributes()
    {
        var options = new SanitizerOptions();

        var result = Sanitizer.Sanitize("<p data-id=\"42\">text</p>", options);
        Assert.Equal("<p>text</p>", result);
    }

    [Fact]
    public void Sanitize_WithDataAttributes_DoesNotAffectRegularAttributes()
    {
        var options = new SanitizerOptions
        {
            AllowedDataAttributes = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "data-id" }
        };

        var result = Sanitizer.Sanitize("<a href=\"https://example.com\" data-id=\"1\">link</a>", options);
        Assert.Equal("<a href=\"https://example.com\" data-id=\"1\">link</a>", result);
    }

    [Fact]
    public void Sanitize_WithDataAttributes_IsCaseInsensitive()
    {
        var options = new SanitizerOptions
        {
            AllowedDataAttributes = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "data-ID" }
        };

        var result = Sanitizer.Sanitize("<p data-id=\"42\">text</p>", options);
        Assert.Equal("<p data-id=\"42\">text</p>", result);
    }
}
