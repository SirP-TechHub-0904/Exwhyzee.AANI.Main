using Exwhyzee.AANI.Domain.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Exwhyzee.AANI.Web.Data
{
    public class AaniDbContext : IdentityDbContext
    {
        public AaniDbContext(DbContextOptions<AaniDbContext> options)
            : base(options)
        {
        }

        public DbSet<BoardOfGovornorCategory> BoardOfGovornorCategories { get; set; } = default!;
        public DbSet<BoardOfGovornorMember> BoardOfGovornorMembers { get; set; } = default!;
        public DbSet<CategoryFamiliesOnSEC> CategoryFamiliesOnSECs { get; set; } = default!;
        public DbSet<Contributor> Contributors { get; set; } = default!;
        public DbSet<ContributorCategory> ContributorCategories { get; set; } = default!;
        public DbSet<Event> Events { get; set; } = default!;
        public DbSet<Fund> Funds { get; set; } = default!;
        public DbSet<FundCategory> FundCategories { get; set; } = default!;
        public DbSet<OfficialRole> OfficialRoles { get; set; } = default!;
        public DbSet<Paper> Papers { get; set; } = default!;
        public DbSet<PaperCategory> paperCategories { get; set; } = default!;
        public DbSet<ParticipantFamiliesOnSEC> ParticipantFamiliesOnSECs { get; set; } = default!;
        public DbSet<PrincipalOfficer> PrincipalOfficers { get; set; } = default!;
        public DbSet<PrincipalOfficerCategory> PrincipalOfficerCategories { get; set; } = default!;
        public DbSet<SEC> SECs { get; set; } = default!;
        public DbSet<SubCategoryFamiliesOnSEC> SubCategoryFamiliesOnSECs { get; set; } = default!;
        public DbSet<State> States { get; set; } = default!;
        public DbSet<LocalGoverment> LocalGoverments { get; set; } = default!;
        public DbSet<Message> Messages { get; set; } = default!;
        public DbSet<EventAttendance> EventAttendances { get; set; } = default!;
        public DbSet<EventCommitte> EventCommittes { get; set; } = default!;
        public DbSet<EventComment> EventComments { get; set; } = default!;
        public DbSet<EventExpenditure> EventExpenditures { get; set; } = default!;
        public DbSet<EventBudget> EventBudget { get; set; } = default!;


    }
}