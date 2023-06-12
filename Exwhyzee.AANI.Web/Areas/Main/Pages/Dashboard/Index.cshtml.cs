using Exwhyzee.AANI.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AANI.Web.Areas.Main.Pages.Dashboard
{
    [Microsoft.AspNetCore.Authorization.Authorize]

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
        public int MNI { get; set; }
        public int NON { get; set; }
        public int Attended { get; set; }
        public int Chapter { get; set; }
        public int Nec { get; set; }
        public int Contributor { get; set; }
        public int Events { get; set; }
        public int Closed { get; set; }
        public int Active { get; set; }
        public int Awaiting { get; set; }
        public int Papers { get; set; }
        public int Patrons { get; set; }
        public int MalePatron { get; set; }
        public int FemalePatron { get; set; }
        public int Secfamily { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var alumni = _userManager.Users.Include(x => x.SEC).Where(x => x.Email != "jinmcever@gmail.com").AsQueryable();
            var chapter = _context.Chapters.AsQueryable();
            var nec = _context.Necs.AsQueryable();
            var contributor = _context.Contributors.AsQueryable();
            var eventlist = _context.Events.AsQueryable();
            var paper = _context.Papers.AsQueryable();
            var patron =  _context.Patrons.Include(x=>x.Participant).AsQueryable();
            var subCategoryFamiliesOnSEC = _context.SubCategoryFamiliesOnSECs.AsQueryable();
            //
            AllAlumni = await alumni.CountAsync();
            Male = await alumni.Where(x => x.GenderStatus == Domain.Enums.GenderStatus.Male).CountAsync();
            Female = await alumni.Where(x => x.GenderStatus == Domain.Enums.GenderStatus.Female).CountAsync();
            MNI = await alumni.Where(x => x.MniStatus == Domain.Enums.MniStatus.MNI).CountAsync();
            NON = await alumni.Where(x => x.MniStatus == Domain.Enums.MniStatus.NONE).CountAsync(); 
            Chapter = await chapter.CountAsync();
            Nec = await nec.CountAsync();
            Contributor = await contributor.CountAsync();
            Events = await eventlist.CountAsync();
            Active = await eventlist.Where(x => x.EventStatus == Domain.Enums.EventStatus.Active).CountAsync();
            Closed = await eventlist.Where(x => x.EventStatus == Domain.Enums.EventStatus.Closed).CountAsync();
            Awaiting = await eventlist.Where(x => x.EventStatus == Domain.Enums.EventStatus.Awaiting).CountAsync();

            Papers = await paper.CountAsync();
            Patrons = await patron.CountAsync();
            MalePatron = await patron.Where(x => x.Participant.GenderStatus == Domain.Enums.GenderStatus.Male).CountAsync();
            FemalePatron = await patron.Where(x => x.Participant.GenderStatus == Domain.Enums.GenderStatus.Female).CountAsync();
            Secfamily = await subCategoryFamiliesOnSEC.CountAsync();
            return Page();
        }
    }
}
