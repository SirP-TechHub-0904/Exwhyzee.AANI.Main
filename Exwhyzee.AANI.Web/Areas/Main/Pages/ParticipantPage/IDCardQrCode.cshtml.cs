using Exwhyzee.AANI.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AANI.Web.Areas.Main.Pages.ParticipantPage
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]

    public class IDCardQrCodeModel : PageModel
    {
        private readonly UserManager<Participant> _userManager;
        private readonly SignInManager<Participant> _signInManager;
        private readonly ILogger<IDCardQrCodeModel> _logger;
        private readonly Exwhyzee.AANI.Web.Data.AaniDbContext _context;


        public IDCardQrCodeModel(SignInManager<Participant> signInManager,
            ILogger<IDCardQrCodeModel> logger,
            UserManager<Participant> userManager, Exwhyzee.AANI.Web.Data.AaniDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _context = context;
        }


        [BindProperty]
        public Participant UserDatas { get; set; }
        public string ProfileLink { get; set; }
        public SEC GetSEC { get; set; }
        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            UserDatas = await _userManager.FindByIdAsync(id);
            if (UserDatas == null)
            {
                return NotFound();
            }
            if (String.IsNullOrEmpty(UserDatas.PictureUrl))
            {
                return RedirectToPage("./UpdateIDCard", new { id = UserDatas.Id });
            }

            var queryableSource = _context.SECs.AsQueryable();
            GetSEC = queryableSource.FirstOrDefault(x => x.Id == UserDatas.SECId);

            while (UserDatas.IdDigit == null)
            {
                string randomAlphaNumeric = GenerateRandomAlphaNumeric(6);
                UserDatas.IdDigit = randomAlphaNumeric.ToString();

                // Check if the generated IdDigit already exists in the database
                var check = await _userManager.Users.FirstOrDefaultAsync(x => x.IdDigit == UserDatas.IdDigit);

                if (check == null)
                {
                    // If not found, update the UserDatas and exit the loop
                    //await _userManager.UpdateAsync(UserDatas);

                    var result = await _userManager.UpdateAsync(UserDatas);

                    if (!result.Succeeded)
                    {
                        foreach (var error in result.Errors)
                        {
                            Console.WriteLine($"{error.Code}: {error.Description}");
                            // Or log it to a file or display in the UI
                        }
                    }
                    else
                    {
                        Console.WriteLine("User updated successfully.");
                    }



                    break;
                }
                else
                {
                    // If found, reset UserDatas.IdDigit to null and generate a new random string in the next iteration
                    UserDatas.IdDigit = null;
                }
            }
            Zen.Barcode.CodeQrBarcodeDraw barcode = Zen.Barcode.BarcodeDrawFactory.CodeQr;
            string userinfo = "";
            try

            {
                ProfileLink = "https://aani.ng/identify/" + UserDatas.IdDigit;
                System.Drawing.Image img = barcode.Draw(ProfileLink, 100);

                byte[] imgBytes = turnImageToByteArray(img);
                //

                string imgString = Convert.ToBase64String(imgBytes);
                Image = imgBytes;
            }
            catch (Exception c)
            {

            }

            return Page();
        }
        [BindProperty]
        public byte[] Image { get; set; }
        [BindProperty]
        public byte[] JurayImage { get; set; }
        private byte[] turnImageToByteArray(System.Drawing.Image img)
        {
            MemoryStream ms = new MemoryStream();
            img.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            return ms.ToArray();
        }
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {


            return RedirectToPage("./Index");
        }
        static string GenerateRandomAlphaNumeric(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }

}
