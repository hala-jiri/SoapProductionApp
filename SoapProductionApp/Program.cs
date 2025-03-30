using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using SoapProductionApp.Data;
using SoapProductionApp.Services;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

//builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// my services
builder.Services.AddSingleton<IEmailSender, EmailSender>();
builder.Services.AddScoped<IAuditService, AuditService>();
// part for logging
builder.Services.AddScoped<AuditService>();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseStaticFiles();

// Přidáváme podporu českého formátování čísel a datumu
var cultureInfo = new CultureInfo("en-US");
cultureInfo.NumberFormat.NumberDecimalSeparator = "."; // Zajišťuje čárku jako oddělovač desetinných míst

var supportedCultures = new[] { cultureInfo };

app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("en-US"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
});



app.UseRouting();

app.UseAuthorization();

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    string[] roleNames = { "Admin", "RegisteredUser" };
    foreach (var roleName in roleNames)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }

    // Definujeme admin uživatele
    string adminEmail = "gabca@soapmate.eu";
    string adminPassword = "Admin@123"; // Změň pro produkční prostředí!

    var adminUser = await userManager.FindByEmailAsync(adminEmail);

    if (adminUser == null)
    {
        adminUser = new IdentityUser { UserName = adminEmail, Email = adminEmail };
        var createUserResult = await userManager.CreateAsync(adminUser, adminPassword);

        if (createUserResult.Succeeded)
            await userManager.AddToRoleAsync(adminUser, "Admin");
    }
    else
    {
        var roles = await userManager.GetRolesAsync(adminUser);
        if (!roles.Contains("Admin"))
            await userManager.AddToRoleAsync(adminUser, "Admin");
    }
}

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
