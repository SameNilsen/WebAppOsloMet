using Microsoft.EntityFrameworkCore;
using WebAppOsloMet.DAL;

var builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddScoped<IPostRepository, PostRepository>();

builder.Services.AddScoped<IUserRepository, UserRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    DBInit.Seed(app);
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
//Change action=Index to acion=Privacy to start with another file(view). See HomeController.cs

app.Run();
