using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Exwhyzee.AANI.Web.Data;
using Exwhyzee.AANI.Domain.Models;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace Exwhyzee.AANI.Web.Areas.Main.Pages.GalleryPage
{
    [Microsoft.AspNetCore.Authorization.Authorize]

    public class IndexRemoveModel : PageModel
    {
        private readonly Exwhyzee.AANI.Web.Data.AaniDbContext _context;
        private readonly IHostingEnvironment _hostingEnv;


        public IndexRemoveModel(Exwhyzee.AANI.Web.Data.AaniDbContext context, IHostingEnvironment hostingEnv)
        {
            _context = context;
            _hostingEnv = hostingEnv;
        }

        public IActionResult OnGet()
        {
            var LagefileDbPathName = $"/GalleryLargeImage/".Trim();

            var LargefilePath = $"{_hostingEnv.WebRootPath}{LagefileDbPathName}".Trim();

            DirectoryInfo dir = new DirectoryInfo(LargefilePath);

            foreach (FileInfo fi in dir.GetFiles())
            {
                fi.Delete();

            }

            return Page();
        }
    }
}
