using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;
using Microsoft.AspNetCore.Identity;

namespace Exwhyzee.AANI.Web.Areas.Main.Pages.ParticipantPage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<Participant> _userManager;

        public IndexModel(UserManager<Participant> userManager)
        {
            _userManager = userManager;
        }

        public IQueryable<Participant>? Participants { get;set; }

        public async Task OnGetAsync()
        {
            Participants = _userManager.Users.Include(x=>x.SEC).AsQueryable();
            //await Task.Run();
        }
    }
}
