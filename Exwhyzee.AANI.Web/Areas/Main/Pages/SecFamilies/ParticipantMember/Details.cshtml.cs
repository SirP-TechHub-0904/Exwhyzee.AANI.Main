﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;

namespace Exwhyzee.AANI.Web.Areas.Main.Pages.SecFamilies.ParticipantMember
{
    [Microsoft.AspNetCore.Authorization.Authorize]

    public class DetailsModel : PageModel
    {
        private readonly Exwhyzee.AANI.Web.Data.AaniDbContext _context;

        public DetailsModel(Exwhyzee.AANI.Web.Data.AaniDbContext context)
        {
            _context = context;
        }

        public ParticipantFamiliesOnSEC ParticipantFamiliesOnSEC { get; set; }

        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ParticipantFamiliesOnSEC = await _context.ParticipantFamiliesOnSECs
                .Include(p => p.SubCategoryFamiliesOnSEC).FirstOrDefaultAsync(m => m.Id == id);

            if (ParticipantFamiliesOnSEC == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
