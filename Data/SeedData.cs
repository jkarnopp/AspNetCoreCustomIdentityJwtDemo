using System;
using System.Threading.Tasks;
using AspNetCoreCustomIdentyJwtDemo.Models;
using Microsoft.AspNetCore.Identity;

namespace AspNetCoreCustomIdentyJwtDemo.Data
{
    public class SeedData
    {
        public static async Task Initialize(ApplicationDbContext context,
                              UserManager<ApplicationUser> userManager,
                              RoleManager<ApplicationRole> roleManager)
        {
            context.Database.EnsureCreated();

            string adminRole = "Admin";
            string adminDesc = "This is the administrator role";

            string userRole = "User";
            string userDesc = "This is the default User role";

            string password = "Password123!";

            if (await roleManager.FindByNameAsync(adminRole) == null)
            {
                await roleManager.CreateAsync(new ApplicationRole(adminRole, adminDesc, DateTime.Now));
            }
            if (await roleManager.FindByNameAsync(userRole) == null)
            {
                await roleManager.CreateAsync(new ApplicationRole(userRole, userDesc, DateTime.Now));
            }
            

            if (await userManager.FindByNameAsync("TestAdmin") == null)
            {
                var user = new ApplicationUser
                {
                    UserName = "TestAdmin",
                    Email = "TestAdmin@email.com",
                    FirstName = "Test",
                    LastName = "Admin",
                    PhoneNumber = "1234567890",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    await userManager.AddPasswordAsync(user, password);
                    await userManager.AddToRoleAsync(user, adminRole);
                }
                
            }

            if (await userManager.FindByNameAsync("TestUser") == null)
            {
                var user = new ApplicationUser
                {
                    UserName = "TestUser",
                    Email = "TestUser@email.com",
                    FirstName = "Test",
                    LastName = "User",
                    PhoneNumber = "0987654321",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    await userManager.AddPasswordAsync(user, password);
                    await userManager.AddToRoleAsync(user, userRole);
                }
                
            }

            
        }  
    }
}