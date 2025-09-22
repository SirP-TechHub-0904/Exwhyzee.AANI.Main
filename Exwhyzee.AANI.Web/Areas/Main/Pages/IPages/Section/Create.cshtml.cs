﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Helper.AWS;

namespace Exwhyzee.AANI.Web.Areas.Main.Pages.IPages.Section
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "mSuperAdmin,Editor")]

    public class CreateModel : PageModel
    {
        private readonly Exwhyzee.AANI.Web.Data.AaniDbContext _context;
        private readonly IConfiguration _config;
        private readonly IStorageService _storageService;

        public CreateModel(Exwhyzee.AANI.Web.Data.AaniDbContext context, IConfiguration config, IStorageService storageService)
        {
            _context = context;
            _config = config;
            _storageService = storageService;
        }


        [BindProperty]
        public IFormFile? imagefile { get; set; }


        [BindProperty]
        public IFormFile? videofile { get; set; }


        public IActionResult OnGet()
        {
        ViewData["WebPageId"] = new SelectList(_context.WebPages, "Id", "Title"); 

            return Page();
        }

        [BindProperty]
        public PageSection PageSection { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
             
            _context.PageSections.Add(PageSection);
            await _context.SaveChangesAsync(); TempData["success"] = "Successful";

            return RedirectToPage("./Index");
        }
    }
}
