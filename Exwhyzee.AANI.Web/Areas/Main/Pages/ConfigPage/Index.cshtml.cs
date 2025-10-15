using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;

namespace Exwhyzee.AANI.Web.Areas.Main.Pages.ConfigPage
{
    public class IndexModel : PageModel
    {
        private readonly Exwhyzee.AANI.Web.Data.AaniDbContext _context;

        public IndexModel(Exwhyzee.AANI.Web.Data.AaniDbContext context)
        {
            _context = context;
        }

        public StylingConfig StylingConfig { get;set; }

        public async Task OnGetAsync()
        {
            StylingConfig = await _context.StylingConfigs.FirstOrDefaultAsync();

            if (StylingConfig == null)
            {
                StylingConfig nx = new StylingConfig();
                nx.CSSStyle = "";
                nx.Config = "";
                _context.StylingConfigs.Add(nx);
                await _context.SaveChangesAsync();
            }
            
             
        }
    }
}
