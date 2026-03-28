# Philiprehberger.HtmlSanitizer

[![CI](https://github.com/philiprehberger/dotnet-html-sanitizer/actions/workflows/ci.yml/badge.svg)](https://github.com/philiprehberger/dotnet-html-sanitizer/actions/workflows/ci.yml)
[![NuGet](https://img.shields.io/nuget/v/Philiprehberger.HtmlSanitizer.svg)](https://www.nuget.org/packages/Philiprehberger.HtmlSanitizer)
[![GitHub release](https://img.shields.io/github/v/release/philiprehberger/dotnet-html-sanitizer)](https://github.com/philiprehberger/dotnet-html-sanitizer/releases)
[![Last updated](https://img.shields.io/github/last-commit/philiprehberger/dotnet-html-sanitizer)](https://github.com/philiprehberger/dotnet-html-sanitizer/commits/main)
[![License](https://img.shields.io/github/license/philiprehberger/dotnet-html-sanitizer)](LICENSE)
[![Bug Reports](https://img.shields.io/github/issues/philiprehberger/dotnet-html-sanitizer/bug)](https://github.com/philiprehberger/dotnet-html-sanitizer/issues?q=is%3Aissue+is%3Aopen+label%3Abug)
[![Feature Requests](https://img.shields.io/github/issues/philiprehberger/dotnet-html-sanitizer/enhancement)](https://github.com/philiprehberger/dotnet-html-sanitizer/issues?q=is%3Aissue+is%3Aopen+label%3Aenhancement)
[![Sponsor](https://img.shields.io/badge/sponsor-GitHub%20Sponsors-ec6cb9)](https://github.com/sponsors/philiprehberger)

Whitelist-based HTML sanitizer for XSS prevention with configurable allowed tags, attributes, and URL schemes.

## Installation

```bash
dotnet add package Philiprehberger.HtmlSanitizer
```

## Usage

```csharp
using Philiprehberger.HtmlSanitizer;

var clean = Sanitizer.Sanitize("<b>Hello</b><script>alert('xss')</script>");
// "<b>Hello</b>"

var stripped = Sanitizer.StripAll("<p>Hello <b>world</b></p>");
// "Hello world"
```

### Custom Options

```csharp
using Philiprehberger.HtmlSanitizer;

var options = new SanitizerOptions
{
    AllowedTags = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "p", "b", "i" },
    AllowedAttributes = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "class" },
    AllowedSchemes = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "https" }
};

var clean = Sanitizer.Sanitize("<p class=\"info\"><a href=\"http://evil.com\">link</a></p>", options);
// "<p class=\"info\">link</p>"
```

### CSS Class Whitelisting

```csharp
using Philiprehberger.HtmlSanitizer;

var options = new SanitizerOptions
{
    AllowedAttributes = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "class" },
    AllowedClasses = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "safe", "info" }
};

var clean = Sanitizer.Sanitize("<p class=\"safe danger info\">text</p>", options);
// "<p class=\"safe info\">text</p>"
```

### Data Attribute Support

```csharp
using Philiprehberger.HtmlSanitizer;

var options = new SanitizerOptions
{
    AllowedDataAttributes = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "data-id", "data-name" }
};

var clean = Sanitizer.Sanitize("<p data-id=\"42\" data-evil=\"bad\">text</p>", options);
// "<p data-id=\"42\">text</p>"
```

### Sanitization Report

```csharp
using Philiprehberger.HtmlSanitizer;

var result = Sanitizer.SanitizeWithReport("<b>ok</b><div onclick=\"evil()\">text</div>");
// result.SanitizedHtml == "<b>ok</b>text"
// result.Report.Removals contains entries for the removed <div> tag and onclick attribute
foreach (var removal in result.Report.Removals)
{
    Console.WriteLine($"{removal.Kind}: {removal.Name} - {removal.Reason}");
}
```

### External Link Safety

```csharp
using Philiprehberger.HtmlSanitizer;

var options = new SanitizerOptions
{
    ForceExternalLinkSafety = true
};

var clean = Sanitizer.Sanitize("<a href=\"https://example.com\">link</a>", options);
// "<a href=\"https://example.com\" target=\"_blank\" rel=\"noopener noreferrer\">link</a>"
```

## API

### `Sanitizer`

| Method | Description |
|--------|-------------|
| `Sanitize(string html)` | Sanitize HTML using default options |
| `Sanitize(string html, SanitizerOptions options)` | Sanitize HTML using custom options |
| `SanitizeWithReport(string html)` | Sanitize HTML and return a detailed removal report |
| `SanitizeWithReport(string html, SanitizerOptions options)` | Sanitize HTML with custom options and return a removal report |
| `StripAll(string html)` | Remove all HTML tags, returning plain text |

### `SanitizerOptions`

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `AllowedTags` | `HashSet<string>` | Common formatting tags | Tags permitted in output |
| `AllowedAttributes` | `HashSet<string>` | `href, src, alt, title` | Attributes permitted in output |
| `AllowedSchemes` | `HashSet<string>` | `http, https, mailto` | URL schemes permitted in href/src |
| `AllowedClasses` | `HashSet<string>` | Empty | CSS classes permitted in class attribute; empty allows all |
| `AllowedDataAttributes` | `HashSet<string>` | Empty | Allowed `data-*` attribute names; empty allows none |
| `ForceExternalLinkSafety` | `bool` | `false` | Add `target="_blank" rel="noopener noreferrer"` to all anchor tags |

### `SanitizationResult`

| Property | Type | Description |
|----------|------|-------------|
| `SanitizedHtml` | `string` | The cleaned HTML string |
| `Report` | `SanitizationReport` | Detailed report of all removals |

### `SanitizationReport`

| Property | Type | Description |
|----------|------|-------------|
| `Removals` | `List<SanitizationRemoval>` | List of all elements removed during sanitization |

### `SanitizationRemoval`

| Property | Type | Description |
|----------|------|-------------|
| `Kind` | `string` | Type of removal: `"tag"`, `"attribute"`, or `"url"` |
| `Name` | `string` | Name of the removed element |
| `Reason` | `string` | Human-readable explanation for the removal |

## Development

```bash
dotnet build src/Philiprehberger.HtmlSanitizer.csproj --configuration Release
```

## Support

If you find this package useful, consider giving it a star on GitHub — it helps motivate continued maintenance and development.

[![LinkedIn](https://img.shields.io/badge/Philip%20Rehberger-LinkedIn-0A66C2?logo=linkedin)](https://www.linkedin.com/in/philiprehberger)
[![More packages](https://img.shields.io/badge/more-open%20source%20packages-blue)](https://philiprehberger.com/open-source-packages)

## License

[MIT](LICENSE)
