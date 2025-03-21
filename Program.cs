using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PadelApp.Data;

var builder = WebApplication.CreateBuilder(args);

var port = Environment.GetEnvironmentVariable("PORT") ?? "80";

builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

builder.Services.AddRazorPages();

var app = builder.Build();



app.Urls.Add("http://0.0.0.0:80");

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await RoleAdmin(services);
}



if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    
    app.UseHsts();
}
app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=public}/{action=Index}/{id?}")
    .WithStaticAssets();


app.MapRazorPages()
   .WithStaticAssets();

app.Run();

//------------------------------------------------------------------------------//
async Task RoleAdmin(IServiceProvider serviceProvider)
{
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

    string adminEmail = "ossu2300@student.miun.se";
    string adminPassword = "Password123@"; 

    var admin = await userManager.FindByEmailAsync(adminEmail);

    if (admin == null){
        admin = new IdentityUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = false };

        var result = await userManager.CreateAsync(admin, adminPassword);

        if (result.Succeeded){

            var adminExists = await roleManager.RoleExistsAsync("Admin");
             if (!adminExists) {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
             }
            await userManager.AddToRoleAsync(admin, "Admin");
        }    

    }
}
//-----------------------------------------------------------------------------------//
