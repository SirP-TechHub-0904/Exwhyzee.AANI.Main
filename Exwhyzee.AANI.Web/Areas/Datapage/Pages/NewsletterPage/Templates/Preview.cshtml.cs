using System.Collections.Generic;
using System.Threading.Tasks;
using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;
using Exwhyzee.AANI.Web.Services.Template;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AANI.Web.Areas.Datapage.Pages.NewsletterPage.Templates
{
    // This page acts like a small API for previewing templates or ad-hoc subject/body content.
    public class PreviewModel : PageModel
    {
        private readonly AaniDbContext _context;
        private readonly ITemplateRenderer _renderer;

        public PreviewModel(AaniDbContext context, ITemplateRenderer renderer)
        {
            _context = context;
            _renderer = renderer;
        }

        // GET handler: ?handler=Template&id=123
        public async Task<IActionResult> OnGetTemplateAsync(long id)
        {
            var t = await _context.MessageTemplates.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (t == null) return NotFound();

            // sample tokens for preview
            var tokens = GetSampleTokens();

            var subject = string.Empty;
             

            var body = _renderer.Render(t.Body ?? string.Empty, tokens);

            return new JsonResult(new { subject, body });
        }

        // POST handler: ?handler=Render
        // Expects form fields: subject (optional) and body (required)
        // Renders using sample tokens and returns result
        public IActionResult OnPostRender()
        {
            var subject = Request.Form["subject"].ToString() ?? string.Empty;
            var body = Request.Form["body"].ToString() ?? string.Empty;

            var tokens = GetSampleTokens();

            var renderedSubject = string.IsNullOrEmpty(subject) ? string.Empty : _renderer.Render(subject, tokens);
            var renderedBody = _renderer.Render(body, tokens);

            return new JsonResult(new { subject = renderedSubject, body = renderedBody });
        }

        private Dictionary<string, string?> GetSampleTokens()
        {
            return new Dictionary<string, string?>()
            {
                ["fullname"] = "John Doe",
                ["name"] = "John",
                ["firstname"] = "John",
                ["surname"] = "Doe",
                ["email"] = "john@example.com",
                ["phone"] = "+2348012345678",
                ["chapter"] = "FCT Chapter",
                ["sec"] = "SEC 10 (2024)",
                ["year"] = "2024"
            };
        }
    }
}