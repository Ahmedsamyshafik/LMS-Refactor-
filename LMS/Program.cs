using LMS.Models.Data;
using LMS.Models.Entities;
using LMS.Repository;
using LMS.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LMS
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.
			builder.Services.AddControllersWithViews();




			#region my services

			//-------- my services ------------
			builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
			//DB
			#region DB
			builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(
					  builder.Configuration.GetConnectionString("DefaultConnection")));

			builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
				.AddDefaultTokenProviders()
				.AddEntityFrameworkStores<AppDbContext>();
			#endregion
			//
			#region Session
			builder.Services.AddDistributedMemoryCache(); // Adds a default in-memory implementation of IDistributedCache
			builder.Services.AddSession(options =>
			{
				options.IdleTimeout = TimeSpan.FromMinutes(30); // Set the timeout period
				options.Cookie.HttpOnly = true; // Make the session cookie HTTP only
				options.Cookie.IsEssential = true; // Make the session cookie essential
			});
			#endregion

			//DI
			#region DI
			builder.Services.AddRepositoryDependencies();
			builder.Services.AddServiceDependencies();
			#endregion

			#endregion
			var app = builder.Build();

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

			app.UseAuthentication();
			app.UseAuthorization();

			app.UseSession(); // Add this line to enable session support


			app.MapControllerRoute(
				name: "default",
				pattern: "{controller=Home}/{action=Index}/{id?}");

			app.Run();
		}
	}
}
