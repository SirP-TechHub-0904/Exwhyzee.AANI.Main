using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AANI.Web.Areas.Datapage.Pages.OfficeManage
{
    public class IndexModel : PageModel
    {
        private readonly AaniDbContext _context;

        public IndexModel(AaniDbContext context)
        {
            _context = context;
        }

        public List<OfficeCategory> OfficeCategories { get; set; } = new();

        [BindProperty]
        public OfficeCategory OfficeCategoryModel { get; set; }
        [BindProperty]
        public Office OfficeModel { get; set; }
        [BindProperty]
        public long? SelectedCategoryId { get; set; }
        [BindProperty]
        public long? EditOfficeId { get; set; }
        [BindProperty]
        public long? EditCategoryId { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            OfficeCategories = await _context.OfficeCategories
                .Include(c => c.Offices)
                .OrderBy(c => c.Name)
                .ToListAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAddCategoryAsync()
        {
            if (!string.IsNullOrWhiteSpace(OfficeCategoryModel.Name))
            {
                _context.OfficeCategories.Add(new OfficeCategory { Name = OfficeCategoryModel.Name });
                await _context.SaveChangesAsync();
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEditCategoryAsync()
        {
            if (EditCategoryId.HasValue)
            {
                var cat = await _context.OfficeCategories.FindAsync(EditCategoryId.Value);
                if (cat != null && !string.IsNullOrWhiteSpace(OfficeCategoryModel.Name))
                {
                    cat.Name = OfficeCategoryModel.Name;
                    await _context.SaveChangesAsync();
                }
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteCategoryAsync(long? deleteCategoryId)
        {
            if (deleteCategoryId.HasValue)
            {
                var cat = await _context.OfficeCategories
                    .Include(c => c.Offices)
                    .FirstOrDefaultAsync(c => c.Id == deleteCategoryId.Value);
                if (cat != null)
                {
                    // Remove offices first to maintain referential integrity
                    if (cat.Offices != null)
                        _context.Offices.RemoveRange(cat.Offices);
                    _context.OfficeCategories.Remove(cat);
                    await _context.SaveChangesAsync();
                }
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostAddOfficeAsync()
        {
            if (!string.IsNullOrWhiteSpace(OfficeModel.Name) && SelectedCategoryId.HasValue)
            {
                _context.Offices.Add(new Office
                {
                    Name = OfficeModel.Name,
                    CategoryId = SelectedCategoryId
                });
                await _context.SaveChangesAsync();
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEditOfficeAsync()
        {
            if (EditOfficeId.HasValue)
            {
                var office = await _context.Offices.FindAsync(EditOfficeId.Value);
                if (office != null && !string.IsNullOrWhiteSpace(OfficeModel.Name))
                {
                    office.Name = OfficeModel.Name;
                    await _context.SaveChangesAsync();
                }
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteOfficeAsync(long? deleteOfficeId)
        {
            if (deleteOfficeId.HasValue)
            {
                var office = await _context.Offices.FindAsync(deleteOfficeId.Value);
                if (office != null)
                {
                    _context.Offices.Remove(office);
                    await _context.SaveChangesAsync();
                }
            }
            return RedirectToPage();
        }
    }
}