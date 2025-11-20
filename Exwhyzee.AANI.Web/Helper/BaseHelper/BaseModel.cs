using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;
using Exwhyzee.AANI.Web.Helper.AWS;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AANI.Web.Helper.BaseHelper
{
    public class BaseModel : IBaseModel
    {
        private readonly Exwhyzee.AANI.Web.Data.AaniDbContext _context;

        public BaseModel(AaniDbContext context)
        {
            _context = context;
        }

        public async Task<List<PageSection>> GetPageSections(string? position, string? pageset, long? id, long? secid)
        {
            var page = _context.PageSections.Include(x => x.PageSectionLists).Where(x => x.Disable == false).AsQueryable();
            if (!String.IsNullOrEmpty(position) && !String.IsNullOrEmpty(pageset))
            {
                if (position.ToLower() == "top" && pageset.ToLower() == "home")
                {
                    page = page.Where(x => x.ShowInHome == true && x.HomeSortFrom == HomeSortFrom.Top).OrderBy(x => x.HomePageSortOrder).AsQueryable();
                    return await page.ToListAsync();
                }
            }
            if (!String.IsNullOrEmpty(position))
            {
                if (position.ToLower() == "fixed")
                {
                    page = page.Where(x => x.FixedAfterFooter == true).OrderBy(x => x.HomePageSortOrder).AsQueryable();
                    return await page.ToListAsync();
                }
            }
            if (!String.IsNullOrEmpty(position) && !String.IsNullOrEmpty(pageset))
            {
                if (position.ToLower() == "bottom" && pageset.ToLower() == "home")
                {
                    page = page.Where(x => x.ShowInHome == true && x.HomeSortFrom == HomeSortFrom.Bottom).OrderBy(x => x.HomePageSortOrder).AsQueryable();
                    return await page.ToListAsync();
                }
            }

            if (String.IsNullOrEmpty(position) && id != null)
            {
                if (id > 0)
                {
                    page = page.Where(x => x.Disable == false && x.WebPageId == id).OrderBy(x => x.PageSortOrder).AsQueryable();
                    return await page.ToListAsync();
                }
            }
            if (String.IsNullOrEmpty(position) && secid != null)
            {
                if (secid > 0)
                {
                    page = page.Where(x => x.Disable == false && x.Id == secid).OrderBy(x => x.PageSortOrder).AsQueryable();
                    return await page.ToListAsync();
                }
            }
            if (!String.IsNullOrEmpty(position))
            {
                if (position.ToLower() == "footer")
                {
                    page = page.Where(x => x.FixedAfterFooter == true).OrderBy(x => x.HomePageSortOrder).AsQueryable();
                    return await page.ToListAsync();
                }
            }

            return await page.Where(x => x.WebPageId == 000).ToListAsync();
        }


        // New method that returns menu data as DTOs instead of a ViewComponent
        public async Task<MenuDataDto> GetMenuData()
        {
            var menuData = new MenuDataDto
            {
                PageCategories = await _context.PageCategories
                    .Include(x => x.WebPages)
                    .Where(x => x.Publish == true)
                    .OrderBy(x => x.MenuSortOrder)
                    .ToListAsync(),

                SinglePages = await _context.WebPages
                    .Where(x => x.Publish == true && x.ShowInMainMenu == true)
                    .ToListAsync(),

                TopPages = await _context.WebPages
                    .Where(x => x.Publish == true && x.ShowInMainTop == true)
                    .ToListAsync(),

                ExternalLinks = await _context.WebPages
                    .Where(x => x.Publish == true && x.ShowInMainMenu == true && x.EnableDirectUrl == true)
                    .ToListAsync(),

                DropdownExternalLinks = await _context.WebPages
                    .Where(x => x.Publish == true && x.ShowInMenuDropDown == false && x.EnableDirectUrl == true)
                    .ToListAsync(),
                 
            };

            return menuData;
        }
        public async Task<FooterMenuDataDto> GetFooterMenuDataAsync()
        {
            var pages = await _context.WebPages
                .Where(x => x.Publish == true && x.ShowInFooter == true && x.SecurityPage == false && x.EnableDirectUrl == false)
                .OrderBy(x => x.SortOrder)
                .ToListAsync();

            var secPages = await _context.WebPages
                .Where(x => x.Publish == true && x.ShowInFooter == true && x.SecurityPage == true)
                .OrderBy(x => x.SortOrder)
                .ToListAsync();

            var linkExternal = await _context.WebPages
                .Where(x => x.Publish == true && x.ShowInFooter == true && x.EnableDirectUrl == true)
                .ToListAsync();
            // Include related data as needed
            var settings = await _context.ContactSettings
                .Include(x => x.PhoneNumbers)
                .Include(x => x.Emails)
                .Include(x => x.Addresses)
                .Include(x => x.SocialMedia)
                .FirstOrDefaultAsync();

            return new FooterMenuDataDto
            {
                Pages = pages,
                SecPages = secPages,
                LinkExternal = linkExternal,
                Settings = settings
            };
        }

        public async Task<ConfigDto> GetConfigAsync()
        {
             
            
            var settings = await _context.StylingConfigs
                
                .FirstOrDefaultAsync();

            return new ConfigDto
            {
               Config = settings.Config,
               CSSStyle = settings.CSSStyle,
            };
        }

        public async Task<List<Executive>> GetExecutives()
        {
            var getYear = await _context.OperationYears.FirstOrDefaultAsync(oy => oy.IsActive);
            var Executive = await _context.Executives
                   .Include(e => e.ExecutivePosition)
                   .Include(e => e.Participant).ThenInclude(p => p.SEC)
                   .Where(e => e.OperationYearId == getYear.Id)
                   .OrderBy(e => e.ExecutivePosition.SortOrder)
                   .ToListAsync();

           return Executive;
        }
        public async Task<List<Event>> GetEvents()
        {
            var getYear = await _context.OperationYears.FirstOrDefaultAsync(oy => oy.IsActive);
            var Event = await _context.Events
                 .Where(e => e.OperationYearId == getYear.Id)
                 .Where(e => e.EventStatus != Domain.Enums.EventStatus.NONE)
                 .OrderByDescending(e => e.StartDate)
                 .ToListAsync();

            return Event;
        }

        public async Task<ContactSettingsModel> GetSettings()
        {
           return await _context.ContactSettings 
                .FirstOrDefaultAsync();
        }
    }

    // DTO to hold all menu-related data
    public class MenuDataDto
    {
        public List<PageCategory> PageCategories { get; set; } = new List<PageCategory>();
        public List<WebPage> SinglePages { get; set; } = new List<WebPage>();
        public List<WebPage> TopPages { get; set; } = new List<WebPage>();
        public List<WebPage> ExternalLinks { get; set; } = new List<WebPage>();
        public List<WebPage> DropdownExternalLinks { get; set; } = new List<WebPage>(); 
    }

    // DTO to hold all footer menu-related data
    public class FooterMenuDataDto
    {
        public List<WebPage> Pages { get; set; } = new List<WebPage>();
        public List<WebPage> SecPages { get; set; } = new List<WebPage>();
        public List<WebPage> LinkExternal { get; set; } = new List<WebPage>();
        public ContactSettingsModel Settings { get; set; }
    }

    public class ConfigDto
    {
        public string? CSSStyle { get; set; }
        public string? Config { get; set; }
    }
}
