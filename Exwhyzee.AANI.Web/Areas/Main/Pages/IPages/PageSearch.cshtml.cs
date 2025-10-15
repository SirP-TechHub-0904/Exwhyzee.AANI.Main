using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Exwhyzee.AANI.Web.Areas.Main.Pages.IPages
{
    public class PageSearchModel : PageModel
    {
        private readonly AaniDbContext _context;

        public List<SearchResultDto> Results { get; set; } = new();
        public int TotalMatches { get; set; }

        public PageSearchModel(AaniDbContext context)
        {
            _context = context;
        }

        public void OnGet(string? query)
        {
            if (string.IsNullOrWhiteSpace(query)) return;

            var normalizedKeyword = query.Trim().ToLowerInvariant();

            // Load categories with navigation properties
            var categories = _context.PageCategories
                .Include(pc => pc.WebPages)
                    .ThenInclude(wp => wp.PageSections)
                        .ThenInclude(ps => ps.PageSectionLists)
                .ToList();

            foreach (var cat in categories)
            {
                SearchObject(cat, normalizedKeyword, query, $"Category: {cat.Title}", Results);

                foreach (var wp in cat.WebPages)
                {
                    SearchObject(wp, normalizedKeyword, query, $"Category: {cat.Title} > Page: {wp.Title}", Results);

                    foreach (var section in wp.PageSections)
                    {
                        SearchObject(section, normalizedKeyword, query, $"Category: {cat.Title} > Page: {wp.Title} > Section: {section.Title}", Results);

                        foreach (var list in section.PageSectionLists)
                        {
                            SearchObject(list, normalizedKeyword, query, $"Category: {cat.Title} > Page: {wp.Title} > Section: {section.Title} > List: {list.Title}", Results);
                        }
                    }
                }
            }

            TotalMatches = Results.Count;
        }

        // NOTE: searchWord is the original query string, not normalized
        private void SearchObject(object obj, string normalizedKeyword, string searchWord, string path, List<SearchResultDto> results)
        {
            if (string.IsNullOrWhiteSpace(normalizedKeyword)) return;

            var props = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var prop in props)
            {
                if (prop.PropertyType == typeof(string))
                {
                    var rawValue = prop.GetValue(obj) as string;
                    if (!string.IsNullOrWhiteSpace(rawValue))
                    {
                        var normalizedValue = rawValue.ToLowerInvariant();

                        int matchIndex = normalizedValue.IndexOf(normalizedKeyword);
                        if (matchIndex >= 0)
                        {
                            // Get snippet: 20 chars before and after
                            int snippetStart = Math.Max(0, matchIndex - 20);
                            int snippetEnd = Math.Min(rawValue.Length, matchIndex + normalizedKeyword.Length + 20);

                            string snippet = rawValue.Substring(snippetStart, snippetEnd - snippetStart);

                            // Highlight all occurrences of the searchWord (case-insensitive, preserve original casing)
                            string highlightedSnippet = System.Text.RegularExpressions.Regex.Replace(
                                snippet,
                                System.Text.RegularExpressions.Regex.Escape(searchWord ?? ""),
                                "<span style=\"background-color:yellow\">$0</span>",
                                System.Text.RegularExpressions.RegexOptions.IgnoreCase
                            );

                            if (snippetStart > 0) highlightedSnippet = "..." + highlightedSnippet;
                            if (snippetEnd < rawValue.Length) highlightedSnippet += "...";

                            results.Add(new SearchResultDto
                            {
                                Path = GetEditUrl(obj),
                                Type = obj.GetType().Name,
                                Field = prop.Name,
                                Value = highlightedSnippet,
                                MatchedText = normalizedKeyword
                            });
                        }
                    }
                }
            }
        }

        private string GetEditUrl(object obj)
        {
            return obj switch
            {
                PageCategory c => $"/Main/IPages/Category/Edit?id={c.Id}",
                WebPage wp => $"/Main/IPages/Main/Edit?id={wp.Id}",
                PageSection ps => $"/Main/IPages/Section/Edit?id={ps.Id}",
                PageSectionList li => $"/Main/IPages/List/Edit?id={li.Id}",
                _ => "#"
            };
        }

        public class SearchResultDto
        {
            public string Path { get; set; }
            public string Type { get; set; }
            public string Field { get; set; }
            public string Value { get; set; }
            public string MatchedText { get; set; }
        }
    }
}