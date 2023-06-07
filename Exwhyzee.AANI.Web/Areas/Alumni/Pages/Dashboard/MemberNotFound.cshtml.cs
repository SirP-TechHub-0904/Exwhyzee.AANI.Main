using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;

namespace Exwhyzee.AANI.Web.Areas.Alumni.Pages.Dashboard
{
    public class MemberNotFoundModel : PageModel
    {
        private readonly Exwhyzee.AANI.Web.Data.AaniDbContext _context;

        public MemberNotFoundModel(Exwhyzee.AANI.Web.Data.AaniDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            ViewData["StateId"] = new SelectList(_context.States, "StateName", "StateName");

            return Page();
        }
        [BindProperty]
        public string Day { get; set; }

        [BindProperty]
        public string Month { get; set; }
        [BindProperty]
        public MemberNotRecorded MemberNotRecorded { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            
                MemberNotRecorded.DOB =  Day+"/"+Month+"/" +2000.ToString();
             
            _context.MemberNotRecorded.Add(MemberNotRecorded);
            await _context.SaveChangesAsync();
            TempData["response"] = "Your Submission is Under Review. You will get a call and an email. Once your Data is verified. Thanks";
            return RedirectToPage("./Response");
        }
    }
}
