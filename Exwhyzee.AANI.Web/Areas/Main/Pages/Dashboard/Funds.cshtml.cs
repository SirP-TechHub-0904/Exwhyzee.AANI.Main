using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Exwhyzee.AANI.Web.Areas.Main.Pages.Dashboard
{
    [Microsoft.AspNetCore.Authorization.Authorize]

    public class FundsModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
