using Exwhyzee.AANI.Domain.Enums;
using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace Exwhyzee.AANI.Web.Areas.Alumni.Pages.Dashboard
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "MNI")]
    public class IndexModel : PageModel
    {
        private readonly UserManager<Participant> _userManager;
        private readonly AaniDbContext _context;

        public IndexModel(UserManager<Participant> userManager, AaniDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // Dashboard cards
        public int AllAlumni { get; set; }
        public int Male { get; set; }
        public int Female { get; set; }
        public int Alive { get; set; }
        public int Dead { get; set; }
        public int Active { get; set; }

        // Chart JSON (serialized on server)
        public string ChartPayload { get; set; } = "null";

        // Operation years for selector
        public IList<OperationYear> OperationYears { get; set; } = new List<OperationYear>();
        public int DefaultChartYear { get; set; } = DateTime.UtcNow.Year;

        // Upcoming events and birthdays
        public IList<EventViewModel> UpcomingEvents { get; set; } = new List<EventViewModel>();
        public string DateofBirthList { get; set; } = string.Empty;


        public SelectList OperationYearsSelectList { get; set; } = default!;

        public List<FundSummaryBox> MyFundSummaries { get; set; } = new();


        public async Task<IActionResult> OnGetAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Base alumni query
            var alumni = _userManager.Users
                .Include(x => x.SEC)
                .Where(x => x.MniStatus == Domain.Enums.MniStatus.MNI)
                .AsQueryable();

            AllAlumni = await alumni.CountAsync();
            Male = await alumni.Where(x => x.GenderStatus == Domain.Enums.GenderStatus.Male && x.AliveStatus == AliveStatus.Alive).CountAsync();
            Female = await alumni.Where(x => x.GenderStatus == Domain.Enums.GenderStatus.Female && x.AliveStatus == AliveStatus.Alive).CountAsync();
            Alive = await alumni.Where(x => x.AliveStatus == Domain.Enums.AliveStatus.Alive).CountAsync();
            Dead = await alumni.Where(x => x.AliveStatus == Domain.Enums.AliveStatus.Dead).CountAsync();
            Active = await alumni.Where(x => x.UserStatus == Domain.Enums.UserStatus.Active).CountAsync();
         
            
            OperationYears = await _context.OperationYears
               .AsNoTracking()
               .OrderByDescending(o => o.StartDate)
               .ToListAsync();

            var activeOperationYearId = OperationYears.FirstOrDefault(o => o.IsActive)?.Id;
            MyFundSummaries = await _context.FundCategories
       .Select(cat => new FundSummaryBox
       {
           CategoryTitle = cat.Title,
           Paid = cat.Funds
               .Where(f => f.ParticipantId == userId && f.FundStatus == FundStatus.Paid && f.OperationYearId == activeOperationYearId)
               .Sum(f => (decimal?)f.Amount) ?? 0,
           Pending = cat.Funds
               .Where(f => f.ParticipantId == userId && f.FundStatus == FundStatus.NotPaid && f.OperationYearId == activeOperationYearId)
               .Sum(f => (decimal?)f.Amount) ?? 0
       })
       .ToListAsync();

            // birthdays for today
            var listdob = await _userManager.Users
                .Include(x => x.SEC)
                .Where(x=>x.AliveStatus == AliveStatus.Alive)
                .Where(x => x.MniStatus == Domain.Enums.MniStatus.MNI && x.DOB.Day == DateTime.UtcNow.Day && x.DOB.Month == DateTime.UtcNow.Month)
                .OrderByDescending(x => x.DOB)
                .ToListAsync();

            foreach (var listx in listdob)
            {
                DateofBirthList += $"<span>🎉 HAPPY BIRTHDAY {System.Net.WebUtility.HtmlEncode(listx.Fullname)} (SEC{System.Net.WebUtility.HtmlEncode(listx.SEC?.Number.ToString() ?? "")}) 🎉</span> ";
            }

            if (listdob.Count > 0)
            {
                TempData["checkifexist"] = "exist";
            }

            // Operation years (desc by StartDate)
           

            if (OperationYears.Any())
            {
                DefaultChartYear = OperationYears.First().StartDate.Year;
            }

            // Build chart payload from DB per OperationYear
            // payload shape: { "2024": { labels:[...], attendance:[...], revenue:[...], logins:[...] }, ... }
            var payload = new Dictionary<string, object>();

            foreach (var op in OperationYears)
            {
                // month labels
                var labels = Enumerable.Range(1, 12).Select(m => new DateTime(2000, m, 1).ToString("MMM")).ToArray();

                // Attendance grouped by ea.Date.Month for events belonging to this operation year
                var attendanceGroups = await _context.EventAttendances
                    .AsNoTracking()
                    .Include(ea => ea.Event)
                    .Where(ea => ea.Event.OperationYearId == op.Id)
                    .GroupBy(ea => ea.Date.Month)
                    .Select(g => new { Month = g.Key, Count = g.Count() })
                    .ToListAsync();

                var attendance = Enumerable.Range(1, 12)
                    .Select(m => attendanceGroups.FirstOrDefault(x => x.Month == m)?.Count ?? 0)
                    .ToArray();

                // Revenue (Paid funds) grouped by DatePaid.Month and filtered by OperationYearId
                var revenueGroups = await _context.Funds
                    .AsNoTracking()
                    .Where(f => f.OperationYearId == op.Id && f.FundStatus == Domain.Enums.FundStatus.Paid)
                    .GroupBy(f => f.DatePaid.Month)
                    .Select(g => new { Month = g.Key, Total = g.Sum(f => f.Amount) })
                    .ToListAsync();

                var revenue = Enumerable.Range(1, 12)
                    .Select(m => (double)(revenueGroups.FirstOrDefault(x => x.Month == m)?.Total ?? 0m))
                    .ToArray();

                // Logins by month if LoginHistory exists, otherwise zero array
                int[] loginsArray = Enumerable.Range(1, 12).Select(_ => 0).ToArray();
                if (await TableExistsAsync("LoginHistories"))
                {
                    // assume OperationYear start-year is the year to aggregate
                    var yearNum = op.StartDate.Year;
                    var loginGroups = await _context.Set<LoginHistory>()
                        .AsNoTracking()
                        .Where(lh => lh.Timestamp.Year == yearNum)
                        .GroupBy(lh => lh.Timestamp.Month)
                        .Select(g => new { Month = g.Key, Count = g.Select(x => x.ParticipantId).Distinct().Count() })
                        .ToListAsync();

                    loginsArray = Enumerable.Range(1, 12)
                        .Select(m => loginGroups.FirstOrDefault(x => x.Month == m)?.Count ?? 0)
                        .ToArray();
                }
                OperationYearsSelectList = new SelectList(
     OperationYears.Select(o => new {
         Id = o.StartDate.Year,
         Text = !string.IsNullOrEmpty(o.Name) ? o.Name : o.StartDate.Year.ToString()
     }),
     "Id",
     "Text",
     DefaultChartYear
 );

                payload[op.StartDate.Year.ToString()] = new
                {
                    labels = labels,
                    attendance = attendance,
                    revenue = revenue,
                    logins = loginsArray
                };
            }

            // Serialize payload for client (camelCase)
            ChartPayload = JsonSerializer.Serialize(payload, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            });

            // Upcoming events (future)
            UpcomingEvents = await _context.Events
                .Where(e => e.OperationYearId == activeOperationYearId)
                .OrderBy(e => e.StartDate)
                .Select(e => new EventViewModel
                {
                    Id = e.Id,
                    Title = e.Title ?? string.Empty,
                    StartDate = e.StartDate,
                    Location = e.Location ?? string.Empty
                })
                .Take(5)
                .ToListAsync();

            return Page();
        }

        // Simple table-exists helper for optional LoginHistory table
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

        public class EventViewModel
        {
            public long Id { get; set; }
            public string Title { get; set; } = string.Empty;
            public DateTime StartDate { get; set; }
            public string Location { get; set; } = string.Empty;
        }

        public class FundSummaryBox
        {
            public string CategoryTitle { get; set; }
            public decimal Paid { get; set; }
            public decimal Pending { get; set; }
        }

    }
}