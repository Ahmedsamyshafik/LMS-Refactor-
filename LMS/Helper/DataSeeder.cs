using LMS.Models.Entities;
using Microsoft.AspNetCore.Identity;

namespace LMS.Helper
{
    public static class DataSeeder
    {
        public static void Seed(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Authorization depends on these roles, so they are created synchronously before the app serves
            // a single request.
            foreach (var role in new[] { Constants.AdminRole, Constants.TeacherRole, Constants.StudentRole })
            {
                if (roleManager.FindByNameAsync(role).GetAwaiter().GetResult() == null)
                {
                    roleManager.CreateAsync(new IdentityRole(role)).GetAwaiter().GetResult();
                }
            }

            var roleAdmin = Constants.AdminRole;

            // Create an admin user if it doesn't exist
            var adminUser = userManager.FindByNameAsync("Admin").Result;
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = "Admin",
                    Email = "Admin@gmail.com",
                    PhoneNumber = "01095385375",
                    EmailConfirmed = true,
                    Name = "Ahmed Samy"
                };

                var result = userManager.CreateAsync(adminUser, "Alahly1907#").Result;

                // Add the admin user to the admin role
                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(adminUser, roleAdmin).GetAwaiter().GetResult();
                }


            }
        }
    }

}
