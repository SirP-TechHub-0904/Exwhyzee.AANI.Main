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

namespace Exwhyzee.AANI.Web.Areas.Admin.Pages.Dashboard
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]

    public class IndexModel : PageModel
    {
        private readonly UserManager<Participant> _userManager;
        private readonly Exwhyzee.AANI.Web.Data.AaniDbContext _context;

        public IndexModel(UserManager<Participant> userManager, Data.AaniDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public int AllAlumni { get; set; }
        public int Male { get; set; }
        public int Female { get; set; }
        public int Alive { get; set; }
        public int Dead { get; set; }
        public int Active { get; set; }
        public string DateofBirthList { get; set; }

        public List<Chapter> Chapters { get; set; }
        public List<SEC> SECs { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            var alumni = _userManager.Users.Include(x => x.SEC).Where(x => x.MniStatus == Domain.Enums.MniStatus.MNI).AsQueryable();
            Chapters = await _context.Chapters.ToListAsync();
            SECs = await _context.SECs.ToListAsync();

            AllAlumni = await alumni.CountAsync();
            Male = await alumni.Where(x => x.GenderStatus == Domain.Enums.GenderStatus.Male).CountAsync();
            Female = await alumni.Where(x => x.GenderStatus == Domain.Enums.GenderStatus.Female).CountAsync();
            Alive = await alumni.Where(x => x.AliveStatus == Domain.Enums.AliveStatus.Alive).CountAsync();
            Dead = await alumni.Where(x => x.AliveStatus == Domain.Enums.AliveStatus.Dead).CountAsync();
            Active = await alumni.Where(x => x.ActiveStatus == Domain.Enums.ActiveStatus.Active).CountAsync();
            var listdob = from s in _userManager.Users.Include(x => x.SEC).Where(x => x.MniStatus == Domain.Enums.MniStatus.MNI)
                   .OrderByDescending(x => x.DOB)
                                .Where(ob => ob.DOB.Date == DateTime.UtcNow.Date)
                          select s;

            foreach (var listx in listdob)
            {
                DateofBirthList = DateofBirthList + "<span>++HAPPY BIRTHDAY " + listx.Fullname + " (SEC" + listx.SEC.Number + ")</span>";

            }
            if (listdob.Count() > 0)
            {
                TempData["checkifexist"] = "exist";
            }
            return Page();
        }
    }
}










