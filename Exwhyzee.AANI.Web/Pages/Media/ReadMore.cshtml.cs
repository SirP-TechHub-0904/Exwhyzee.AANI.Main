using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;

namespace Exwhyzee.AANI.Web.Pages.Media
{
    public class ReadMoreModel : PageModel
    {
        private readonly Exwhyzee.AANI.Web.Data.AaniDbContext _context;

        public ReadMoreModel(Exwhyzee.AANI.Web.Data.AaniDbContext context)
        {
            _context = context;
        }

        public Blog Blog { get; set; }

        public async Task<IActionResult> OnGetAsync(long? id, string? title)
        {
            if (id == null)
            {
                return NotFound();
            }

            Blog = await _context.Blogs.Include(x=>x.Comments).FirstOrDefaultAsync(m => m.Id == id);

            if (Blog == null)
            {
                return NotFound();
            }
            return Page();
        }


        [BindProperty]
        public Comment Comment { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {

            _context.Comments.Add(Comment);
            await _context.SaveChangesAsync();
            TempData["aasuccess"] = "commented successfully";

            return RedirectToPage("/Media/ReadMore", new { id = Comment.BlogId });
        }
    }
}
