using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity;

namespace Exwhyzee.AANI.Web.Areas.Main.Pages.ParticipantPage
{
    public class DetailsModel : PageModel
    {
        private readonly UserManager<Participant> _userManager;
        
        private readonly Exwhyzee.AANI.Web.Data.AaniDbContext _context;

        public DetailsModel(
            UserManager<Participant> userManager,
            AaniDbContext context)
        {
            _userManager = userManager;
           
            _context = context;
        }


        public Participant Participant { get; set; }

        public async Task<IActionResult> OnGetAsync(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Participant = await _userManager.FindByIdAsync(id);

            if (Participant == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
