using Exwhyzee.AANI.Domain.Dtos;
using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;
using Exwhyzee.AANI.Web.Helper;
using Exwhyzee.AANI.Web.Helper.AWS;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Exwhyzee.AANI.Web.Areas.Main.Pages.ParticipantPage
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin,AANI")]

    public class SendResetAccountModel : PageModel
    {
        private readonly SignInManager<Participant> _signInManager;
        private readonly UserManager<Participant> _userManager;
        private readonly Exwhyzee.AANI.Web.Data.AaniDbContext _context;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _config;



        public SendResetAccountModel(
            UserManager<Participant> userManager,
            SignInManager<Participant> signInManager,
            AaniDbContext context,
            IEmailSender emailSender,
            IConfiguration config)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _emailSender = emailSender;
            _config = config;
        }

        [BindProperty]
        public Participant Participant { get; set; }
        public List<Participant> ParticipantsList { get; set; }

        [BindProperty]
        public List<string> SelectedParticipantIds { get; set; } // IDs of selected participants

        public List<SelectListItem> ChapterList { get; set; } = new();
        public List<SelectListItem> SecList { get; set; } = new();

        public Chapter Chapter { get; set; }

        public SEC SEC { get; set; }

        public async Task<IActionResult> OnGetAsync(string? id, long secid = 0, long chapterId = 0)
        {
            if (id == null)
            {
                // Load all participants (you can filter as needed)
                ParticipantsList = await _userManager.Users
                    .Include(x => x.Chapter)
                    .Include(x => x.SEC)
                    .ToListAsync();

                Chapter = await _context.Chapters.FirstOrDefaultAsync(x => x.Id == chapterId);
                if (Chapter != null)
                {
                    ParticipantsList = ParticipantsList.Where(x => x.ChapterId == Chapter.Id).ToList();
                }

                SEC = await _context.SECs.FirstOrDefaultAsync(x => x.Id == secid);
                if (SEC != null)
                {
                    ParticipantsList = ParticipantsList.Where(x => x.SECId == SEC.Id).ToList();
                }

            }
            else
            {
                Participant = await _userManager.FindByIdAsync(id);

                if (Participant == null)
                {
                    return RedirectToPage("/Notfound", new { area = "" });
                }
            }
            ChapterList = await _context.Chapters.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.State
            }).ToListAsync();

            SecList = await _context.SECs.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = "SEC " + x.Number + " - " + x.Year.ToString()
            }).ToListAsync();
            return Page();
        }
        public async Task EmailContent(Participant participant, string callbackUrl)
        {


            if (!string.IsNullOrWhiteSpace(participant?.Email))
            {


                var subject = "AANI | New AANI Portal – Kindly Update Your Profile";

                var body = $@" 
  <tr>
        <td align=""center"">
            <table width=""600"" cellspacing=""0"" cellpadding=""0"" style=""background-color:#ffffff; padding:30px; border-radius:8px; box-shadow:0 0 10px rgba(0,0,0,0.1);"">
                <tr>
                    <td>
                        <h2 style=""color:#d32f2f; margin-top:0;"">Welcome to the New AANI Portal – Kindly Update Your Profile</h2>
                        <p style=""line-height:1.6;"">Dear Distinguished {participant?.Fullname}, mni</p>
                        <p style=""line-height:1.6;"">Warm greetings to you.</p>
                        <p style=""line-height:1.6;"">We are pleased to inform you that the <strong>new AANI Digital Secretariat Portal</strong> is now fully built and ready for your use. This platform represents a major step in modernizing our operations, strengthening our internal coordination, and enhancing the visibility and prestige of the <strong>Alumni Association of the National Institute (AANI)</strong>.</p>

                        <p style=""line-height:1.6;"">The portal serves as our central digital hub, hosting:</p>
                        <ul style=""margin:10px 0 20px 20px;"">
                            <li>Your personal and professional profile</li>
                            <li>SEC and Chapter information</li>
                            <li>AANI Digital Library (IRPs, SEC themes, research archives, etc.)</li>
                            <li>Payment of dues (National & Chapters)</li>
                            <li>Upcoming AANI elections</li>
                            <li>Announcements, newsletters & membership updates</li>
                            <li>Many more features to improve networking and engagement</li>
                        </ul>

                        <p style=""line-height:1.6;"">To help us maintain an accurate and dynamic database, you are kindly requested to log in and update your information.</p>
                        <p style=""margin:20px 0;"">
                            <a href=""{callbackUrl}"" style=""display:inline-block; padding:12px 25px; background-color:#d32f2f; color:#ffffff; text-decoration:none; border-radius:5px;"">Click here to reset your account and access the portal and complete your profile</a>
                        </p>

                        <p style=""line-height:1.6;"">Please take a few minutes to update your details, including:</p>
                        <ul style=""margin:10px 0 20px 20px;"">
                            <li>Name</li>
                            <li>Contact information</li>
                            <li>Date of birth</li>
                            <li>SEC / Set</li>
                            <li>Professional profile</li>
                            <li>State Chapter</li>
                            <li>Any other relevant information as requested in the form</li>
                        </ul>

                        <p style=""line-height:1.6;"">Your participation will ensure that AANI maintains a reliable, forward-looking digital record of all members and continues to grow as a strong, united, and visible association.</p>
                        <p style=""line-height:1.6;"">If you need assistance, kindly feel free to reach out to 07041238888 or 08037915777.</p>
                        <p style=""line-height:1.6;"">Thank you for your cooperation and continuous commitment to AANI — Towards a Better Society.</p>

                        <p style=""line-height:1.6;"">Warm regards,</p>
                        <p style=""line-height:1.6;""><strong>Engr. Ifeanyi Jude Ngama, mni</strong><br>National Publicity Secretary</p>
 
                    </td>
                </tr>
            </table>
        </td>
    </tr> ";

                try
                {
                    await _emailSender.SendEmailAsync(participant.Email, subject, body);
                }
                catch (Exception ex)
                {
                    // Handle or log error
                }
            }

        }

        public async Task SendNotify(Participant participant, string callbackUrl)
        {
            if (!string.IsNullOrWhiteSpace(participant?.PhoneNumber))
            {
                var smsMessage = $"AANI: Your account password has been reset by the AANI administrator. Please check your email for reset instructions and update your profile. - AANI Support";
                try
                {
                    await _emailSender.SendNotification(participant.PhoneNumber, smsMessage);
                }
                catch (Exception ex)
                {
                }

                //try
                //{
                //    var whatsappMessage = $"Hello {participant.Fullname}, A password reset link has been sent to your registered email address and " +
                //      $"Please reset your password and log in to the new AANI Portal to update your profile and keep your information current: {callbackUrl}";

                //    await _emailSender.SendWhatsappAsync(participant.PhoneNumber, whatsappMessage);
                //}
                //catch (Exception ex)
                //{
                //}
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (SelectedParticipantIds == null || !SelectedParticipantIds.Any())
            {
                ModelState.AddModelError("", "Please select at least one participant.");
                ParticipantsList = await _userManager.Users.ToListAsync();
                return Page();
            }

            foreach (var id in SelectedParticipantIds)
            {



                var participant = await _userManager.FindByIdAsync(id);
                if (participant == null) continue;
                var code = await _userManager.GeneratePasswordResetTokenAsync(participant);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ResetPassword",
                    pageHandler: null,
                    values: new { area = "Identity", code },
                    protocol: Request.Scheme);
                try
                {
                    try { await EmailContent(participant, callbackUrl); }
                    catch (Exception ex) { }

                    try
                    {
                        await SendNotify(participant, callbackUrl);
                    }
                    catch (Exception ex) { }

                    participant.SendResetAccount = true;
                    participant.DateAccountResetSent = DateTime.UtcNow;
                    await _userManager.UpdateAsync(participant);


                }
                catch (Exception ex)
                {

                }

            }

            TempData["Success"] = "Reset links have been sent to selected participants.";
            return RedirectToPage();
        }


        public async Task<IActionResult> OnPostSingleAsync()
        {
            var participant = await _userManager.FindByIdAsync(Participant.Id);
            try
            {
                var code = await _userManager.GeneratePasswordResetTokenAsync(participant);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ResetPassword",
                    pageHandler: null,
                    values: new { area = "Identity", code },
                    protocol: Request.Scheme);
                try { await EmailContent(participant, callbackUrl); }
                catch (Exception ex) { }

                try
                {
                    await SendNotify(participant, callbackUrl);
                }
                catch (Exception ex) { }

                participant.SendResetAccount = true;
                participant.DateAccountResetSent = DateTime.UtcNow;
                await _userManager.UpdateAsync(participant);
                TempData["success"] = "Account reset instructions sent successfully.";
                return RedirectToPage("./SendResetAccount", new { id = participant.Id });

            }
            catch (Exception ex)
            {
                TempData["error"] = "An error occurred while sending reset instructions.";
                return RedirectToPage("./SendResetAccount", new { id = participant.Id });
            }


        }
    }
}