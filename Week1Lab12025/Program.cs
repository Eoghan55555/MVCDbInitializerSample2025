using Microsoft.EntityFrameworkCore;
using Tracker.WebAPIClient;
using Week1Lab12025.Models;
namespace Week1Lab12025
{
    public class Program
    {

        public static void Main(string[] args)
        
        {
            ActivityAPIClient.Track(StudentID: "S00233339", StudentName: "Eoghan Brown",
            activityName: "Rad302 2025 Week 1 Lab 1", Task: "Database Initializers Setup Successfully");
            var builder = WebApplication.CreateBuilder(args);
            var dbConnectionString = builder.Configuration.GetConnectionString("DefaultConnection")
               ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<UserContext>(options =>
            options.UseSqlServer(dbConnectionString));
            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var app = builder.Build();
            using (var scope = app.Services.CreateScope())
            {
                var _ctx = scope.ServiceProvider.GetRequiredService<UserContext>();
                var hostEnviroment = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
                DbSeeder dbSeeder = new DbSeeder(_ctx, hostEnviroment);
                dbSeeder.Seed();
            }

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
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

            app.Run();
        }
    }
}
