using Ganss.Xss;

namespace Web.Infrastructure.Security;

public sealed class HtmlContentSanitizer
{
    private readonly HtmlSanitizer _sanitizer;

    public HtmlContentSanitizer()
    {
        _sanitizer = new HtmlSanitizer();
        _sanitizer.AllowedTags.Clear();
        _sanitizer.AllowedTags.UnionWith([
            "a", "blockquote", "br", "code", "em", "h1", "h2", "h3", "h4", "h5", "h6",
            "hr", "i", "li", "ol", "p", "pre", "strong", "s", "u", "ul"
        ]);
        _sanitizer.AllowedAttributes.Clear();
        _sanitizer.AllowedAttributes.UnionWith(["href", "title"]);
        _sanitizer.AllowedSchemes.Clear();
        _sanitizer.AllowedSchemes.Add("https");
        _sanitizer.AllowedSchemes.Add("http");
    }

    public string Sanitize(string? html) => _sanitizer.Sanitize(html ?? string.Empty);
}
