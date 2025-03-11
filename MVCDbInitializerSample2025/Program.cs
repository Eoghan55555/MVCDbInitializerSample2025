using ClassLib.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MVCDbInitializerSample2025.Data;
using MVCDbInitializerSample2025.Models;

namespace MVCDbInitializerSample2025
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            // Application User subclassing IdentityUser in The MVC App uses the ApplicationDbContext<ApplicationUser> Pattern
            // this requires an additional migration
            // using add-migration -Context ApplicationDbContext "Create Application User"
            var dbConnectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<UserContext>(options =>
            options.UseSqlServer(dbConnectionString));



     

            // Note Clubs Context is visible via a Project Assembly Reference
            // The migrations manager in the PMC window picks up on these Program code entries in terms of Contexts
            // Migration is added using this command
            // add-migration -Context ClubsContext "Initial Club Context creation" -OutputDir Data/Migrations/ClubMigrations
            var clubConnectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
                                        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ClubsContext>(options =>
            //New Target assembly directive for migrations
                options.UseSqlServer(clubConnectionString, b => b.MigrationsAssembly("MVCDbInitializerSample2025")));

            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Scope and activate DB Context Seeding method which is in the ClubsContext class
            using (var scope = app.Services.CreateScope())
            {
                var _ctx = scope.ServiceProvider.GetRequiredService<ClubsContext>();
                // Retrieve the IWebHostEnvironment for the Content Root
                var hostEnvironment = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
                _ctx.Seed(hostEnvironment.ContentRootPath);
            }
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

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();

            app.Run();
        }
    }
}
