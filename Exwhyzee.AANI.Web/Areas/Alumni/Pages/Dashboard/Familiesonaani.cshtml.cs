using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;

namespace Exwhyzee.AANI.Web.Areas.Alumni.Pages.Dashboard
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "MNI")]

    public class FamiliesonaaniModel : PageModel
    {
        private readonly Exwhyzee.AANI.Web.Data.AaniDbContext _context;

        public FamiliesonaaniModel(Exwhyzee.AANI.Web.Data.AaniDbContext context)
        {
            _context = context;
        }

        public IList<ParticipantFamiliesOnSEC> ParticipantFamiliesOnSEC { get;set; }

        public async Task OnGetAsync()
        {
            ParticipantFamiliesOnSEC = await _context.ParticipantFamiliesOnSECs
                .Include(p => p.Participant)
                .Include(p => p.SubCategoryFamiliesOnSEC).ToListAsync();
        }
    }
}
