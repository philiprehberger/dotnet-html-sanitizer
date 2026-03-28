namespace Philiprehberger.HtmlSanitizer.Tests;

public class SanitizationReportTests
{
    [Fact]
    public void SanitizeWithReport_ReportsRemovedTags()
    {
        var result = Sanitizer.SanitizeWithReport("<b>ok</b><div>removed</div>");

        Assert.Equal("<b>ok</b>removed", result.SanitizedHtml);
        Assert.Contains(result.Report.Removals, r => r.Kind == "tag" && r.Name == "div");
    }

    [Fact]
    public void SanitizeWithReport_ReportsRemovedAttributes()
    {
        var result = Sanitizer.SanitizeWithReport("<p onclick=\"evil()\">text</p>");

        Assert.Equal("<p>text</p>", result.SanitizedHtml);
        Assert.Contains(result.Report.Removals, r => r.Kind == "attribute" && r.Name == "onclick");
    }

    [Fact]
    public void SanitizeWithReport_ReportsRemovedUrls()
    {
        var result = Sanitizer.SanitizeWithReport("<a href=\"javascript:alert('xss')\">link</a>");

        Assert.Equal("<a>link</a>", result.SanitizedHtml);
        Assert.Contains(result.Report.Removals, r => r.Kind == "url" && r.Name == "javascript:alert('xss')");
    }

    [Fact]
    public void SanitizeWithReport_WithCleanHtml_ReturnsEmptyReport()
    {
        var result = Sanitizer.SanitizeWithReport("<b>safe</b>");

        Assert.Equal("<b>safe</b>", result.SanitizedHtml);
        Assert.Empty(result.Report.Removals);
    }

    [Fact]
    public void SanitizeWithReport_WithEmptyString_ReturnsEmptyResult()
    {
        var result = Sanitizer.SanitizeWithReport("");

        Assert.Equal("", result.SanitizedHtml);
        Assert.Empty(result.Report.Removals);
    }

    [Fact]
    public void SanitizeWithReport_WithNull_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => Sanitizer.SanitizeWithReport(null!));
    }
}
