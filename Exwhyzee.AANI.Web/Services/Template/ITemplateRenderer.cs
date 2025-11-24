using System.Text.RegularExpressions;

namespace Exwhyzee.AANI.Web.Services.Template
{
    public interface ITemplateRenderer
    {
        // Renders a template body with the provided token dictionary.
        // Tokens are of the form {{token}}.
        string Render(string templateBody, IDictionary<string, string?> tokens);
    }

    public class TemplateRenderer : ITemplateRenderer
    {
        // Simple token replacement with regex. Unknown tokens replaced with empty string.
        // Example token format: {{fullname}} or {{fullname  }} with whitespace.
        private static readonly Regex TokenRegex = new Regex(@"\{\{\s*(?<key>[a-zA-Z0-9_.\-]+)\s*\}\}", RegexOptions.Compiled);

        public string Render(string templateBody, IDictionary<string, string?> tokens)
        {
            if (string.IsNullOrEmpty(templateBody)) return string.Empty;

            string result = TokenRegex.Replace(templateBody, m =>
            {
                var key = m.Groups["key"].Value;
                if (tokens != null && tokens.TryGetValue(key, out var val))
                {
                    return val ?? string.Empty;
                }
                // not found => empty string
                return string.Empty;
            });

            return result;
        }
    }
}