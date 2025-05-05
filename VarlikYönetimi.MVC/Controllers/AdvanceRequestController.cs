using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VarlikYönetimi.Core.Entities;
using VarlikYönetimi.Services.Services.Interfaces;
using VarlikYönetimi.Core.Enums;
using VarlikYönetimi.Core.ViewModels;
using VarlikYönetimi.Services.Interfaces;
using System.Security.Claims;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using VarlikYönetimi.Data.Context;
using VarlikYönetimi.Data;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace VarlikYönetimi.MVC.Controllers
{
    [Authorize]
    public class AdvanceRequestController : Controller
    {
        private readonly IAdvanceRequestService _advanceRequestService;
        private readonly IApprovalProcessService _approvalProcessService;
        private readonly IUserService _userService;
        private readonly IProjectService _projectService;
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly INotificationService _notificationService;

        public AdvanceRequestController(
            IAdvanceRequestService advanceRequestService,
            IApprovalProcessService approvalProcessService,
            IUserService userService,
            IProjectService projectService,
            AppDbContext context,
            UserManager<User> userManager,
            IWebHostEnvironment webHostEnvironment,
            INotificationService notificationService)
        {
            _advanceRequestService = advanceRequestService;
            _approvalProcessService = approvalProcessService;
            _userService = userService;
            _projectService = projectService;
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
            _notificationService = notificationService;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var requests = await _context.AdvanceRequests
                .Where(x => x.UserId == user.Id)
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new AdvanceRequestViewModel
                {
                    Id = x.Id,
                    RequestNumber = x.RequestNumber,
                    Amount = x.Amount,
                    Status = x.Status,
                    CreatedAt = x.CreatedAt,
                    RepaymentDueDate = x.RepaymentDueDate,
                    StatusText = GetStatusTextStatic(x.Status),
                    StatusBadgeClass = GetStatusBadgeClassStatic(x.Status)
                })
                .ToListAsync();

            return View(requests);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AdvanceRequest request, IFormFile Document)
        {
            if (!ModelState.IsValid)
            {
                return View(request);
            }

            try
            {
                var limits = await _context.AdvanceLimits
                    .Where(x => x.IsActive && x.ApprovalLevel == ApprovalLevel.Personel)
                    .OrderByDescending(x => x.UpdatedAt)
                    .FirstOrDefaultAsync();

                if (limits != null)
                {
                    if (request.Amount < limits.MinAmount)
                    {
                        ModelState.AddModelError("Amount", $"Avans tutarı minimum {limits.MinAmount:C2} olmalıdır.");
                        return View(request);
                    }

                    if (request.Amount > limits.MaxAmount)
                    {
                        ModelState.AddModelError("Amount", $"Avans tutarı maksimum {limits.MaxAmount:C2} olmalıdır.");
                        return View(request);
                    }
                }

                request.ValidateDesiredDate();

                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound();
                }

                request.UserId = user.Id;
                request.RequestDate = DateTime.UtcNow;
                request.Status = RequestStatus.Pending;
                request.CurrentLevel = ApprovalLevel.BirimMuduru;
                request.RequestNumber = await GenerateRequestNumber();

                if (Document != null && Document.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);
                    var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(Document.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await Document.CopyToAsync(fileStream);
                    }
                    request.DocumentPath = "/uploads/" + uniqueFileName;
                }

                await _context.AdvanceRequests.AddAsync(request);
                await _context.SaveChangesAsync();

                var notification = new Notification
                {
                    UserId = user.Id,
                    Title = "Avans Talebi Oluşturuldu",
                    Message = $"{request.Amount:C2} tutarında avans talebiniz oluşturuldu ve onay sürecine alındı.",
                    Type = NotificationType.ApprovalRequest,
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                };
                await _context.Notifications.AddAsync(notification);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Avans talebiniz başarıyla oluşturuldu.";
                return RedirectToAction(nameof(Index));
            }
            catch (ValidationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(request);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var request = await _context.AdvanceRequests
                .FirstOrDefaultAsync(r => r.Id == id && r.UserId == int.Parse(userId));

            if (request == null)
            {
                return NotFound();
            }

            return View(request);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AdvanceRequest request, IFormFile Document)
        {
            if (id != request.Id)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var existingRequest = await _context.AdvanceRequests
                .FirstOrDefaultAsync(r => r.Id == id && r.UserId == int.Parse(userId));

            if (existingRequest == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var limits = await _context.AdvanceLimits.FirstOrDefaultAsync();
                    if (limits != null)
                    {
                        request.ValidateAmount(limits.MinAmount, limits.MaxAmount);
                    }

                    request.ValidateDesiredDate();

                    existingRequest.Amount = request.Amount;
                    existingRequest.Description = request.Description;
                    existingRequest.DesiredDate = request.DesiredDate;
                    existingRequest.UpdatedAt = DateTime.Now;

                    if (Document != null && Document.Length > 0)
                    {
                        var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                        if (!Directory.Exists(uploadsFolder))
                            Directory.CreateDirectory(uploadsFolder);
                        var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(Document.FileName);
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await Document.CopyToAsync(fileStream);
                        }
                        existingRequest.DocumentPath = "/uploads/" + uniqueFileName;
                    }

                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Avans talebiniz başarıyla güncellendi.";
                    return RedirectToAction(nameof(Index));
                }
                catch (ValidationException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    return View(request);
                }
            }

            return View(request);
        }

        private async Task<string> GenerateRequestNumber()
        {
            var today = DateTime.UtcNow.Date;
            var requestCount = await _context.AdvanceRequests
                .CountAsync(x => x.RequestDate.Date == today);

            return $"AT{DateTime.UtcNow:yyyyMMdd}{(requestCount + 1):D4}";
        }

        public async Task<IActionResult> Details(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var request = await _context.AdvanceRequests
                .Include(r => r.User)
                .Include(r => r.Approvals)
                    .ThenInclude(a => a.ApproverUser)
                .FirstOrDefaultAsync(r => r.Id == id && r.UserId == int.Parse(userId));

            if (request == null)
            {
                return NotFound();
            }

            var viewModel = new AdvanceRequestViewModel
            {
                Id = request.Id,
                RequestNumber = request.RequestNumber,
                Amount = request.Amount,
                Description = request.Description,
                DesiredDate = request.DesiredDate,
                ApprovedAmount = request.ApprovedAmount,
                Status = request.Status,
                CurrentLevel = request.CurrentLevel,
                RejectionReason = request.RejectionReason,
                CreatedAt = request.RequestDate,
                UpdatedAt = request.UpdatedAt,
                ApprovedAt = request.ApprovedAt,
                RequesterName = request.User?.FullName,
                StatusBadgeClass = GetStatusBadgeClass(request.Status),
                StatusText = GetStatusText(request.Status),
                CanApprove = CanApprove(request),
                ApprovalHistory = request.Approvals?
                    .OrderByDescending(x => x.CreatedAt)
                    .Select(a => new ApprovalHistoryViewModel
                    {
                        ApproverName = a.ApproverUser?.FullName,
                        Level = a.Level.ToString(),
                        Status = a.Status.ToString(),
                        Notes = a.Comments,
                        CreatedAt = a.CreatedAt,
                        ActionText = GetActionText(a.Status),
                        UserName = a.ApproverUser?.FullName
                    }).ToList()
            };

            return View(viewModel);
        }

        private static string GetStatusTextStatic(RequestStatus status)
        {
            return status switch
            {
                RequestStatus.Pending => "Beklemede",
                RequestStatus.Approved => "Onaylandı",
                RequestStatus.Rejected => "Reddedildi",
                _ => "Bilinmiyor"
            };
        }

        private static string GetStatusBadgeClassStatic(RequestStatus status)
        {
            return status switch
            {
                RequestStatus.Pending => "badge bg-warning",
                RequestStatus.Approved => "badge bg-success",
                RequestStatus.Rejected => "badge bg-danger",
                _ => "badge bg-secondary"
            };
        }

        private string GetStatusText(RequestStatus status)
        {
            return status switch
            {
                RequestStatus.Pending => "Beklemede",
                RequestStatus.Approved => "Onaylandı",
                RequestStatus.Rejected => "Reddedildi",
                RequestStatus.AvansGeriOdenmeyiBekliyor => "Geri Ödeme Bekliyor",
                RequestStatus.HukukiIslemBaslatildi => "Hukuki İşlem",
                _ => status.ToString()
            };
        }

        private string GetStatusBadgeClass(RequestStatus status)
        {
            return status switch
            {
                RequestStatus.Pending => "warning",
                RequestStatus.Approved => "success",
                RequestStatus.Rejected => "danger",
                RequestStatus.AvansGeriOdenmeyiBekliyor => "warning",
                RequestStatus.HukukiIslemBaslatildi => "danger",
                _ => "secondary"
            };
        }

        private bool CanApprove(AdvanceRequest request)
        {
            var currentUser = _userManager.GetUserAsync(User).Result;
            if (currentUser == null) return false;

            var userRoles = _userManager.GetRolesAsync(currentUser).Result;
            if (!userRoles.Any()) return false;

            var currentLevel = request.CurrentLevel;
            var userRole = userRoles.First();

            return (currentLevel == ApprovalLevel.BirimMuduru && userRole == "BirimMuduru") ||
                   (currentLevel == ApprovalLevel.Direktor && userRole == "Direktor") ||
                   (currentLevel == ApprovalLevel.GenelMudurYardimcisi && userRole == "GenelMudurYardimcisi") ||
                   (currentLevel == ApprovalLevel.GenelMudur && userRole == "GenelMudur") ||
                   (currentLevel == ApprovalLevel.FinansMuduru && userRole == "FinansMuduru") ||
                   (currentLevel == ApprovalLevel.OnMuhasebe && userRole == "OnMuhasebe");
        }

        private string GetActionText(ApprovalStatus status)
        {
            return status switch
            {
                ApprovalStatus.Approved => "Onaylandı",
                ApprovalStatus.Rejected => "Reddedildi",
                ApprovalStatus.Pending => "Beklemede",
                _ => status.ToString()
            };
        }

        [HttpPost]
        public async Task<IActionResult> Approve(int id, string notes)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var result = await _advanceRequestService.ApproveRequestAsync(id, userId, 0, notes);
            
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
        public async Task<IActionResult> Reject(int id, string notes)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var result = await _advanceRequestService.RejectRequestAsync(id, userId, notes);
            
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