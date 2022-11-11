using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Exwhyzee.AANI.Web.Data;
using Exwhyzee.AANI.Domain.Models;

namespace Exwhyzee.AANI.Web.Areas.Main.Pages.BlogNews
{
    

    public class IndexModel : PageModel
    {
        private readonly Exwhyzee.AANI.Web.Data.AaniDbContext _context;

        public IndexModel(Exwhyzee.AANI.Web.Data.AaniDbContext context)
        {
            _context = context;
        }

        public IList<Blog> Blogs { get;set; }

        public async Task OnGetAsync()
        {
            Blogs = await _context.Blogs.OrderByDescending(d=>d.Date).ToListAsync();
        }
    }
}
