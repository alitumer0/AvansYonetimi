using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VarlikYönetimi.Services.Interfaces;
using VarlikYönetimi.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using VarlikYönetimi.Core.ViewModels;
using Microsoft.AspNetCore.Identity;
using VarlikYönetimi.Services.Services.Interfaces;
using System.Linq;

namespace VarlikYönetimi.MVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AccountController(
            IUserService userService,
            UserManager<User> userManager,
            SignInManager<User> signInManager)
        {
            _userService = userService;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null) //her birim için önce farklı area düşünüldü tabii sonra bundan vazgeçildi Todo:
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    var roles = await _userManager.GetRolesAsync(user);

                    if (roles.Contains("Admin"))
                    {
                        return RedirectToAction("Index", "Home", new { area = "Admin" });
                    }
                    else if (roles.Contains("BM"))
                    {
                        return RedirectToAction("Index", "Home", new { area = "BM" });
                    }
                    else if (roles.Contains("GMY"))
                    {
                        return RedirectToAction("Index", "Home", new { area = "GMY" });
                    }
                    else if (roles.Contains("OM"))
                    {
                        return RedirectToAction("Index", "Home", new { area = "OM" });
                    }
                    else if (roles.Contains("Direktor"))
                    {
                        return RedirectToAction("Index", "Home", new { area = "Direktor" });
                    }
                    else if (roles.Contains("GenelMudur"))
                    {
                        return RedirectToAction("Index", "Home", new { area = "GM" });
                    }
                    else if (roles.Contains("FinansMuduru"))
                    {
                        return RedirectToAction("Index", "Home", new { area = "FM" });
                    }
                    else if (roles.Contains("Personel"))
                    {
                        return RedirectToAction("Index", "Home", new { area = "Personel" });
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Geçersiz giriş denemesi.");
                    return View(model);
                }
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    UserName = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                var result = await _userManager.CreateAsync(user, model.Password);
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

            return View(model);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var model = new UserProfileViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber
            };

            return View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Profile(UserProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.PhoneNumber = model.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Profil güncellenirken bir hata oluştu");
                return View(model);
            }

            TempData["SuccessMessage"] = "Profil başarıyla güncellendi";
            return RedirectToAction("Profile");
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Profile", new UserProfileViewModel { ChangePasswordModel = model });
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Şifre değiştirilirken bir hata oluştu");
                return View("Profile", new UserProfileViewModel { ChangePasswordModel = model });
            }

            await _signInManager.RefreshSignInAsync(user);
            TempData["SuccessMessage"] = "Şifre başarıyla değiştirildi";
            return RedirectToAction("Profile");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                var user = _userManager.GetUserAsync(User).Result;
                if (user != null)
                {
                    var roles = _userManager.GetRolesAsync(user).Result;
                    if (roles.Contains("Admin"))
                    {
                        return RedirectToAction("Index", "Home", new { area = "Admin" });
                    }
                    else if (roles.Contains("BM"))
                    {
                        return RedirectToAction("Index", "Home", new { area = "BM" });
                    }
                    else if (roles.Contains("GMY"))
                    {
                        return RedirectToAction("Index", "Home", new { area = "GMY" });
                    }
                    else if (roles.Contains("OM"))
                    {
                        return RedirectToAction("Index", "Home", new { area = "OM" });
                    }
                    else if (roles.Contains("Direktor"))
                    {
                        return RedirectToAction("Index", "Home", new { area = "Direktor" });
                    }
                    else if (roles.Contains("GenelMudur"))
                    {
                        return RedirectToAction("Index", "Home", new { area = "GM" });
                    }
                    else if (roles.Contains("FinansMuduru"))
                    {
                        return RedirectToAction("Index", "Home", new { area = "FM" });
                    }
                    else if (roles.Contains("Personel"))
                    {
                        return RedirectToAction("Index", "Home", new { area = "Personel" });
                    }
                }
                return RedirectToAction("Index", "Home");
            }
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
} 