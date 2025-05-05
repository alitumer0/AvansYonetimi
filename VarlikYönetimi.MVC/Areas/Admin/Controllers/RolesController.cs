using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using VarlikYönetimi.Core.Entities;
using VarlikYönetimi.Core.ViewModels;
using VarlikYönetimi.Core.Enums;
using VarlikYönetimi.Data.Context;

namespace VarlikYönetimi.MVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class RolesController : Controller
    {
        private readonly RoleManager<Role> _roleManager;
        private readonly UserManager<User> _userManager;
        private readonly AppDbContext _context;

        public RolesController(
            RoleManager<Role> roleManager,
            UserManager<User> userManager,
            AppDbContext context)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .ToListAsync();

            var viewModels = new List<UserRoleViewModel>();
            
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var currentRole = roles.FirstOrDefault();
                
                viewModels.Add(new UserRoleViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    FullName = $"{user.FirstName} {user.LastName}",
                    Email = user.Email,
                    CurrentRole = currentRole ?? "Rol Atanmamış",
                    AvailableRoles = Enum.GetValues(typeof(ApprovalLevel))
                        .Cast<ApprovalLevel>()
                        .Select(r => r.ToString())
                        .ToList()
                });
            }

            return View(viewModels);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateUserRole(int userId, string newRole)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return NotFound();
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            
            
            if (currentRoles.Any())
            {
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
            }

            
            if (!string.IsNullOrEmpty(newRole))
            {
                
                var roleExists = await _roleManager.RoleExistsAsync(newRole);
                if (!roleExists)
                {
                    await _roleManager.CreateAsync(new Role
                    {
                        Name = newRole,
                        Description = $"{newRole} rolü",
                        ApprovalCount = 1,
                        ApprovalLevels = "[]" 
                    });
                }

                await _userManager.AddToRoleAsync(user, newRole);
            }

            TempData["SuccessMessage"] = "Kullanıcı rolü başarıyla güncellendi.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var role = await _roleManager.FindByIdAsync(id.ToString());
            if (role == null)
            {
                return NotFound();
            }

            var approvalLevels = new List<ApprovalLevelViewModel>();
            var selectedLevels = string.IsNullOrEmpty(role.ApprovalLevels) 
                ? new List<ApprovalLevel>() 
                : JsonSerializer.Deserialize<List<ApprovalLevel>>(role.ApprovalLevels);

            foreach (ApprovalLevel level in Enum.GetValues(typeof(ApprovalLevel)))
            {
                approvalLevels.Add(new ApprovalLevelViewModel
                {
                    Level = level,
                    IsSelected = selectedLevels?.Contains(level) ?? false
                });
            }

            var viewModel = new RoleEditViewModel
            {
                Id = role.Id.ToString(),
                Name = role.Name,
                Description = role.Description,
                IsActive = true,
                ApprovalLevels = approvalLevels
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, RoleEditViewModel model)
        {
            if (id.ToString() != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var role = await _roleManager.FindByIdAsync(id.ToString());
                if (role == null)
                {
                    return NotFound();
                }

                role.Name = model.Name;
                role.Description = model.Description;
                role.ApprovalLevels = JsonSerializer.Serialize(
                    model.ApprovalLevels
                        .Where(x => x.IsSelected)
                        .Select(x => x.Level)
                        .ToList()
                );

                var result = await _roleManager.UpdateAsync(role);
                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Rol başarıyla güncellendi.";
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }
    }
} 