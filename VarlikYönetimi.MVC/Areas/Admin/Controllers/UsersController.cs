using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VarlikYönetimi.Core.Entities;
using VarlikYönetimi.Services.Services.Interfaces;
using VarlikYönetimi.MVC.Models;

namespace VarlikYönetimi.MVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;

        public UsersController(IUserService userService, IRoleService roleService)
        {
            _userService = userService;
            _roleService = roleService;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userService.GetAllUsersAsync();
            var usersWithRoles = new List<UserViewModel>();

            foreach (var user in users)
            {
                var roles = await _userService.GetUserRolesAsync(user.Id);
                usersWithRoles.Add(new UserViewModel
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    IsActive = user.IsActive,
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = user.UpdatedAt,
                    RoleNames = roles
                });
            }

            return View(usersWithRoles);
        }

        public async Task<IActionResult> Details(int id)
        {
            var user = await _userService.GetUserWithRolesAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userService.GetUserRolesAsync(id);
            var viewModel = new UserViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                RoleNames = roles
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var user = await _userService.GetUserWithRolesAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userService.GetUserRolesAsync(id);
            var viewModel = new UserViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                IsActive = user.IsActive,
                RoleNames = roles
            };

            ViewBag.Roles = await _roleService.GetAllRolesAsync();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromBody] UserViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _userService.GetByIdAsync(model.Id);
                    if (user == null)
                    {
                        return NotFound("Kullanıcı bulunamadı.");
                    }

                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.Email = model.Email;
                    user.PhoneNumber = model.PhoneNumber;
                    user.UpdatedAt = DateTime.UtcNow;

                    var result = await _userService.UpdateAsync(user);
                    if (result)
                    {
                    return Ok();
                    }
                    return BadRequest("Kullanıcı güncellenemedi.");
                }
                catch (Exception ex)
                {
                    return BadRequest($"Kullanıcı güncellenirken bir hata oluştu: {ex.Message}");
                }
            }

            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            return BadRequest($"Geçersiz model durumu: {string.Join(", ", errors)}");
        }

        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userService.GetUserWithRolesAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userService.GetUserRolesAsync(id);
            var viewModel = new UserViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                RoleNames = roles
            };

            return View(viewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _userService.DeleteAsync(id);
            TempData["SuccessMessage"] = "Kullanıcı başarıyla silindi.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Roles(int id)
        {
            var user = await _userService.GetUserWithRolesAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userService.GetUserRolesAsync(id);
            var viewModel = new UserViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                IsActive = user.IsActive,
                RoleNames = roles
            };

            ViewBag.Roles = await _roleService.GetAllRolesAsync();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateRoles(int id, List<int> roleIds)
        {
            var user = await _userService.GetUserWithRolesAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            try
            {
                
                var currentRoles = await _userService.GetUserRolesAsync(id);
                foreach (var role in currentRoles)
                {
                    var roleEntity = await _roleService.GetRoleByNameAsync(role);
                    if (roleEntity != null)
                    {
                        await _userService.RemoveUserRoleAsync(id, roleEntity.Id);
                    }
                }

                
                foreach (var roleId in roleIds)
                {
                    await _userService.AddUserRoleAsync(id, roleId);
                }

                TempData["SuccessMessage"] = "Kullanıcı rolleri başarıyla güncellendi.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Roller güncellenirken bir hata oluştu.");
                ViewBag.Roles = await _roleService.GetAllRolesAsync();
                return View("Roles", new UserViewModel { Id = id });
            }
        }
    }
} 