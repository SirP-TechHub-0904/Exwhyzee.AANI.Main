using Amazon.S3;
using Exwhyzee.AANI.Domain.Models;
using Exwhyzee.AANI.Web.Data;
using Exwhyzee.AANI.Web.FileManager;
using Exwhyzee.AANI.Web.Helper;
using Exwhyzee.AANI.Web.Helper.AWS;
using Exwhyzee.AANI.Web.Helper.BaseHelper;
using Exwhyzee.AANI.Web.HostedServices;
using Exwhyzee.AANI.Web.Services.Template;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PostmarkEmailService;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.//
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AaniDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<Participant, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<AaniDbContext>()
    .AddDefaultTokenProviders();
builder.Services.AddAWSService<IAmazonS3>();
builder.Services.AddTransient<IFileManagement, FileManagement>(); 
builder.Services.AddTransient<IStorageService, StorageService>();
builder.Services.AddTransient<IBaseModel, BaseModel>();
// Register your TokenService and email sender
builder.Services.AddScoped<TokenService>();                // or ITokenService if you add an interface
builder.Services.AddScoped<Exwhyzee.AANI.Web.Helper.IEmailSender, EmailSender>(); // register the SMTP implementation
                                                                                  // Register Hosted Service
builder.Services.AddHostedService<NotificationSenderHostedService>();                                                                             // configure HttpClient for Kudi
builder.Services.AddHttpClient("kudi", client =>
{
    client.BaseAddress = new Uri("https://my.kudisms.net/"); // EmailSender posts to api/sms (relative)
    client.Timeout = TimeSpan.FromSeconds(30);
});


// Set token lifespan to 7 days
builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
{
    options.TokenLifespan = TimeSpan.FromDays(7); // 1 week
});
builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings.
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
    // options.Password.RequiredUniqueChars = 0;

    // Lockout settings.
    //options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    //options.Lockout.MaxFailedAccessAttempts = 5;
    //options.Lockout.AllowedForNewUsers = true;

    // User settings.
    options.User.AllowedUserNameCharacters =
    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;

});
builder.Services.AddHttpClient();
builder.Services.AddMvc(options =>
{
    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
}).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
builder.Services.AddControllersWithViews();
//
builder.Services.AddScoped<ITemplateRenderer, TemplateRenderer>();
builder.Services.AddScoped<IRecipientParser, RecipientParser>();
builder.Services.AddHostedService<NotificationSenderHostedService>();



//
builder.Services.AddMvc(option => option.EnableEndpointRouting = false)
            .AddRazorPagesOptions(options =>
            {
                options.RootDirectory = "/Pages";
                //options.AddPageRouteOption("/Questionner", "{name}");
            });

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = $"/Identity/Account/Login";
    options.LogoutPath = $"/Identity/Account/Logout";
    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
});
builder.Services.AddTransient<PostmarkClient>(_ =>
                          new PostmarkClient(builder.Configuration.GetSection("PostmarkSettings")["ServerToken"]));


builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
