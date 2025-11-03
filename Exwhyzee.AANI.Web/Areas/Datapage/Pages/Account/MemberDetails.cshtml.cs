using Exwhyzee.AANI.Domain.Enums;
using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SkiaSharp;
using SkiaSharp.Extended.Svg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Exwhyzee.AANI.Web.Areas.Datapage.Pages.Account
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin,MNI")]
    public class MemberDetailsModel : PageModel
    {
        private readonly UserManager<Participant> _userManager;
        private readonly AaniDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IWebHostEnvironment _env;
        public MemberDetailsModel(UserManager<Participant> userManager, AaniDbContext context, IHttpClientFactory httpClientFactory, IWebHostEnvironment env)
        {
            _userManager = userManager;
            _context = context;
            _httpClientFactory = httpClientFactory;
            _env = env;
        }

        // Participant loaded from DB / Identity
        public Participant Participant { get; set; } = default!;

        // Operation years list and selected id
        public IList<OperationYear> OperationYears { get; set; } = new List<OperationYear>();
        public long? OperationYearId { get; set; }
        public string? OperationYearDisplay { get; set; }

        // Totals for profile card (operation-year scoped)
        public int TotalAttendance { get; set; }
        public decimal TotalFundsContributed { get; set; }
        public int TotalLogins { get; set; }

        // Tab lists
        public IList<EventAttendanceViewModel> EventsAttended { get; set; } = new List<EventAttendanceViewModel>();
        public IList<FundViewModel> FinancialRecords { get; set; } = new List<FundViewModel>();
        public IList<CommitteeViewModel> Committees { get; set; } = new List<CommitteeViewModel>();
        public string ProfileLink { get; set; }
        public SEC GetSEC { get; set; }

        public string UserFullname { get;set; }
        // Add this property (PageModel) so the Razor view can render the generated SVG
        [BindProperty]
        public string? IdCardSvg { get; set; }
        // OnGet - accepts optional participant id and optional operationYearId
        public async Task<IActionResult> OnGetAsync(string? id, long? operationYearId)
        {
            // Determine participant id: if not provided, use current user
            if (string.IsNullOrWhiteSpace(id))
            {
                var current = await _userManager.GetUserAsync(User);
                if (current == null) return Forbid();
                id = current.Id;
                TempData["profile"] = "MY PROFILE";
            }
            else
            {
                TempData["profile"] = "MEMBER PROFILE";
            }

            // Load participant via DbContext to include navigation properties if present
            Participant = await _userManager.Users
                .AsNoTracking()
                .Include(p => p.Chapter)
                .Include(p => p.SEC)
                .Include(p => p.OfficialRoles)
                .Include(p => p.Office).ThenInclude(x => x.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            // Fallback to UserManager if not found in context
            if (Participant == null)
            {
                var fallback = await _userManager.FindByIdAsync(id);
                if (fallback == null) return NotFound();
                Participant = fallback;
            }

            // Load available operation years for dropdown (descending by StartDate)
            OperationYears = await _context.OperationYears
                .AsNoTracking()
                .OrderByDescending(o => o.StartDate)
                .ToListAsync();

            // Selection logic:
            // 1) If operationYearId provided -> use it
            // 2) Else if any OperationYear.IsActive == true -> use the first active
            // 3) Else -> use the latest OperationYear (by StartDate)
            if (operationYearId.HasValue)
            {
                OperationYearId = operationYearId.Value;
            }
            else
            {
                var activeOp = OperationYears.FirstOrDefault(o => o.IsActive);
                if (activeOp != null)
                {
                    OperationYearId = activeOp.Id;
                }
                else
                {
                    var latest = OperationYears.FirstOrDefault();
                    OperationYearId = latest?.Id;
                }
            }

            OperationYearDisplay = OperationYearId.HasValue
                ? OperationYears.FirstOrDefault(o => o.Id == OperationYearId.Value)?.Name
                  ?? OperationYears.FirstOrDefault(o => o.Id == OperationYearId.Value)?.StartDate.Year.ToString()
                : null;

            // If no operation year found, set zeros and return
            if (!OperationYearId.HasValue)
            {
                TotalAttendance = 0;
                TotalFundsContributed = 0m;
                TotalLogins = 0;
                EventsAttended = new List<EventAttendanceViewModel>();
                FinancialRecords = new List<FundViewModel>();
                Committees = new List<CommitteeViewModel>();
                return Page();
            }

            // --- Totals and lists filtered by OperationYearId ---

            // Total attendance for participant in selected operation year
            TotalAttendance = await _context.EventAttendances
                .AsNoTracking()
                .Include(ea => ea.Event)
                .Where(ea => ea.ParticipantId == id && ea.Event.OperationYearId == OperationYearId)
                .CountAsync();

            // Total funds contributed (Paid) for the operation year
            TotalFundsContributed = await _context.Funds
                .AsNoTracking()
                .Where(f => f.ParticipantId == id && f.OperationYearId == OperationYearId && f.FundStatus == FundStatus.Paid)
                .Select(f => (decimal?)f.Amount)
                .SumAsync() ?? 0m;

            // Total logins for the operation year (if you have a LoginHistory table)
            if (await TableExistsAsync("LoginHistories"))
            {
                var opYearNum = (await _context.OperationYears.AsNoTracking().Where(o => o.Id == OperationYearId.Value).Select(o => o.StartDate.Year).FirstOrDefaultAsync());
                TotalLogins = await _context.Set<LoginHistory>()
                    .AsNoTracking()
                    .Where(lh => lh.ParticipantId == id && lh.Timestamp.Year == opYearNum)
                    .Select(lh => lh.ParticipantId)
                    .Distinct()
                    .CountAsync();
            }
            else
            {
                // Fallback: show 1 if LastLogin present else 0
                TotalLogins = Participant.LastLogin != default ? 1 : 0;
            }

            // Latest 5 events attended (operation year)
            EventsAttended = await _context.EventAttendances
                .AsNoTracking()
                .Include(ea => ea.Event)
                .Where(ea => ea.ParticipantId == id && ea.Event.OperationYearId == OperationYearId)
                .OrderByDescending(ea => ea.DatetimeArrival)
                .Select(ea => new EventAttendanceViewModel
                {
                    EventId = ea.EventId,
                    Title = ea.Event.Title,
                    StartDate = ea.Event.StartDate,
                    Location = ea.Event.Location,
                    Arrival = ea.DatetimeArrival,
                    Departure = ea.DatetimeDeparture
                })
                .Take(5)
                .ToListAsync();

            // Latest 10 fund records for the operation year
            FinancialRecords = await _context.Funds
                .AsNoTracking()
                .Include(f => f.FundCategory)
                .Include(f => f.Event)
                .Where(f => f.ParticipantId == id && f.OperationYearId == OperationYearId)
                .OrderByDescending(f => f.DatePaid)
                .Select(f => new FundViewModel
                {
                    Id = f.Id,
                    DatePaid = f.DatePaid,
                    Amount = f.Amount,
                    Status = f.FundStatus.ToString(),
                    Category = f.FundCategory != null ? f.FundCategory.Title : null,
                    EventTitle = f.Event != null ? f.Event.Title : null
                })
                .Take(10)
                .ToListAsync();

            // Committees for this participant in this operation year
            Committees = await _context.EventCommittes
                .AsNoTracking()
                .Include(ec => ec.Event)
                .Where(ec => ec.ParticipantId == id && ec.Event.OperationYearId == OperationYearId)
                .OrderByDescending(ec => ec.Date)
                .Select(ec => new CommitteeViewModel
                {
                    EventId = ec.EventId,
                    EventTitle = ec.Event.Title,
                    Position = ec.Position,
                    DateAssigned = ec.Date
                })
                .ToListAsync();



            var queryableSource = _context.SECs.AsQueryable();
            GetSEC = queryableSource.FirstOrDefault(x => x.Id == Participant.SECId);

            while (Participant.IdDigit == null)
            {
                string randomAlphaNumeric = GenerateRandomAlphaNumeric(6);
                Participant.IdDigit = randomAlphaNumeric.ToString();

                // Check if the generated IdDigit already exists in the database
                var check = await _userManager.Users.FirstOrDefaultAsync(x => x.IdDigit == Participant.IdDigit);

                if (check == null)
                {
                    // If not found, update the UserDatas and exit the loop
                    //await _userManager.UpdateAsync(UserDatas);

                    var result = await _userManager.UpdateAsync(Participant);

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
                    Participant.IdDigit = null;
                }
            }
            IdCardSvg = await BuildIdCardSvgForParticipantAsync(Participant);

            UserFullname = BuildTitleFullNameSecYearFilename(Participant);
            return Page();
        }
        [BindProperty]
        public byte[] Image { get; set; }
        [BindProperty]
        public byte[] BarImage { get; set; }

        // Add helper method(s) anywhere inside the MemberDetailsModel class
        private byte[] ImageToPngBytes(System.Drawing.Image img)
        {
            using var ms = new MemoryStream();
            img.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            return ms.ToArray();
        }

        private async Task<string> GetImageDataUriAsync(string? urlOrPath)
        {
            if (string.IsNullOrWhiteSpace(urlOrPath))
                return string.Empty;

            // if already a data URI, return as-is
            if (urlOrPath.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
                return urlOrPath;

            try
            {
                // treat as URL when it starts with http(s)
                if (urlOrPath.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                {
                    var client = _httpClientFactory.CreateClient();
                    var resp = await client.GetAsync(urlOrPath);
                    if (!resp.IsSuccessStatusCode) return string.Empty;
                    var bytes = await resp.Content.ReadAsByteArrayAsync();
                    var contentType = resp.Content.Headers.ContentType?.MediaType ?? "application/octet-stream";
                    return $"data:{contentType};base64,{Convert.ToBase64String(bytes)}";
                }
                else
                {
                    // assume a local server path (absolute or relative to content root)
                    // try absolute first, else try content root
                    string path = urlOrPath;
                    if (!System.IO.File.Exists(path))
                    {
                        var contentRoot = Directory.GetCurrentDirectory();
                        path = Path.Combine(contentRoot, urlOrPath.TrimStart('~', '/', '\\'));
                    }
                    if (!System.IO.File.Exists(path)) return string.Empty;
                    var bytes = await System.IO.File.ReadAllBytesAsync(path);
                    // try infer content type from extension
                    var ext = Path.GetExtension(path).ToLowerInvariant();
                    var ct = ext switch
                    {
                        ".png" => "image/png",
                        ".jpg" => "image/jpeg",
                        ".jpeg" => "image/jpeg",
                        ".gif" => "image/gif",
                        _ => "application/octet-stream"
                    };
                    return $"data:{ct};base64,{Convert.ToBase64String(bytes)}";
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        // Resolves a path under wwwroot and returns a data URI (or empty string if not found)
        private async Task<string> GetWebRootFileDataUriAsync(string wwwrootRelativePath)
        {
            if (string.IsNullOrWhiteSpace(wwwrootRelativePath))
                return string.Empty;

            // Normalize path: remove leading ~ or /
            var trimmed = wwwrootRelativePath.TrimStart('~', '/', '\\');

            // Prefer IWebHostEnvironment.WebRootPath (physical path to wwwroot)
            var webRoot = _env.WebRootPath;
            if (string.IsNullOrEmpty(webRoot))
            {
                // Fallback to ContentRootPath + "wwwroot"
                webRoot = Path.Combine(_env.ContentRootPath ?? Directory.GetCurrentDirectory(), "wwwroot");
            }

            // Build the physical path
            var physicalPath = Path.Combine(webRoot, trimmed);

            // OPTIONAL: log or inspect the paths during debugging
            // Console.WriteLine($"Resolving webroot file: webRoot={webRoot}, physicalPath={physicalPath}");

            if (!System.IO.File.Exists(physicalPath))
                return string.Empty;

            var bytes = await System.IO.File.ReadAllBytesAsync(physicalPath);
            var ext = Path.GetExtension(physicalPath).ToLowerInvariant();
            var contentType = ext switch
            {
                ".png" => "image/png",
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".svg" => "image/svg+xml",
                ".gif" => "image/gif",
                _ => "application/octet-stream"
            };

            return $"data:{contentType};base64,{Convert.ToBase64String(bytes)}";
        }
        private async Task<string> BuildIdCardSvgForParticipantAsync(Participant participant)
        {
            if (participant == null) return string.Empty;
            Zen.Barcode.CodeQrBarcodeDraw barcode = Zen.Barcode.BarcodeDrawFactory.CodeQr;
            string userinfo = "";
            try

            {
                ProfileLink = "https://aani.ng/identify/" + participant.IdDigit;
                System.Drawing.Image img = barcode.Draw(ProfileLink, 100);

                byte[] imgBytes = turnImageToByteArray(img);
                //

                string imgString = Convert.ToBase64String(imgBytes);
                Image = imgBytes;
            }
            catch (Exception c)
            {

            }
            string qrDataUri = string.Empty;
            try
            {
                var profileLink = "https://aani.ng/identify/" + participant.IdDigit;
                Zen.Barcode.CodeQrBarcodeDraw qr = Zen.Barcode.BarcodeDrawFactory.CodeQr;
                using var qrImg = qr.Draw(profileLink, 200); // System.Drawing.Image
                var qrPng = ImageToPngBytes(qrImg);
                qrDataUri = "data:image/png;base64," + Convert.ToBase64String(qrPng);
            }
            catch
            {
                qrDataUri = string.Empty;
            }
            // 2) fetch/convert passport image to data URI (participant.PictureUrl may be data URI already)
            var passportDataUri = await GetImageDataUriAsync(participant.PictureUrl);

            // 3) fetch/convert base template background image (if you have one)
            //    Set your template path or URL here. Example: "wwwroot/templates/id-template.png" or an S3 URL.
            //var templatePathOrUrl = "wwwroot/based_image.jpg"; // <-- REPLACE with your server path or URL if different
            //var templateDataUri = await GetImageDataUriAsync(templatePathOrUrl);


            var templateRelative = "based_image.jpg";
            var templateDataUri = await GetWebRootFileDataUriAsync(templateRelative);


            // 4) compose SVG embedding the template, the passport image and the QR (all as data URIs)
            //    Coordinates and sizes follow the template used earlier; tweak if necessary.
            // Compose a minimal, no-color SVG: background image, passport, QR, name + position (neutral)
            // Safe build of full name with null checks, uppercase for name parts, and an optional suffix.
            // This avoids NullReferenceException if Participant or any name property is null.
            var title = participant?.Title?.Trim() ?? string.Empty;
            var surname = (participant?.Surname ?? string.Empty).Trim().ToUpperInvariant();
            var firstName = (participant?.FirstName ?? string.Empty).Trim().ToUpperInvariant();
            var otherName = (participant?.OtherName ?? string.Empty).Trim().ToUpperInvariant();

            // Join only non-empty parts with a single space
            var nameParts = new[] { title, surname, firstName, otherName }
                .Where(p => !string.IsNullOrWhiteSpace(p));

            var baseName = string.Join(" ", nameParts).Trim();

            // If you always want the suffix ", mni" appended only when there's a name:
            var suffix = ", mni";
            var finalName = string.IsNullOrEmpty(baseName) ? string.Empty : (baseName + suffix);

            // HTML-encode for safe embedding in SVG/HTML
            var fullName = WebUtility.HtmlEncode(finalName);

            var positionText = WebUtility.HtmlEncode((participant.CurrentPosition ?? "").ToUpperInvariant());
            var secText = participant.SEC != null ? WebUtility.HtmlEncode($"SEC {participant.SEC.Number} ({participant.SEC.Year})") : "";

            // Toggle frame visibility: set to false to remove frame strokes completely
            bool showFrame = false;

            // Compose a minimal, neutral SVG: background artwork, passport (clipped), QR, name + position.
            // Note: outer svg uses only viewBox so CSS can control width/height responsively.
            // SHIFT DOWN DELTA = 40 px (≈0.133 in)
            // Paste this into your OnGetAsync (replace the previous svg composition block).
            // It centralizes the 6 editable knobs so you can tweak positions by changing the ints below.
            // Fixed SVG composition string: corrected tag syntax, removed stray spaces,
            // and avoided nested interpolation issues by building small fragments first.

            // Editable layout knobs for the ID card SVG.
            // Comments above each variable explain what it controls and how to tweak it.

            int canvasW = 900;   // Canvas width in pixels. Defines the SVG coordinate width (900 px ≈ 3.00 in at 300 DPI).
            int canvasH = 1400;  // Canvas height in pixels. Defines the SVG coordinate height (1400 px ≈ 4.667 in).

            // Global vertical offset applied to the photo block and derived positions.
            // Increase this to move the whole photo/name/QR group down; decrease to move it up.
            int yOffset = 169;

            // Photo box (inner visible image area) position and size.
            // photo_x: left offset from the left edge of the canvas to the left edge of the photo box.
            //   Increase → move photo right. Decrease → move photo left.
            int photo_x = 230;

            // photo_y: top offset from the top edge of the canvas to the top of the photo box.
            //   It's computed with yOffset so changing yOffset moves the photo (and dependent items) together.
            //   Increase → move photo down. Decrease → move photo up.
            int photo_y = 200 + yOffset;

            // photo_w: width of the visible photo area in pixels.
            //   Increase → photo shows wider area (bigger). Decrease → smaller visible area.
            //   Keep photo_w == photo_h for a square photo.
            int photo_w = 440;

            // photo_h: height of the visible photo area in pixels.
            //   Increase → taller visible area. Decrease → shorter.
            int photo_h = 470;

            // Spacing values between photo and text/QR.
            // nameGap: vertical gap in pixels from the bottom of the photo box to the name baseline.
            //   Increase → name sits further below photo. Decrease → name moves closer to photo.
            int nameGap = 105;

            // positionGap: vertical gap in pixels from the name baseline to the position/job-title baseline.
            //   Increase → more space between name and position. Decrease → less space.
            int positionGap = 40;

            // qrGap: vertical gap in pixels from the bottom of the photo box to the top of the QR code.
            //   Increase → QR moves further down. Decrease → QR moves up.
            int qrGap = 274;

            // QR placement and size.
            // qr_size: QR square size in pixels. Larger improves scannability when printed.
            //   Example: 180 px ≈ 0.6 in at 300 DPI. Use ~168 px for 0.56 in if you need exact measurements.
            int qr_size = 200;

            // centerX: horizontal center of the canvas. Useful for centering text and centering the QR.
            int centerX = canvasW / 2;

            // Derived vertical positions computed from photo_y/photo_h and gaps.
            // name_y: baseline Y coordinate for the name text (photo bottom + nameGap).
            int name_y = photo_y + photo_h + nameGap;

            // position_y: baseline Y coordinate for the position text (name baseline + positionGap).
            int position_y = name_y + positionGap;

            // sec_y: baseline Y coordinate for the SEC/third line (position baseline + fixed offset).
            //   Change the +38 value if you want more/less space beneath the position line.
            int sec_y = position_y + 58;

            // qr_x: left X coordinate for the QR, computed to center the QR horizontally.
            int qr_x = centerX - (qr_size / 2);

            // qr_y: top Y coordinate for the QR, placed below the photo by qrGap.
            int qr_y = photo_y + photo_h + qrGap;

            // Outer decorative frame (computed relative to the inner photo box).
            // outer_x: left of outer frame (10 px left of inner photo box by default).
            //   Change the 10 to tighten/loosen the outer border spacing.
            int outer_x = photo_x - 10;

            // outer_y: top of outer frame (10 px above inner photo box by default).
            int outer_y = photo_y - 10;

            // outer_w: outer frame width (inner width + 20 for symmetric horizontal padding).
            int outer_w = photo_w + 20;

            // outer_h: outer frame height (inner height + 20 for symmetric vertical padding).
            int outer_h = photo_h + 20;

            // outer_rx: corner radius for the outer frame. Increase for more rounded corners.
            int outer_rx = 20;

            // inner_rx: corner radius for the inner clipped photo box and clipPath.
            //   Increase to make inner corners more rounded; set to 0 for square corners.
            int inner_rx = 16;

            showFrame = true;

            // Build fragments to avoid nested interpolation problems
            string bgFragment = string.IsNullOrEmpty(templateDataUri)
                ? ""
                : $"<image href=\"{templateDataUri}\" x=\"0\" y=\"0\" width=\"{canvasW}\" height=\"{canvasH}\" preserveAspectRatio=\"xMidYMid slice\" />";

            string frameOuterFragment = showFrame
                ? $"<rect x=\"{outer_x}\" y=\"{outer_y}\" width=\"{outer_w}\" height=\"{outer_h}\" rx=\"{outer_rx}\" ry=\"{outer_rx}\" class=\"frame-outer\" />"
                : "";

            string frameInnerFragment = showFrame
                ? $"<rect x=\"{photo_x}\" y=\"{photo_y}\" width=\"{photo_w}\" height=\"{photo_h}\" rx=\"{inner_rx}\" ry=\"{inner_rx}\" class=\"frame-inner\" />"
                : "";

            string clipFragment = $"<defs><clipPath id=\"photoClip\"><rect x=\"{photo_x}\" y=\"{photo_y}\" width=\"{photo_w}\" height=\"{photo_h}\" rx=\"{inner_rx}\" ry=\"{inner_rx}\" /></clipPath></defs>";

            string passportFragment = string.IsNullOrEmpty(passportDataUri)
                ? $"<rect x=\"{photo_x}\" y=\"{photo_y}\" width=\"{photo_w}\" height=\"{photo_h}\" fill=\"#ffffff\" />"
                : $"<g clip-path=\"url(#photoClip)\"><image href=\"{passportDataUri}\" x=\"{photo_x}\" y=\"{photo_y}\" width=\"{photo_w}\" height=\"{photo_h}\" preserveAspectRatio=\"xMidYMid slice\" /></g>";

            string textFragment =
                $"<text x=\"{centerX}\" y=\"{name_y}\" class=\"id-name\">{fullName}</text>" +
                //$"<text x=\"{centerX}\" y=\"{position_y}\" class=\"id-position\">{positionText}</text>" +
                $"<text x=\"{centerX}\" y=\"{sec_y}\" class=\"id-sec\">{secText}</text>";

            string qrFragment = string.IsNullOrEmpty(qrDataUri)
                ? ""
                : $"<image href=\"{qrDataUri}\" x=\"{qr_x}\" y=\"{qr_y}\" width=\"{qr_size}\" height=\"{qr_size}\" preserveAspectRatio=\"xMidYMid meet\" />";

            // Compose final SVG (single interpolated string, no nested $"..." inside it)
            var svg = $@"<?xml version=""1.0"" encoding=""utf-8""?>
<svg xmlns=""http://www.w3.org/2000/svg"" viewBox=""0 0 {canvasW} {canvasH}"" preserveAspectRatio=""xMidYMid meet"">
  <style>
    .id-name {{ font-family: 'Montserrat', Arial, sans-serif; font-weight:800; font-size:30px; fill:#ffffff; text-anchor:middle;text-transform: none;}}
    .id-position {{ font-family: 'Montserrat', Arial, sans-serif; font-weight:700; font-size:30px; fill:#ffffff; text-anchor:middle;text-transform: none; }}
    .id-sec {{ font-family: 'Montserrat', Arial, sans-serif; font-weight:800; font-size:30px; fill:#ffffff; text-anchor:middle;text-transform: none; }}
    .frame-outer {{ fill:none; stroke:#ffffff; stroke-width:2; opacity:0.65; }}
    .frame-inner {{ fill:none; stroke:#ffffff; stroke-width:1.5; opacity:0.6; }}
  </style>

  {bgFragment}
  {frameOuterFragment}
  {frameInnerFragment}
  {clipFragment}
  {passportFragment}
  {textFragment}
  {qrFragment}
</svg>";

            return svg;
        }
        private byte[] turnImageToByteArray(System.Drawing.Image img)
        {
            MemoryStream ms = new MemoryStream();
            img.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            return ms.ToArray();
        }
        static string GenerateRandomAlphaNumeric(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        // Returns a filename-safe string like: "Dr_JOHN_DOE_SEC_12_2024"
        // Parts are joined with underscores, upper-cased where appropriate, and sanitized for file systems.
        private string BuildTitleFullNameSecYearFilename(Participant participant)
        {
            if (participant == null) return "aani-id-card";

            // Safe extraction with trims and case
            var title = (participant.Title ?? "").Trim();
            var surname = (participant.Surname ?? "").Trim().ToUpperInvariant();
            var firstName = (participant.FirstName ?? "").Trim().ToUpperInvariant();
            var otherName = (participant.OtherName ?? "").Trim().ToUpperInvariant();

            // Build full name: include only non-empty parts
            var nameParts = new[] { surname, firstName, otherName }.Where(p => !string.IsNullOrWhiteSpace(p));
            var fullName = string.Join("_", nameParts); // already upper-cased

            // SEC info: use participant.SEC if available, or fallback to SECId/empty
            string secPart;
            if (participant.SEC != null)
            {
                // use number + year if available
                secPart = $"SEC{participant.SEC.Number}_{participant.SEC.Year}";
            }
            
            else
            {
                secPart = "SEC";
            }

            // Year: prefer SEC.Year, else try to use current year
            var year = participant.SEC?.Year.ToString() ?? DateTime.UtcNow.Year.ToString();

            // Compose title (keep title as-is but trimmed), then join pieces that are non-empty
            var pieces = new List<string>();
            if (!string.IsNullOrWhiteSpace(title)) pieces.Add(title.Replace(' ', '_')); // keep casing of title
            if (!string.IsNullOrWhiteSpace(fullName)) pieces.Add(fullName);
            if (!string.IsNullOrWhiteSpace(secPart)) pieces.Add(secPart);

            var raw = string.Join("_", pieces);

            // Sanitize for filenames: remove or replace characters not allowed in file names
            // We'll replace any character other than letters, digits, underscore, hyphen, or dot with underscore.
            var sanitized = System.Text.RegularExpressions.Regex.Replace(raw, @"[^A-Za-z0-9_\-\.]", "_");

            // Optionally trim length to avoid very long filenames
            var maxLength = 120;
            if (sanitized.Length > maxLength)
            {
                sanitized = sanitized.Substring(0, maxLength);
            }

            return sanitized;
        }
        // Helper to check if a DB table exists (used for optional LoginHistory)
        private async Task<bool> TableExistsAsync(string tableName)
        {
            try
            {
                var conn = _context.Database.GetDbConnection();
                await conn.OpenAsync();
                using var cmd = conn.CreateCommand();
                cmd.CommandText = $"SELECT CASE WHEN OBJECT_ID('{tableName}', 'U') IS NOT NULL THEN 1 ELSE 0 END";
                var result = await cmd.ExecuteScalarAsync();
                return Convert.ToInt32(result) == 1;
            }
            catch
            {
                return false;
            }
        }
        // Add these two handler methods into your MemberDetailsModel class.
        // If you want PNG server-side rasterization, add the NuGet packages:
        //   - SkiaSharp
        //   - SkiaSharp.Svg
        // using SkiaSharp;
        // using SkiaSharp.Svg;
        // using System.Text;

        public async Task<IActionResult> OnGetDownloadSvg(string id)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest();

            // Load participant by id (same as you do in OnGetAsync)
            var participant = await _userManager.Users
                .AsNoTracking()
                .Include(p => p.Chapter)
                .Include(p => p.SEC)
                .Include(p => p.OfficialRoles)
                .Include(p => p.Office).ThenInclude(x => x.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (participant == null) return NotFound();

            var svg = await BuildIdCardSvgForParticipantAsync(participant);
            if (string.IsNullOrEmpty(svg)) return NotFound("SVG generation failed.");

            var bytes = System.Text.Encoding.UTF8.GetBytes(svg);
           string userFullname = BuildTitleFullNameSecYearFilename(participant);

            var fileName = $"{userFullname}.svg";
            return File(bytes, "image/svg+xml", fileName);
        }

        public async Task<IActionResult> OnGetDownloadPng(string id)
        {
            // If you don't want server-side rasterization, you can remove this handler and use client-side conversion instead.
            if (string.IsNullOrEmpty(id)) return BadRequest();

            // Load participant by id (same as you do in OnGetAsync)
            var participant = await _userManager.Users
                .AsNoTracking()
                .Include(p => p.Chapter)
                .Include(p => p.SEC)
                .Include(p => p.OfficialRoles)
                .Include(p => p.Office).ThenInclude(x => x.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (participant == null) return NotFound();


            // Try server-side rasterization using SkiaSharp + SkiaSharp.Svg
            try
            {
                // Render to PNG at the same logical pixel size as your SVG canvas.
                // Increase outputWidth/Height for higher DPI output.
                int outputWidth = 900;
                int outputHeight = 1400;
                var svgfile = await BuildIdCardSvgForParticipantAsync(participant);

                using var svgStream = new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(svgfile));

                // If you do not have SkiaSharp.Svg available, this block will fail.
                // Add SkiaSharp.Svg NuGet package to enable the code below.
                var svg = new SkiaSharp.Extended.Svg.SKSvg();
                svg.Load(svgStream);
                var picture = svg.Picture;
                if (picture == null)
                {
                    return StatusCode(500, "Failed to parse SVG for PNG conversion.");
                }

                float svgWidth = picture.CullRect.Width;
                float svgHeight = picture.CullRect.Height;

                // If the SVG does not include explicit size, fall back to viewBox sizing by assuming outputWidth/outputHeight
                float scaleX = svgWidth > 0 ? (float)outputWidth / svgWidth : 1f;
                float scaleY = svgHeight > 0 ? (float)outputHeight / svgHeight : 1f;
                float scale = Math.Min(scaleX, scaleY);

                var imgInfo = new SKImageInfo(outputWidth, outputHeight);
                using var surface = SKSurface.Create(imgInfo);
                var canvas = surface.Canvas;
                canvas.Clear(SKColors.Transparent);

                // Center the drawing
                var translateX = svgWidth > 0 ? (outputWidth - svgWidth * scale) / 2f : 0f;
                var translateY = svgHeight > 0 ? (outputHeight - svgHeight * scale) / 2f : 0f;

                using (new SKAutoCanvasRestore(canvas))
                {
                    canvas.Translate(translateX, translateY);
                    canvas.Scale(scale, scale);
                    canvas.DrawPicture(picture);
                }

                using var image = surface.Snapshot();
                using var data = image.Encode(SKEncodedImageFormat.Png, 100);
                var pngBytes = data.ToArray();

                var fileName = $"aani-id-{(string.IsNullOrEmpty(id) ? "card" : id)}.png";
                return File(pngBytes, "image/png", fileName);
            }
            catch (System.IO.FileNotFoundException)
            {
                // Missing native Skia assets on the host - fall through to a helpful error.
                return StatusCode(500, "PNG conversion failed: SkiaSharp native dependencies missing. Install SkiaSharp and SkiaSharp.Svg on the server.");
            }
            catch (System.Exception ex)
            {
                // If SkiaSharp is not available or conversion fails, return server error with message.
                return StatusCode(500, $"PNG conversion failed: {ex.Message}");
            }
        }
        // View models
        public class EventAttendanceViewModel
        {
            public long EventId { get; set; }
            public string? Title { get; set; }
            public DateTime StartDate { get; set; }
            public string? Location { get; set; }
            public DateTime Arrival { get; set; }
            public DateTime Departure { get; set; }
        }

        public class FundViewModel
        {
            public long Id { get; set; }
            public DateTime DatePaid { get; set; }
            public decimal Amount { get; set; }
            public string? Status { get; set; }
            public string? Category { get; set; }
            public string? EventTitle { get; set; }
        }

        public class CommitteeViewModel
        {
            public long EventId { get; set; }
            public string? EventTitle { get; set; }
            public string? Position { get; set; }
            public DateTime DateAssigned { get; set; }
        }
    }
}