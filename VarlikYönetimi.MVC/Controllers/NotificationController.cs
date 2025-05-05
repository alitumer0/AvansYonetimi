using Microsoft.AspNetCore.Mvc;
using VarlikYönetimi.Core.Entities;
using VarlikYönetimi.Services.Services.Interfaces;
using System.Security.Claims;

namespace VarlikYönetimi.MVC.Controllers
{
    public class NotificationController : Controller
    {
        private readonly INotificationService _notificationService;
        private readonly IUserService _userService;

        public NotificationController(
            INotificationService notificationService,
            IUserService userService)
        {
            _notificationService = notificationService;
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var notifications = await _notificationService.GetByUserIdAsync(userId);
            return View(notifications);
        }

        public async Task<IActionResult> Details(int id)
        {
            var notification = await _notificationService.GetByIdAsync(id);
            if (notification == null)
            {
                return NotFound();
            }

            if (!notification.IsRead)
            {
                await _notificationService.MarkAsReadAsync(id);
            }

            return View(notification);
        }

        [HttpPost]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            await _notificationService.MarkAsReadAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            await _notificationService.MarkAllAsReadAsync(userId);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> GetUnreadNotifications()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var notifications = await _notificationService.GetUnreadNotificationsAsync(userId);
            return Json(notifications);
        }

        [HttpGet]
        public async Task<IActionResult> GetUnreadCount()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var count = await _notificationService.GetUnreadCountAsync(userId);
            return Json(new { count });
        }
    }
} 