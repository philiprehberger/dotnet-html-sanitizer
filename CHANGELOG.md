# Changelog

## 0.2.0 (2026-03-28)

- Add CSS class whitelisting via `AllowedClasses` option to filter permitted class attribute values
- Add `data-*` attribute support via `AllowedDataAttributes` option for fine-grained data attribute control
- Add `SanitizeWithReport` method returning `SanitizationResult` with cleaned HTML and a detailed removal report
- Add external link safety via `ForceExternalLinkSafety` option that appends `target="_blank" rel="noopener noreferrer"` to anchor tags
- Add unit test project with xUnit covering all features
- Add GitHub issue templates, dependabot configuration, and pull request template
- Update CI workflow to include test step
- Update README with all 9 required sections and 8 badges

## 0.1.0 (2026-03-22)

- Initial release
- Whitelist-based HTML sanitization with configurable allowed tags, attributes, and URL schemes
- Default safe set: common formatting tags, links, lists, headings, blockquote, pre, code
- `Sanitize` with default or custom options
- `StripAll` to remove all HTML tags
- Simple state machine tokenizer for tag parsing
- Script and style tags stripped entirely
