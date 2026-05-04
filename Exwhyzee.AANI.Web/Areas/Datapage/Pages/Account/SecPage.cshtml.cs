using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Exwhyzee.AANI.Web.Areas.Datapage.Pages.Account
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin,MNI,AANI")]

    public class SecPageModel : PageModel
    {
        private readonly Exwhyzee.AANI.Web.Data.AaniDbContext _context;
        private readonly UserManager<Participant> _userManager;

        public SecPageModel(Exwhyzee.AANI.Web.Data.AaniDbContext context, UserManager<Participant> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IList<SEC> SEC { get;set; } = new List<SEC>();
        public Dictionary<long, Participant> MonitorGenerals { get; set; } = new();
        public async Task OnGetAsync()
        {
            SEC = await _context.SECs
          .AsNoTracking()
          .ToListAsync();

            MonitorGenerals = await _userManager.Users 
                .Where(x => x.IsMonotorGeneral && x.SECId != null)
                .AsNoTracking()
                .GroupBy(x => x.SECId!.Value)
                .ToDictionaryAsync(g => g.Key, g => g.First());
        }
    }
}
