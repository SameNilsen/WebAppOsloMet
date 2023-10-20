using Microsoft.EntityFrameworkCore;
using WebAppOsloMet.DAL;
using Microsoft.AspNetCore.Identity;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("PostDbContextConnection") ?? throw new InvalidOperationException("Connection string 'PostDbContextConnection' not found.");

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
});

builder.Services.AddDbContext<PostDbContext>(options =>
{
    options.UseSqlite(
        builder.Configuration["ConnectionStrings:PostDbContextConnection"]);
});

//builder.Services.AddDefaultIdentity<IdentityUser>()
//    .AddEntityFrameworkStores<PostDbContext>();

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredUniqueChars = 6;

    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(60);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    options.User.RequireUniqueEmail = true;
})
    .AddEntityFrameworkStores<PostDbContext>()
    .AddDefaultTokenProviders()
    .AddDefaultUI();    //  Pga IEmailSender

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".Adventure.Session";
    options.IdleTimeout = TimeSpan.FromSeconds(1800);
    options.Cookie.IsEssential = true;
});


builder.Services.AddScoped<IPostRepository, PostRepository>();

builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddRazorPages();
builder.Services.AddSession();

// Logger
var loggerConfiguration = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.File($"Logs/app_{DateTime.Now:yyyyMMdd_HHmmss}.log");

loggerConfiguration.Filter.ByExcluding(e => e.Properties.TryGetValue("SourceContext", out var value) &&
e.Level == LogEventLevel.Information &&
e.MessageTemplate.Text.Contains("Executed DbCommand"));

var logger = loggerConfiguration.CreateLogger();
builder.Logging.AddSerilog(logger);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    DBInit.Seed(app);
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.UseAuthentication();

app.MapControllerRoute(
name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
//Change action=Index to acion=Privacy to start with another file(view). See HomeController.cs

app.MapRazorPages();

app.Run();
