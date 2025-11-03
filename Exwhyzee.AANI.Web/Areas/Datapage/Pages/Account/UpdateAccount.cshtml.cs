using Exwhyzee.AANI.Domain.Dtos;
using Exwhyzee.AANI.Domain.Enums;
using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;
using Exwhyzee.AANI.Web.Helper.AWS;
using Exwhyzee.AANI.Web.Migrations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Exwhyzee.AANI.Web.Areas.Datapage.Pages.Account
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin,MNI")]
    public class UpdateAccountModel : PageModel
    {
        private readonly UserManager<Participant> _userManager;
        private readonly AaniDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _config;
        private readonly IStorageService _storageService;

        public UpdateAccountModel(UserManager<Participant> userManager, AaniDbContext context, IWebHostEnvironment env, IConfiguration config, IStorageService storageService)
        {
            _userManager = userManager;
            _context = context;
            _env = env;
            _config = config;
            _storageService = storageService;
        }

        [BindProperty] public int Phase { get; set; } = 1;
        [BindProperty] public Participant Participant { get; set; } = default!;

        // Additional bind properties for fields not in Participant (or for easier binding)
        [BindProperty] public long? OfficeId { get; set; }
        [BindProperty] public string OfficerRole { get; set; }
        [BindProperty] public string Biography { get; set; }
        [BindProperty] public long? SelectedOfficeCategoryId { get; set; }
        [BindProperty] public string LGA { get; set; }
        [BindProperty] public string State { get; set; }
        [BindProperty] public string ContactAddress { get; set; }
        [BindProperty] public string HomeAddress { get; set; }
        [BindProperty] public string PhoneNumber { get; set; }
        [BindProperty] public string? AltPhoneNumber { get; set; }
        [BindProperty, DataType(DataType.Date)] public DateTime DOB { get; set; }
        [BindProperty] public IFormFile PictureFile { get; set; }

        //public List<SelectListItem> OfficeCategoryList { get; set; } = new();
        //public List<SelectListItem> OfficeList { get; set; } = new();
        public List<SelectListItem> SECList { get; set; } = new();
        public List<SelectListItem> ChapterList { get; set; } = new();
        public List<SelectListItem> StateList { get; set; } = new();
        public List<SelectListItem> LGAList { get; set; } = new();

        public SelectList OfficeCategoryList { get; set; }
        public SelectList OfficeList { get; set; }

        [BindProperty]
        public IFormFile? imagefile { get; set; }

        //public long? OfficeId { get; set; }
        public async Task<IActionResult> OnGetAsync(string id, int phase = 1)
        {
            Phase = phase;
            if (id == null) return RedirectToPage("/Notfound", new { area = "" });
            Participant = await _userManager.Users
                .Include(x => x.Office).ThenInclude(o => o.Category)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (Participant == null) return RedirectToPage("/Notfound", new { area = "" });

            await PreparePageAsync(Participant);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var updateparticipant = await _userManager.Users
                .Include(x => x.Office).ThenInclude(o => o.Category)
                .FirstOrDefaultAsync(x => x.Id == Participant.Id);

            if (updateparticipant == null) return RedirectToPage("/Notfound", new { area = "" });

            switch (Phase)
            {
                case 1:

                    // Validate required fields for Phase 1.
                    // According to your Participant model: State and LGA are strings, enum properties are enums,
                    // SECId/ChapterId are long?.
                    if (string.IsNullOrWhiteSpace(Participant.Surname))
                        ModelState.AddModelError(nameof(Participant.Surname), "Surname is required.");
                    if (string.IsNullOrWhiteSpace(Participant.FirstName))
                        ModelState.AddModelError(nameof(Participant.FirstName), "First name is required.");
                    if (string.IsNullOrWhiteSpace(Participant.Title))
                        ModelState.AddModelError(nameof(Participant.Title), "Title is required.");
                    if (DOB == null)
                        ModelState.AddModelError(nameof(DOB), "Date of birth is required.");
                    // enums: check against default value
                    if (Participant.GenderStatus == default(GenderStatus))
                        ModelState.AddModelError(nameof(Participant.GenderStatus), "Gender is required.");
                    if (Participant.MaritalStatus == default(MaritalStatus))
                        ModelState.AddModelError(nameof(Participant.MaritalStatus), "Marital status is required.");
                    if (Participant.ReligionStatus == default(ReligionStatus))
                        ModelState.AddModelError(nameof(Participant.ReligionStatus), "Religion is required.");
                    if (string.IsNullOrWhiteSpace(State))
                        ModelState.AddModelError(nameof(State), "State is required.");
                    if (string.IsNullOrWhiteSpace(LGA))
                        ModelState.AddModelError(nameof(LGA), "LGA is required.");
                
                    if (string.IsNullOrWhiteSpace(Participant.Sponsor))
                        ModelState.AddModelError(nameof(Participant.Sponsor), "Sponsor is required.");
                    if (!Participant.SECId.HasValue || Participant.SECId.Value <= 0)
                        ModelState.AddModelError(nameof(Participant.SECId), "SEC selection is required.");
                    if (!Participant.ChapterId.HasValue || Participant.ChapterId.Value <= 0)
                        ModelState.AddModelError(nameof(Participant.ChapterId), "Chapter selection is required.");
                    if (string.IsNullOrWhiteSpace(PhoneNumber))
                        ModelState.AddModelError(nameof(PhoneNumber), "Phone number is required.");

                    // treat image as valid if either an existing url is present or a file was uploaded
                    var hasUploadedImage = imagefile != null && imagefile.Length > 0;
                    if (!hasUploadedImage && string.IsNullOrWhiteSpace(Participant.PictureUrl))
                    {
                        ModelState.AddModelError(nameof(Participant.PictureUrl), "Profile picture is required.");
                    }
                    else if (hasUploadedImage)
                    {
                        // clear any previous modelstate error for the picture key so it won't show
                        ModelState.Remove(nameof(Participant.PictureUrl));
                    }

                    // Check only the relevant fields
                    var phase1Fields = new[]
                    {
    nameof(Participant.Surname),
    nameof(Participant.FirstName),
    nameof(Participant.Title),
    nameof(DOB),
    nameof(Participant.GenderStatus),
    nameof(Participant.MaritalStatus),
    nameof(Participant.ReligionStatus),
    nameof(State),
    nameof(LGA),
    nameof(Participant.Sponsor),
    nameof(Participant.SECId),
    nameof(Participant.ChapterId),
    nameof(PhoneNumber),
    nameof(Participant.PictureUrl)
};

                    bool Phase1HasErrors = phase1Fields.Any(key =>
                        ModelState.TryGetValue(key, out var entry) && entry?.Errors?.Count > 0);

                    // If any Phase 1 field failed, prepare page data and return
                    if (Phase1HasErrors)
                    {
                        await PreparePageAsync(updateparticipant);
                        return Page();
                    }

                    // Update all phase 1 fields
                    updateparticipant.Surname = Participant.Surname;
                    updateparticipant.FirstName = Participant.FirstName;
                    updateparticipant.OtherName = Participant.OtherName;
                    updateparticipant.Title = Participant.Title;
                    updateparticipant.DOB = DOB;
                    updateparticipant.GenderStatus = Participant.GenderStatus;
                    updateparticipant.MaritalStatus = Participant.MaritalStatus;
                    updateparticipant.ReligionStatus = Participant.ReligionStatus;
                    updateparticipant.AltPhoneNumber = AltPhoneNumber;
                    updateparticipant.State = State;
                    updateparticipant.LGA = LGA;
                    updateparticipant.ContactAddress = ContactAddress;
                    updateparticipant.HomeAddress = HomeAddress;
                    updateparticipant.Sponsor = Participant.Sponsor;
                    updateparticipant.SECId = Participant.SECId;
                    updateparticipant.ChapterId = Participant.ChapterId;
                    if (imagefile != null)
                    {
                        try
                        {
                            // Process file
                            await using var memoryStream = new MemoryStream();
                            await imagefile.CopyToAsync(memoryStream);

                            var fileExt = Path.GetExtension(imagefile.FileName);
                            var docName = $"{Guid.NewGuid()}{fileExt}";
                            // call server

                            var s3Obj = new Domain.Dtos.S3Object()
                            {
                                BucketName = "aani2023",
                                InputStream = memoryStream,
                                Name = docName
                            };

                            var cred = new AwsCredentials()
                            {
                                AccessKey = _config["AwsConfiguration:AWSAccessKey"],
                                SecretKey = _config["AwsConfiguration:AWSSecretKey"]
                            };

                            var xresult = await _storageService.UploadFileReturnUrlAsync(s3Obj, cred, "");
                            // 
                            if (xresult.Message.Contains("200"))
                            {
                                updateparticipant.PictureUrl = xresult.Url;
                                updateparticipant.PictureKey = xresult.Key;
                            }
                            else
                            {
                                TempData["error"] = "unable to upload image";
                                //return Page();
                            }
                        }
                        catch (Exception c)
                        {

                        }
                    }

                    await _userManager.UpdateAsync(updateparticipant);

                    var token = await _userManager.GenerateChangePhoneNumberTokenAsync(updateparticipant, PhoneNumber);
                    await _userManager.ChangePhoneNumberAsync(updateparticipant, PhoneNumber, token);




                    Phase = 2;
                    break;

                case 2:

                    // Phase 2 is compulsory: both OfficeId and OfficerRole required.
                    if (!OfficeId.HasValue || OfficeId.Value <= 0)
                        ModelState.AddModelError(nameof(OfficeId), "Office selection is required for Phase 2.");
                    if (string.IsNullOrWhiteSpace(OfficerRole))
                        ModelState.AddModelError(nameof(OfficerRole), "Current position / officer role is required for Phase 2.");

                    // Check only the Phase-2 fields for errors
                    var phase2Keys = new[] { nameof(OfficeId), nameof(OfficerRole) };

                    bool Phase2HasErrors = phase2Keys.Any(key =>
                        ModelState.TryGetValue(key, out var entry) && entry?.Errors?.Count > 0);

                    if (Phase2HasErrors)
                    {
                        // Optional: remove unrelated modelstate errors so only Phase-2 errors are shown on the page
                        var otherKeys = ModelState.Keys.Except(phase2Keys).ToList();
                        foreach (var k in otherKeys) ModelState.Remove(k);

                        await PreparePageAsync(updateparticipant);
                        return Page();
                    }


                    updateparticipant.OfficeId = OfficeId;
                    updateparticipant.CurrentPosition = OfficerRole;
                    await _userManager.UpdateAsync(updateparticipant);
                    Phase = 3;
                    break;

                case 3:
                    // Biography must have real content (not only empty tags). Enforce requirement.
                    if (!HasNonEmptyText(Biography))
                    {
                        await PreparePageAsync(updateparticipant);
                        ModelState.AddModelError(nameof(Biography), "Biography is required and must contain visible text (not just empty HTML tags).");
                        return Page();
                    }


                    updateparticipant.Bio = Biography;
                    await _userManager.UpdateAsync(updateparticipant);
                    TempData["aasuccess"] = "Profile updated successfully!";

                    var getOperationalYear = await _context.OperationYears.FirstOrDefaultAsync(x=>x.IsActive);
                    return RedirectToPage("./MemberDetails", new {id = updateparticipant.Id, operationYearId  = getOperationalYear.Id});
            }

            // Repopulate for tab UI
            SelectedOfficeCategoryId = updateparticipant.Office?.CategoryId;
          
            await PopulateDropdowns(SelectedOfficeCategoryId);
            await PopulateLGA();

            // Rebind for redisplay
            Participant = updateparticipant;
            LGA = Participant.LGA;
            State = Participant.State;
            ContactAddress = Participant.ContactAddress;
            HomeAddress = Participant.HomeAddress;
            AltPhoneNumber = Participant.AltPhoneNumber;
            DOB = Participant.DOB;
            SelectedOfficeCategoryId = Participant.Office?.CategoryId;
            OfficeId = Participant.OfficeId;
            OfficerRole = Participant.CurrentPosition;
            Biography = Participant.Bio;

            return Page();
        }
        // Helper that centralizes the OnGet logic so it can be reused by OnPost before returning Page()
        private async Task PreparePageAsync(Participant participant)
        {
            Participant = participant;

            // Assign for easier binding (Phase 1)
            LGA = participant.LGA;
            State = participant.State;
            ContactAddress = participant.ContactAddress;
            HomeAddress = participant.HomeAddress;
            PhoneNumber = participant.PhoneNumber;
            AltPhoneNumber = participant.AltPhoneNumber;
            DOB = participant.DOB == default ? DateTime.Now.AddYears(-20) : participant.DOB;

            // Office/position phase
            SelectedOfficeCategoryId = participant.Office?.CategoryId;
            OfficeId = participant.OfficeId;
            OfficerRole = participant.CurrentPosition;
            Biography = participant.Bio;

            await PopulateDropdowns(SelectedOfficeCategoryId);
            await PopulateLGA();
        }

        public async Task<JsonResult> OnGetOffices(long categoryId)
        {
            var offices = await _context.Offices
                .Where(x => x.CategoryId == categoryId)
                .Select(x => new { x.Id, x.Name })
                .ToListAsync();

            return new JsonResult(offices);
        }

        public async Task<JsonResult> OnGetLGA(string stateName)
        {
            var lgas = await _context.LocalGoverments
                .Where(x => x.States.StateName == stateName)
                .Select(x => new { x.LGAName })
                .ToListAsync();

            return new JsonResult(lgas.Select(a => new SelectListItem { Value = a.LGAName, Text = a.LGAName }));
        }
        // Helper: returns true if html contains any non-whitespace textual content after stripping tags.
        private bool HasNonEmptyText(string? html)
        {
            if (string.IsNullOrWhiteSpace(html)) return false;

            // Remove script/style blocks
            html = Regex.Replace(html, "<(script|style)[^>]*?>.*?</\\1>", string.Empty, RegexOptions.Singleline | RegexOptions.IgnoreCase);

            // Remove tags
            var text = Regex.Replace(html, "<.*?>", string.Empty);

            // Decode common HTML entities (basic)
            text = Regex.Replace(text, "&nbsp;", " ", RegexOptions.IgnoreCase);
            text = System.Net.WebUtility.HtmlDecode(text);

            // If after trimming there is any character left => valid
            return !string.IsNullOrWhiteSpace(text.Trim());
        }
        private async Task PopulateDropdowns(long? selectedOfficeCategoryId)
        {

            var categories = await _context.OfficeCategories.ToListAsync();
            OfficeCategoryList = new SelectList(
                categories,
                "Id",
                "Name",
                SelectedOfficeCategoryId    // Sets the selected value
            );
            var offices = await _context.Offices
       .Where(o => o.CategoryId == selectedOfficeCategoryId) // filter by selected category
       .ToListAsync();
            //var offices = await _context.Offices.ToListAsync();
            OfficeList = new SelectList(
    offices,      // Your office entities
    "Id",         // Value field
    "Name",       // Text field
    OfficeId      // Selected value
);
            //OfficeCategoryList = await _context.OfficeCategories
            //    .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name })
            //    .ToListAsync();

            //OfficeList = SelectedOfficeCategoryId.HasValue
            //    ? await _context.Offices.Where(o => o.CategoryId == SelectedOfficeCategoryId)
            //        .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name })
            //        .ToListAsync()
            //    : new List<SelectListItem>();

            SECList = await _context.SECs.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = "SEC " + x.Number + " (" + x.Year + ")"
            }).ToListAsync();

            ChapterList = await _context.Chapters.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.State
            }).ToListAsync();

            StateList = await _context.States.Select(x => new SelectListItem
            {
                Value = x.StateName,
                Text = x.StateName
            }).ToListAsync();
        }

        private async Task PopulateLGA()
        {
            if (!string.IsNullOrEmpty(State))
            {
                LGAList = await _context.LocalGoverments
                    .Where(x => x.States.StateName == State)
                    .Select(x => new SelectListItem { Value = x.LGAName, Text = x.LGAName })
                    .ToListAsync();
            }
            else
            {
                LGAList = new List<SelectListItem>();
            }
        }
    }
}