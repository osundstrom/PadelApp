using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PadelApp.Data;

var builder = WebApplication.CreateBuilder(args);

//för deploy
var port = Environment.GetEnvironmentVariable("PORT") ?? "80";

//för deploy
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


//för deploy
app.Urls.Add("http://0.0.0.0:80"); 


//scope lägga till admin och user. 
using (var scope = app.Services.CreateScope()){
    var services = scope.ServiceProvider;
    await RoleAdmin(services);
}



if (app.Environment.IsDevelopment()){
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

app.UseAuthorization(); //auth

app.MapStaticAssets(); //statiska

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Public}/{action=Index}/{id?}")
    .WithStaticAssets();


app.MapRazorPages()
   .WithStaticAssets();

app.Run();//kör

//------------------------------admin roll, skapa admin------------------------------------------------//
async Task RoleAdmin(IServiceProvider serviceProvider)
{
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>(); //rolemanager
    var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>(); //usermanager

    string adminEmail = "ossu2300@student.miun.se"; //sätter epost
    string adminPassword = "Password123@";  //sätter lösen

    var admin = await userManager.FindByEmailAsync(adminEmail);//kolla omm finns

    if (admin == null){ //om admin ej finns
        //skapa ny admin
        admin = new IdentityUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = false };
        //skapar användare
        var result = await userManager.CreateAsync(admin, adminPassword);

        if (result.Succeeded){ 
            //om admin rollen redan finns
            var adminExists = await roleManager.RoleExistsAsync("Admin");
             if (!adminExists) {
                //Om admin rollen ej finns
                await roleManager.CreateAsync(new IdentityRole("Admin"));
             }
             //Lägger till som admin
            await userManager.AddToRoleAsync(admin, "Admin");
        }    

    }
}
//-----------------------------------------------------------------------------------//
