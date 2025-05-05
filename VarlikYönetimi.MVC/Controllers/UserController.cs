using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VarlikYönetimi.Core.Entities;
using VarlikYönetimi.Services.Services.Interfaces;
using System.Security.Claims;

namespace VarlikYönetimi.MVC.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        public UserController(
            IUserService userService,
            IRoleService roleService,
            SignInManager<User> signInManager,
            UserManager<User> userManager)
        {
            _userService = userService;
            _roleService = roleService;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password, bool rememberMe)
        {
            var result = await _signInManager.PasswordSignInAsync(username, password, rememberMe, lockoutOnFailure: false);
            
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            
            ModelState.AddModelError(string.Empty, "Geçersiz kullanıcı adı veya şifre.");
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(User user, string password)
        {
            if (ModelState.IsValid)
            {
                var result = await _userManager.CreateAsync(user, password);
                
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
                
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var users = await _userService.GetAllUsersAsync();
            return View(users);
        }

        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [Authorize]
        public async Task<IActionResult> Create()
        {
            var roles = await _roleService.GetAllAsync();
            ViewBag.Roles = roles;
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(User user)
        {
            if (ModelState.IsValid)
            {
                await _userService.AddAsync(user);
                return RedirectToAction(nameof(Index));
            }

            var roles = await _roleService.GetAllAsync();
            ViewBag.Roles = roles;
            return View(user);
        }

        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _roleService.GetAllAsync();
            ViewBag.Roles = roles;
            return View(user);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(User user)
        {
            if (ModelState.IsValid)
            {
                await _userService.UpdateAsync(user);
                return RedirectToAction(nameof(Index));
            }

            var roles = await _roleService.GetAllAsync();
            ViewBag.Roles = roles;
            return View(user);
        }

        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user != null)
            {
                await _userService.RemoveAsync(user);
            }
            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        public async Task<IActionResult> AssignRole(int userId)
        {
            var user = await _userService.GetByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _roleService.GetAllAsync();
            ViewBag.Roles = roles;
            return View(user);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignRole(int userId, int roleId)
        {
            await _userService.AddUserRoleAsync(userId, roleId);
            return RedirectToAction(nameof(Details), new { id = userId });
        }

        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = await _userService.GetUserWithRolesAsync(userId);
            
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(User user)
        {
            if (ModelState.IsValid)
            {
                await _userService.UpdateAsync(user);
                TempData["SuccessMessage"] = "Profil başarıyla güncellendi.";
                return RedirectToAction(nameof(Profile));
            }

            return View("Profile", user);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(string currentPassword, string newPassword)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var result = await _userService.ChangePasswordAsync(userId, currentPassword, newPassword);

            if (result)
            {
                TempData["SuccessMessage"] = "Şifre başarıyla değiştirildi.";
            }
            else
            {
                TempData["ErrorMessage"] = "Mevcut şifre yanlış veya yeni şifre geçersiz.";
            }

            return RedirectToAction(nameof(Profile));
        }
    }
} 