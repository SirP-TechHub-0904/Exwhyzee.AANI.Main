﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;
using Exwhyzee.AANI.Domain.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AANI.Web.Areas.Main.Pages.PaperPage.PaperList
{
    [Microsoft.AspNetCore.Authorization.Authorize]

    public class CreateModel : PageModel
    {
        private readonly Exwhyzee.AANI.Web.Data.AaniDbContext _context;
        private readonly UserManager<Participant> _userManager;

        public CreateModel(Exwhyzee.AANI.Web.Data.AaniDbContext context, UserManager<Participant> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult OnGet()
        {
            var partaccount = _userManager.Users.Include(x => x.SEC).Where(x => x.MniStatus == Domain.Enums.MniStatus.MNI).AsQueryable();
            var secoutput = partaccount.Select(x => new ParticipantDropdownDto
            {
                Id = x.Id,
                Fullname =  x.Surname + " " + x.FirstName + " " + x.OtherName + "(SEC " + x.SEC.Number + "- " + x.SEC.Year + ")"
            });
            ViewData["ParticipantId"] = new SelectList(secoutput, "Id", "Fullname");
            ViewData["EventId"] = new SelectList(_context.Events, "Id", "Title");
        ViewData["PaperCategoryId"] = new SelectList(_context.paperCategories, "Id", "Title");

            return Page();
        }

        [BindProperty]
        public Paper Paper { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            

            _context.Papers.Add(Paper);
            await _context.SaveChangesAsync();
            TempData["aasuccess"] = "created successfully";

            return RedirectToPage("/PaperPage/Category/Details", new { id = Paper.PaperCategoryId });
        }
    }
}
