using Microsoft.AspNetCore.Identity;
using VarlikYönetimi.Core.Entities;
using System.Text.Json;

namespace VarlikYönetimi.Data.Seeds
{
    public static class UserSeed
    {
        public static async Task SeedAdminUser(UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            // Admin rolü
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                var adminRole = new Role 
                { 
                    Name = "Admin",
                    Description = "Sistem Yöneticisi",
                    CreatedAt = DateTime.UtcNow,
                    ApprovalCount = 0,
                    MaxApprovalAmount = 999999999,
                    ApprovalLevels = JsonSerializer.Serialize(new List<string>()) 
                };
                await roleManager.CreateAsync(adminRole);
            }

            // Admin kullanıcısı
            var adminUser = new User
            {
                UserName = "admin@varlikyonetimi.com",
                Email = "admin@varlikyonetimi.com",
                NormalizedUserName = "ADMIN@VARLIKYONETIMI.COM",
                NormalizedEmail = "ADMIN@VARLIKYONETIMI.COM",
                FirstName = "Admin",
                LastName = "User",
                PhoneNumber = "5551234567",
                IsActive = true,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                CreatedAt = DateTime.UtcNow
            };

            // Eğer admin kullanıcısı yoksa
            if (await userManager.FindByEmailAsync(adminUser.Email) == null)
            {
                var result = await userManager.CreateAsync(adminUser, "Admin123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }
    }
} 