using Microsoft.AspNetCore.Mvc;
using VarlikYönetimi.Core.Entities;
using VarlikYönetimi.Services.Services.Interfaces;
using VarlikYönetimi.Core.Enums;
using System.Security.Claims;

namespace VarlikYönetimi.MVC.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IPaymentService _paymentService;
        private readonly IAdvanceRequestService _advanceRequestService;
        private readonly INotificationService _notificationService;
        private readonly IUserService _userService;

        public PaymentController(
            IPaymentService paymentService,
            IAdvanceRequestService advanceRequestService,
            INotificationService notificationService,
            IUserService userService)
        {
            _paymentService = paymentService;
            _advanceRequestService = advanceRequestService;
            _notificationService = notificationService;
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var payments = await _paymentService.GetPaymentsByUserIdAsync(userId);
            return View(payments);
        }

        public async Task<IActionResult> Details(int id)
        {
            var payment = await _paymentService.GetByIdAsync(id);
            if (payment == null)
            {
                return NotFound();
            }

            return View(payment);
        }

        public async Task<IActionResult> Create(int advanceRequestId)
        {
            var request = await _advanceRequestService.GetByIdAsync(advanceRequestId);
            if (request == null)
            {
                return NotFound();
            }

            ViewBag.AdvanceRequest = request;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Payment payment)
        {
            if (ModelState.IsValid)
            {
                payment.CreatedAt = DateTime.UtcNow;
                await _paymentService.CreateAsync(payment);
                return RedirectToAction(nameof(Index));
            }

            var request = await _advanceRequestService.GetByIdAsync(payment.AdvanceRequestId);
            ViewBag.AdvanceRequest = request;
            return View(payment);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var payment = await _paymentService.GetByIdAsync(id);
            if (payment == null)
            {
                return NotFound();
            }

            return View(payment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Payment payment)
        {
            if (ModelState.IsValid)
            {
                payment.UpdatedAt = DateTime.UtcNow;
                await _paymentService.UpdateAsync(payment);
                return RedirectToAction(nameof(Index));
            }

            return View(payment);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var payment = await _paymentService.GetByIdAsync(id);
            if (payment == null)
            {
                return NotFound();
            }

            return View(payment);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _paymentService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> UpdateStatus(int id)
        {
            var payment = await _paymentService.GetByIdAsync(id);
            if (payment == null)
            {
                return NotFound();
            }

            return View(payment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, PaymentStatus newStatus)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            await _paymentService.UpdatePaymentStatusAsync(id, newStatus, userId);
            
            var payment = await _paymentService.GetByIdAsync(id);
            if (payment.Status == PaymentStatus.Paid)
            {
                var request = await _advanceRequestService.GetByIdAsync(payment.AdvanceRequestId);
                await _notificationService.SendPaymentCompletedNotificationAsync(request);
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> PendingPayments()
        {
            var payments = await _paymentService.GetPendingPaymentsAsync();
            return View(payments);
        }

        public async Task<IActionResult> OverduePayments()
        {
            var payments = await _paymentService.GetOverduePaymentsAsync();
            return View(payments);
        }
    }
} 