using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VarlikYönetimi.Core.Entities;
using VarlikYönetimi.Services.Services.Interfaces;
using VarlikYönetimi.MVC.Models;
using VarlikYönetimi.Data.Context;
using VarlikYönetimi.Core.Enums;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using VarlikYönetimi.Core.ViewModels;

namespace VarlikYönetimi.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly IAdvanceRequestService _advanceRequestService;
        private readonly INotificationService _notificationService;
        private readonly IUserService _userService;
        private readonly IPaymentService _paymentService;
        private readonly AppDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(
            IAdvanceRequestService advanceRequestService,
            INotificationService notificationService,
            IUserService userService,
            IPaymentService paymentService,
            AppDbContext context,
            ILogger<HomeController> logger)
        {
            _advanceRequestService = advanceRequestService;
            _notificationService = notificationService;
            _userService = userService;
            _paymentService = paymentService;
            _context = context;
            _logger = logger;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return View();
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var user = await _userService.GetUserWithRolesAsync(userId);
            var userRoles = await _userService.GetUserRolesAsync(userId);

            
            if (userRoles.Contains("Admin"))
            {
                
                ViewBag.TotalUsers = await _context.Users.CountAsync();
                ViewBag.TotalRequests = await _context.AdvanceRequests.CountAsync();
                ViewBag.PendingApprovals = await _context.AdvanceRequests
                    .Where(r => r.Status == RequestStatus.Pending)
                    .CountAsync();
                ViewBag.TotalPayments = await _context.Payments.CountAsync();
            }
            else if (userRoles.Contains("Mali İşler"))
            {
                
                ViewBag.PendingPayments = await _context.Payments
                    .Where(p => p.Status == PaymentStatus.Pending)
                    .CountAsync();
                ViewBag.TotalPayments = await _context.Payments.CountAsync();
                ViewBag.RecentPayments = await _context.Payments
                    .OrderByDescending(p => p.CreatedAt)
                    .Take(5)
                    .ToListAsync();
            }
            else
            {
                
                ViewBag.UserAdvanceRequests = await _context.AdvanceRequests
                    .Where(r => r.UserId == userId)
                    .OrderByDescending(r => r.CreatedAt)
                    .Take(5)
                    .ToListAsync();

                ViewBag.UserPayments = await _context.Payments
                    .Where(p => p.AdvanceRequest.UserId == userId)
                    .OrderByDescending(p => p.CreatedAt)
                    .Take(5)
                    .ToListAsync();
            }

            
            ViewBag.RecentNotifications = await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .Take(5)
                .ToListAsync();

            ViewBag.UserRoles = userRoles;

            return View();
        }

        [Authorize]
        public async Task<IActionResult> Dashboard()
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
            
            var pendingRequests = await _advanceRequestService.GetPendingApprovalsAsync(0);
            var pendingPayments = await _paymentService.GetPendingPaymentsAsync();
            var overduePayments = await _paymentService.GetOverduePaymentsAsync();
            var unreadNotificationCount = await _notificationService.GetUnreadCountAsync(userId);

            var dashboardViewModel = new DashboardViewModel
            {
                PendingRequests = pendingRequests,
                PendingPayments = pendingPayments,
                OverduePayments = overduePayments,
                UnreadNotificationCount = unreadNotificationCount
            };

            return View(dashboardViewModel);
        }

        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
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
        public async Task<IActionResult> Settings()
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
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
        public async Task<IActionResult> ChangePassword(string currentPassword, string newPassword)
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
            var result = await _userService.ChangePasswordAsync(userId, currentPassword, newPassword);

            if (result)
            {
                TempData["SuccessMessage"] = "Şifre başarıyla değiştirildi.";
            }
            else
            {
                TempData["ErrorMessage"] = "Mevcut şifre yanlış veya yeni şifre geçersiz.";
            }

            return RedirectToAction(nameof(Settings));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
