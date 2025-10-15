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
        //EntityFrameworkCore\Add-Migration
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
        public DbSet<Chapter> Chapters { get; set; } = default!;
        public DbSet<Patron> Patrons { get; set; } = default!;
        public DbSet<HeritageCouncil> HeritageCouncils { get; set; } = default!;
        public DbSet<PastExecutiveMember> PastExecutiveMembers { get; set; } = default!;
        public DbSet<Executive> Executives { get; set; } = default!;
        public DbSet<PastExecutiveYear> PastExecutiveYear { get; set; } = default!;
        public DbSet<Nec> Necs { get; set; } = default!;
        public DbSet<Gallery> Galleries { get; set; } = default!;
        public DbSet<Blog> Blogs { get; set; } = default!;
        public DbSet<Comment> Comments { get; set; } = default!;
        public DbSet<MemberNotRecorded> MemberNotRecorded { get; set; } = default!;
        public DbSet<Campain> Campains { get; set; } = default!;
        public DbSet<CampainPost> CampainPosts { get; set; } = default!;
        public DbSet<ExecutivePosition> ExecutivePositions { get; set; } = default!;
        public DbSet<QouteOfDay> QouteOfDays { get; set; } = default!;
        public DbSet<Slider> Sliders { get; set; } = default!;
        public DbSet<BlogCategory> BlogCategories { get; set; } = default!;
        public DbSet<MessageTemplateCategory> MessageTemplateCategories { get; set; } = default!;
        public DbSet<MessageTemplateContent> MessageTemplateContents { get; set; } = default!;

        public DbSet<WebPage> WebPages { get; set; } 
        public DbSet<PageSection> PageSections { get; set; }
        public DbSet<PageSectionList> PageSectionLists { get; set; }
        public DbSet<PageCategory> PageCategories { get; set; }


        public DbSet<ContactSettingsModel> ContactSettings { get; set; }
        public DbSet<PhoneNumberInfo> PhoneNumbers { get; set; }
        public DbSet<EmailInfo> Emails { get; set; }
        public DbSet<AddressInfo> Addresses { get; set; }
        public DbSet<SocialMediaLinks> SocialMediaLinks { get; set; }


        public DbSet<StylingConfig> StylingConfigs { get; set; }
    }
}