using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VarlikYönetimi.Core.Entities;
using VarlikYönetimi.Core.Enums;
using VarlikYönetimi.Core.ViewModels;
using VarlikYönetimi.Services.Services.Interfaces;

namespace VarlikYönetimi.MVC.Controllers
{
    [Authorize]
    public class ApprovalProcessController : Controller
    {
        private readonly IApprovalProcessService _approvalProcessService;
        private readonly IAdvanceRequestService _advanceRequestService;
        private readonly IUserService _userService;

        public ApprovalProcessController(
            IApprovalProcessService approvalProcessService,
            IAdvanceRequestService advanceRequestService,
            IUserService userService)
        {
            _approvalProcessService = approvalProcessService;
            _advanceRequestService = advanceRequestService;
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
            var user = await _userService.GetUserWithRolesAsync(userId);
            var userRoles = await _userService.GetUserRolesAsync(userId);

            var approvalProcesses = await _approvalProcessService.GetPendingApprovalsAsync(userId);
            var viewModels = approvalProcesses.Select(ap => new ApprovalProcessViewModel
            {
                Id = ap.Id,
                RequestNumber = ap.AdvanceRequest?.RequestNumber,
                RequesterName = ap.AdvanceRequest?.User?.FullName,
                Amount = ap.AdvanceRequest?.Amount ?? 0,
                CurrentLevel = ap.AdvanceRequest?.CurrentLevel ?? ApprovalLevel.Personel,
                Status = ap.AdvanceRequest?.Status ?? RequestStatus.Pending,
                UpdatedAt = ap.UpdatedAt
            });

            return View(viewModels);
        }

        public async Task<IActionResult> Details(int id)
        {
            var approvalProcess = await _approvalProcessService.GetByIdAsync(id);
            if (approvalProcess == null)
            {
                return NotFound();
            }

            var viewModel = new ApprovalProcessViewModel
            {
                Id = approvalProcess.Id,
                RequestNumber = approvalProcess.AdvanceRequest?.RequestNumber,
                RequesterName = approvalProcess.AdvanceRequest?.User?.FullName,
                Amount = approvalProcess.AdvanceRequest?.Amount ?? 0,
                CurrentLevel = approvalProcess.AdvanceRequest?.CurrentLevel ?? ApprovalLevel.Personel,
                Status = approvalProcess.AdvanceRequest?.Status ?? RequestStatus.Pending,
                UpdatedAt = approvalProcess.UpdatedAt
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id, string notes)
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
            var result = await _approvalProcessService.ApproveAsync(id, userId, notes);
            
            if (result)
            {
                TempData["SuccessMessage"] = "Talep başarıyla onaylandı.";
            }
            else
            {
                TempData["ErrorMessage"] = "Talep onaylanırken bir hata oluştu.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id, string notes)
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
            var result = await _approvalProcessService.RejectAsync(id, userId, notes);
            
            if (result)
            {
                TempData["SuccessMessage"] = "Talep başarıyla reddedildi.";
            }
            else
            {
                TempData["ErrorMessage"] = "Talep reddedilirken bir hata oluştu.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
} 