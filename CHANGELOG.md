# Changelog

## 0.1.0 (2026-03-22)

- Initial release
- Whitelist-based HTML sanitization with configurable allowed tags, attributes, and URL schemes
- Default safe set: common formatting tags, links, lists, headings, blockquote, pre, code
- `Sanitize` with default or custom options
- `StripAll` to remove all HTML tags
- Simple state machine tokenizer for tag parsing
- Script and style tags stripped entirely
