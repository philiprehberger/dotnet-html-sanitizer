# Philiprehberger.HtmlSanitizer

[![CI](https://github.com/philiprehberger/dotnet-html-sanitizer/actions/workflows/ci.yml/badge.svg)](https://github.com/philiprehberger/dotnet-html-sanitizer/actions/workflows/ci.yml)
[![NuGet](https://img.shields.io/nuget/v/Philiprehberger.HtmlSanitizer.svg)](https://www.nuget.org/packages/Philiprehberger.HtmlSanitizer)
[![License](https://img.shields.io/github/license/philiprehberger/dotnet-html-sanitizer)](LICENSE)
[![Sponsor](https://img.shields.io/badge/sponsor-GitHub%20Sponsors-ec6cb9)](https://github.com/sponsors/philiprehberger)

Whitelist-based HTML sanitizer for XSS prevention with configurable allowed tags, attributes, and URL schemes.

## Installation

```bash
dotnet add package Philiprehberger.HtmlSanitizer
```

## Usage

### Basic Sanitization

```csharp
using Philiprehberger.HtmlSanitizer;

var clean = Sanitizer.Sanitize("<b>Hello</b><script>alert('xss')</script>");
// "<b>Hello</b>"

var stripped = Sanitizer.StripAll("<p>Hello <b>world</b></p>");
// "Hello world"
```

### Custom Options

```csharp
var options = new SanitizerOptions
{
    AllowedTags = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "p", "b", "i" },
    AllowedAttributes = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "class" },
    AllowedSchemes = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "https" }
};

var clean = Sanitizer.Sanitize("<p class=\"info\"><a href=\"http://evil.com\">link</a></p>", options);
// "<p class=\"info\">link</p>"
```

### URL Scheme Filtering

```csharp
// Default allows http, https, mailto
var safe = Sanitizer.Sanitize("<a href=\"javascript:alert('xss')\">click</a>");
// "<a>click</a>"

var mailto = Sanitizer.Sanitize("<a href=\"mailto:test@example.com\">email</a>");
// "<a href=\"mailto:test@example.com\">email</a>"
```

## API

### `Sanitizer`

| Method | Description |
|--------|-------------|
| `Sanitize(string html)` | Sanitize HTML using default options |
| `Sanitize(string html, SanitizerOptions options)` | Sanitize HTML using custom options |
| `StripAll(string html)` | Remove all HTML tags, returning plain text |

### `SanitizerOptions`

| Property | Type | Description |
|----------|------|-------------|
| `AllowedTags` | `HashSet<string>` | Tags permitted in output (default: b, i, em, strong, a, p, br, ul, ol, li, h1-h6, blockquote, pre, code) |
| `AllowedAttributes` | `HashSet<string>` | Attributes permitted in output (default: href, src, alt, title) |
| `AllowedSchemes` | `HashSet<string>` | URL schemes permitted in href/src (default: http, https, mailto) |

### Default Allowed Tags

`b`, `i`, `em`, `strong`, `a`, `p`, `br`, `ul`, `ol`, `li`, `h1`, `h2`, `h3`, `h4`, `h5`, `h6`, `blockquote`, `pre`, `code`

## Development

```bash
dotnet build src/Philiprehberger.HtmlSanitizer.csproj --configuration Release
```

## License

MIT
