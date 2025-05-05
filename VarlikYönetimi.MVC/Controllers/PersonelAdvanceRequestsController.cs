using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VarlikYönetimi.Core.Entities;
using VarlikYönetimi.Core.Enums;
using VarlikYönetimi.Data.Context;
using VarlikYönetimi.Services.Services.Interfaces;
using VarlikYönetimi.Core.ViewModels;
using System.Text.Json;

namespace VarlikYönetimi.MVC.Controllers
{
    [Authorize]
    public class PersonelAdvanceRequestsController : Controller
    {
        private readonly ILogger<PersonelAdvanceRequestsController> _logger;
        private readonly UserManager<User> _userManager;
        private readonly AppDbContext _context;
        private readonly INotificationService _notificationService;
        private readonly IAdvanceLimitService _advanceLimitService;

        public PersonelAdvanceRequestsController(
            AppDbContext context, 
            ILogger<PersonelAdvanceRequestsController> logger, 
            UserManager<User> userManager,
            INotificationService notificationService,
            IAdvanceLimitService advanceLimitService)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
            _notificationService = notificationService;
            _advanceLimitService = advanceLimitService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound();
                }

                var userRoles = await _userManager.GetRolesAsync(user);
                var requests = new List<AdvanceRequest>();

                if (userRoles.Contains("Admin"))
                {
                    requests = await _context.AdvanceRequests
                        .Include(x => x.User)
                        .OrderByDescending(x => x.RequestDate)
                        .ToListAsync();
                }
                else if (userRoles.Contains("OnMuhasebe"))
                {
                    requests = await _context.AdvanceRequests
                        .Include(x => x.User)
                        .OrderByDescending(x => x.RequestDate)
                        .ToListAsync();
                }
                else if (userRoles.Contains("FinansMuduru"))
                {
                    requests = await _context.AdvanceRequests
                        .Include(x => x.User)
                        .Where(x => x.CurrentLevel == ApprovalLevel.FinansMuduru || 
                               x.CurrentLevel == ApprovalLevel.GenelMudur ||
                               x.CurrentLevel == ApprovalLevel.GenelMudurYardimcisi ||
                               x.CurrentLevel == ApprovalLevel.Direktor ||
                               x.CurrentLevel == ApprovalLevel.BirimMuduru)
                        .OrderByDescending(x => x.RequestDate)
                        .ToListAsync();
                }
                else if (userRoles.Contains("GenelMudur"))
                {
                    requests = await _context.AdvanceRequests
                        .Include(x => x.User)
                        .Where(x => x.CurrentLevel == ApprovalLevel.GenelMudur ||
                               x.CurrentLevel == ApprovalLevel.GenelMudurYardimcisi ||
                               x.CurrentLevel == ApprovalLevel.Direktor ||
                               x.CurrentLevel == ApprovalLevel.BirimMuduru)
                        .OrderByDescending(x => x.RequestDate)
                        .ToListAsync();
                }
                else if (userRoles.Contains("GenelMudurYardimcisi"))
                {
                    requests = await _context.AdvanceRequests
                        .Include(x => x.User)
                        .Where(x => x.CurrentLevel == ApprovalLevel.GenelMudurYardimcisi ||
                               x.CurrentLevel == ApprovalLevel.Direktor ||
                               x.CurrentLevel == ApprovalLevel.BirimMuduru)
                        .OrderByDescending(x => x.RequestDate)
                        .ToListAsync();
                }
                else if (userRoles.Contains("Direktor"))
                {
                    requests = await _context.AdvanceRequests
                        .Include(x => x.User)
                        .Where(x => x.CurrentLevel == ApprovalLevel.Direktor ||
                               x.CurrentLevel == ApprovalLevel.BirimMuduru)
                        .OrderByDescending(x => x.RequestDate)
                        .ToListAsync();
                }
                else if (userRoles.Contains("BirimMuduru"))
                {
                    requests = await _context.AdvanceRequests
                        .Include(x => x.User)
                        .Where(x => x.CurrentLevel == ApprovalLevel.BirimMuduru)
                        .OrderByDescending(x => x.RequestDate)
                        .ToListAsync();
                }
                else
                {
                    requests = await _context.AdvanceRequests
                        .Include(x => x.User)
                        .Where(x => x.UserId == user.Id)
                        .OrderByDescending(x => x.RequestDate)
                        .ToListAsync();
                }

                var viewModels = requests.Select(r => new AdvanceRequestViewModel
                {
                    Id = r.Id,
                    RequestNumber = r.RequestNumber,
                    Amount = r.Amount,
                    Description = r.Description,
                    DesiredDate = r.DesiredDate,
                    ApprovedAmount = r.ApprovedAmount,
                    Status = r.Status,
                    CurrentLevel = r.CurrentLevel,
                    RejectionReason = r.RejectionReason,
                    CreatedAt = r.RequestDate,
                    UpdatedAt = r.UpdatedAt,
                    ApprovedAt = r.ApprovedAt,
                    RequesterName = r.User?.FullName,
                    StatusBadgeClass = GetStatusBadgeClass(r.Status),
                    StatusText = GetStatusText(r.Status),
                    CanApprove = CanApprove(r)
                }).ToList();

                return View(viewModels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Avans talepleri listelenirken hata oluştu");
                TempData["ErrorMessage"] = "Avans talepleri listelenirken bir hata oluştu. Lütfen daha sonra tekrar deneyin.";
                return View(new List<AdvanceRequestViewModel>());
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            var advanceRequest = await _context.AdvanceRequests
                .Include(a => a.User)
                .Include(a => a.Approvals)
                    .ThenInclude(a => a.ApproverUser)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (advanceRequest == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            var userRoles = await _userManager.GetRolesAsync(currentUser);

            var viewModel = new AdvanceRequestViewModel
            {
                Id = advanceRequest.Id,
                RequestNumber = advanceRequest.RequestNumber,
                Amount = advanceRequest.Amount,
                Description = advanceRequest.Description,
                DesiredDate = advanceRequest.DesiredDate,
                ApprovedAmount = advanceRequest.ApprovedAmount,
                Status = advanceRequest.Status,
                CurrentLevel = advanceRequest.CurrentLevel,
                RejectionReason = advanceRequest.RejectionReason,
                CreatedAt = advanceRequest.RequestDate,
                UpdatedAt = advanceRequest.UpdatedAt,
                ApprovedAt = advanceRequest.ApprovedAt,
                RequesterName = advanceRequest.User?.FullName,
                StatusBadgeClass = GetStatusBadgeClass(advanceRequest.Status),
                StatusText = GetStatusText(advanceRequest.Status),
                CanApprove = CanApprove(advanceRequest),
                ApprovalHistory = advanceRequest.Approvals?
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

        private string GetStatusBadgeClass(RequestStatus status)
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
                _ => "Bilinmiyor"
            };
        }

        private bool CanApprove(AdvanceRequest request)
        {
            var currentUser = _userManager.GetUserAsync(User).Result;
            if (currentUser == null) return false;

            if (request.UserId == currentUser.Id) return false;

            var userRoles = _userManager.GetRolesAsync(currentUser).Result;
            if (!userRoles.Any()) return false;

            var currentLevel = request.CurrentLevel;
            var userRole = userRoles.First();

            switch (userRole)
            {
                case "BirimMuduru":
                    return currentLevel == ApprovalLevel.BirimMuduru;
                case "Direktor":
                    return currentLevel == ApprovalLevel.BirimMuduru || currentLevel == ApprovalLevel.Direktor;
                case "GenelMudurYardimcisi":
                    return currentLevel == ApprovalLevel.Direktor || currentLevel == ApprovalLevel.GenelMudurYardimcisi;
                case "GenelMudur":
                    return currentLevel == ApprovalLevel.GenelMudurYardimcisi || currentLevel == ApprovalLevel.GenelMudur;
                case "FinansMuduru":
                    return currentLevel == ApprovalLevel.GenelMudur || currentLevel == ApprovalLevel.FinansMuduru;
                case "OnMuhasebe":
                    return currentLevel == ApprovalLevel.FinansMuduru || currentLevel == ApprovalLevel.OnMuhasebe;
                default:
                    return false;
            }
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id, string notes)
        {
            if (!Request.Headers["X-Requested-With"].Equals("XMLHttpRequest"))
            {
                return Json(new { success = false, message = "Bu işlem sadece AJAX çağrıları ile yapılabilir." });
            }

            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Json(new { success = false, message = "Kullanıcı bulunamadı." });
                }

                var advanceRequest = await _context.AdvanceRequests
                    .Include(x => x.User)
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (advanceRequest == null)
                {
                    return Json(new { success = false, message = "Avans talebi bulunamadı." });
                }

                if (advanceRequest.Status != RequestStatus.Pending)
                {
                    return Json(new { success = false, message = "Bu talep zaten onaylanmış veya reddedilmiş." });
                }

                var userRoles = await _userManager.GetRolesAsync(user);
                var currentRole = userRoles.FirstOrDefault();
                var currentLevel = Enum.TryParse<ApprovalLevel>(currentRole, out var level) ? level : ApprovalLevel.None;

                if (!CanApprove(advanceRequest))
                {
                    return Json(new { success = false, message = "Bu talebi onaylama yetkiniz bulunmamaktadır." });
                }

                var departmentLimits = await _advanceLimitService.GetByDepartmentAndLevelAsync(advanceRequest.User?.DepartmentId, currentLevel);
                var generalLimits = await _advanceLimitService.GetByDepartmentAndLevelAsync(null, currentLevel);

                var maxLimit = departmentLimits?.MaxAmount ?? generalLimits?.MaxAmount;

                if (maxLimit.HasValue && advanceRequest.Amount > maxLimit.Value)
                {
                    return Json(new { 
                        success = false, 
                        message = $"Bu tutarı onaylama yetkiniz bulunmamaktadır. Sizin onaylayabileceğiniz maksimum tutar: {maxLimit.Value:C2}"
                    });
                }

                advanceRequest.Status = RequestStatus.Approved;
                advanceRequest.ApprovedBy = user.Id.ToString();
                advanceRequest.ApprovedAt = DateTime.UtcNow;
                advanceRequest.UpdatedAt = DateTime.UtcNow;
                advanceRequest.UpdatedBy = user.Id;

                var nextLevel = await GetNextApprovalLevel(advanceRequest.Amount, currentLevel);
                if (nextLevel != currentLevel)
                {
                    advanceRequest.Status = RequestStatus.Pending;
                    advanceRequest.CurrentLevel = nextLevel;
                }

                var approval = new ApprovalProcess
                {
                    AdvanceRequestId = advanceRequest.Id,
                    ApproverUserId = user.Id,
                    Level = currentLevel,
                    Status = ApprovalStatus.Approved,
                    Comments = notes,
                    CreatedAt = DateTime.UtcNow
                };

                await _context.ApprovalProcesses.AddAsync(approval);
                _context.AdvanceRequests.Update(advanceRequest);
                await _context.SaveChangesAsync();

                var message = nextLevel != currentLevel ?
                    $"#{advanceRequest.RequestNumber} numaralı avans talebi {user.FullName} tarafından onaylandı ve {nextLevel} seviyesine iletildi." :
                    $"#{advanceRequest.RequestNumber} numaralı avans talebi {user.FullName} tarafından onaylandı.";

                await _notificationService.CreateAsync(new Notification
                {
                    Title = "Avans Talebi Onaylandı",
                    Message = message,
                    UserId = advanceRequest.UserId,
                    Type = NotificationType.ApprovalCompleted,
                    CreatedAt = DateTime.UtcNow
                });

                var adminUsers = await _userManager.GetUsersInRoleAsync("Admin");
                foreach (var adminUser in adminUsers)
                {
                    await _notificationService.CreateAsync(new Notification
                    {
                        Title = "Avans Talebi Onaylandı",
                        Message = message,
                        UserId = adminUser.Id,
                        Type = NotificationType.ApprovalCompleted,
                        CreatedAt = DateTime.UtcNow
                    });
                }

                return Json(new { success = true, redirectUrl = Url.Action("Index", "PersonelAdvanceRequests") });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Avans talebi onaylanırken hata oluştu.");
                return Json(new { success = false, message = "İşlem sırasında bir hata oluştu." });
            }
        }

        private async Task<List<LimitCheckResult>> CheckLimits(AdvanceRequest request, ApprovalLevel currentLevel)
        {
            var results = new List<LimitCheckResult>();

            var allLevels = new[]
            {
                ApprovalLevel.BirimMuduru,
                ApprovalLevel.Direktor,
                ApprovalLevel.GenelMudurYardimcisi,
                ApprovalLevel.GenelMudur,
                ApprovalLevel.FinansMuduru,
                ApprovalLevel.OnMuhasebe
            };

            var applicableLevels = allLevels.TakeWhile(level => (int)level <= (int)currentLevel);
            decimal? minLimit = null;
            decimal? maxLimit = null;

            foreach (var level in applicableLevels)
            {
                var departmentLimits = await _advanceLimitService.GetByDepartmentAndLevelAsync(request.User?.DepartmentId, level);
                if (departmentLimits != null)
                {
                    if (!minLimit.HasValue || departmentLimits.MinAmount < minLimit)
                        minLimit = departmentLimits.MinAmount;
                    
                    if (!maxLimit.HasValue || departmentLimits.MaxAmount > maxLimit)
                        maxLimit = departmentLimits.MaxAmount;
                }

                var generalLimits = await _advanceLimitService.GetByDepartmentAndLevelAsync(null, level);
                if (generalLimits != null)
                {
                    if (!minLimit.HasValue || generalLimits.MinAmount < minLimit)
                        minLimit = generalLimits.MinAmount;
                    
                    if (!maxLimit.HasValue || generalLimits.MaxAmount > maxLimit)
                        maxLimit = generalLimits.MaxAmount;
                }
            }

            if (minLimit.HasValue && maxLimit.HasValue)
            {
                var isExceeded = request.Amount < minLimit || request.Amount > maxLimit;
                results.Add(new LimitCheckResult
                {
                    Level = currentLevel,
                    IsDepartmentLimit = true,
                    IsExceeded = isExceeded,
                    Message = isExceeded ? 
                        $"Onay yetkiniz ({minLimit:C2} - {maxLimit:C2}) dışında olduğu için onaylayamazsınız." :
                        $"Onay limitiniz içinde: {minLimit:C2} - {maxLimit:C2}"
                });

                if (!isExceeded)
                {
                    var nextLevels = allLevels.SkipWhile(level => (int)level <= (int)currentLevel);
                    foreach (var nextLevel in nextLevels)
                    {
                        var nextLevelLimits = await _advanceLimitService.GetByDepartmentAndLevelAsync(request.User?.DepartmentId, nextLevel);
                        if (nextLevelLimits != null)
                        {
                            results.Add(new LimitCheckResult
                            {
                                Level = nextLevel,
                                IsDepartmentLimit = true,
                                IsExceeded = false,
                                Message = $"Bir sonraki onay seviyesi {nextLevel}: {nextLevelLimits.MinAmount:C2} - {nextLevelLimits.MaxAmount:C2}"
                            });
                            break;
                        }
                    }
                }
            }

            return results;
        }

        private async Task<ApprovalLevel> GetNextApprovalLevel(decimal amount, ApprovalLevel currentLevel)
        {
            var allLevels = new[]
            {
                ApprovalLevel.BirimMuduru,
                ApprovalLevel.Direktor,
                ApprovalLevel.GenelMudurYardimcisi,
                ApprovalLevel.GenelMudur,
                ApprovalLevel.FinansMuduru,
                ApprovalLevel.OnMuhasebe
            };

            var nextLevels = allLevels.SkipWhile(level => (int)level <= (int)currentLevel);
            
            foreach (var nextLevel in nextLevels)
            {
                var limits = await _advanceLimitService.GetByDepartmentAndLevelAsync(null, nextLevel);
                if (limits != null && amount > limits.MinAmount)
                {
                    return nextLevel;
                }
            }

            return currentLevel;
        }

        private class LimitCheckResult
        {
            public ApprovalLevel Level { get; set; }
            public bool IsDepartmentLimit { get; set; }
            public bool IsExceeded { get; set; }
            public string Message { get; set; }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id, string notes)
        {
            if (!Request.Headers["X-Requested-With"].Equals("XMLHttpRequest"))
            {
                return Json(new { success = false, message = "Bu işlem sadece AJAX çağrıları ile yapılabilir." });
            }

            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Json(new { success = false, message = "Kullanıcı bulunamadı." });
                }

                var advanceRequest = await _context.AdvanceRequests
                    .Include(x => x.User)
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (advanceRequest == null)
                {
                    return Json(new { success = false, message = "Avans talebi bulunamadı." });
                }

                if (advanceRequest.Status != RequestStatus.Pending)
                {
                    return Json(new { success = false, message = "Bu talep zaten onaylanmış veya reddedilmiş." });
                }

                if (string.IsNullOrWhiteSpace(notes))
                {
                    return Json(new { success = false, message = "Red nedeni belirtilmelidir." });
                }

                var userRoles = await _userManager.GetRolesAsync(user);
                var currentRole = userRoles.FirstOrDefault();
                var currentLevel = Enum.TryParse<ApprovalLevel>(currentRole, out var level) ? level : ApprovalLevel.None;

                if (!CanApprove(advanceRequest))
                {
                    return Json(new { success = false, message = "Bu talebi reddetme yetkiniz bulunmamaktadır." });
                }

                advanceRequest.Status = RequestStatus.Rejected;
                advanceRequest.RejectedBy = user.Id.ToString();
                advanceRequest.RejectionReason = notes;
                advanceRequest.UpdatedAt = DateTime.UtcNow;
                advanceRequest.UpdatedBy = user.Id;

                var approval = new ApprovalProcess
                {
                    AdvanceRequestId = advanceRequest.Id,
                    ApproverUserId = user.Id,
                    Level = currentLevel,
                    Status = ApprovalStatus.Rejected,
                    Comments = notes,
                    CreatedAt = DateTime.UtcNow
                };

                await _context.ApprovalProcesses.AddAsync(approval);
                _context.AdvanceRequests.Update(advanceRequest);
                await _context.SaveChangesAsync();

                var rejectMessage = $"#{advanceRequest.RequestNumber} numaralı avans talebi {user.FullName} tarafından reddedildi. Neden: {notes}";

                await _notificationService.CreateAsync(new Notification
                {
                    Title = "Avans Talebi Reddedildi",
                    Message = rejectMessage,
                    UserId = advanceRequest.UserId,
                    Type = NotificationType.Rejection,
                    CreatedAt = DateTime.UtcNow
                });

                var adminUsers = await _userManager.GetUsersInRoleAsync("Admin");
                foreach (var adminUser in adminUsers)
                {
                    await _notificationService.CreateAsync(new Notification
                    {
                        Title = "Avans Talebi Reddedildi",
                        Message = rejectMessage,
                        UserId = adminUser.Id,
                        Type = NotificationType.Rejection,
                        CreatedAt = DateTime.UtcNow
                    });
                }

                return Json(new { 
                    success = true, 
                    message = "Avans talebi başarıyla reddedildi.",
                    requestNumber = advanceRequest.RequestNumber
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Avans talebi reddedilirken hata oluştu.");
                return Json(new { success = false, message = "İşlem sırasında bir hata oluştu." });
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            try
            {
                _logger.LogInformation("Create sayfası yükleniyor");
                var model = new AdvanceRequest
                {
                    DesiredDate = DateTime.Now.AddDays(1)
                };
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Create sayfası yüklenirken hata oluştu");
                TempData["ErrorMessage"] = "Sayfa yüklenirken bir hata oluştu. Lütfen daha sonra tekrar deneyin.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Amount,Description,DesiredDate,Notes")] AdvanceRequest request)
        {
            _logger.LogInformation("Create POST metodu başladı");
            
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState geçersiz: {@ModelState}", ModelState);
                return View(request);
            }

            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    _logger.LogWarning("Kullanıcı bulunamadı");
                    ModelState.AddModelError("", "Kullanıcı bulunamadı.");
                    return View(request);
                }

                _logger.LogInformation("Kullanıcı bulundu: {UserId}", user.Id);

                var limits = await _context.AdvanceLimits
                    .OrderByDescending(x => x.UpdatedAt)
                    .FirstOrDefaultAsync();

                if (limits == null)
                {
                    _logger.LogWarning("Avans limitleri bulunamadı");
                    ModelState.AddModelError("", "Avans limitleri tanımlanmamış. Lütfen sistem yöneticisi ile iletişime geçin.");
                    return View(request);
                }

                _logger.LogInformation("Avans limitleri bulundu: Min={MinAmount}, Max={MaxAmount}", limits.MinAmount, limits.MaxAmount);

                if (request.Amount < limits.MinAmount)
                {
                    _logger.LogWarning("Tutar minimum limit altında: {Amount} < {MinAmount}", request.Amount, limits.MinAmount);
                    ModelState.AddModelError("Amount", $"Avans tutarı minimum {limits.MinAmount:C2} olmalıdır.");
                    return View(request);
                }

                if (request.Amount > limits.MaxAmount)
                {
                    _logger.LogWarning("Tutar maksimum limit üstünde: {Amount} > {MaxAmount}", request.Amount, limits.MaxAmount);
                    ModelState.AddModelError("Amount", $"Avans tutarı maksimum {limits.MaxAmount:C2} olmalıdır.");
                    return View(request);
                }

                _logger.LogInformation("Tutar limitleri kontrol edildi");

                var newRequest = new AdvanceRequest
                {
                    UserId = user.Id,
                    Amount = request.Amount,
                    Description = request.Description,
                    DesiredDate = request.DesiredDate,
                    Notes = request.Notes,
                    RequestDate = DateTime.UtcNow,
                    Status = RequestStatus.Pending,
                    CreatedAt = DateTime.UtcNow,
                    RequestNumber = $"AT{DateTime.UtcNow:yyyyMMddHHmmss}",
                    ApprovedBy = "-",
                    RejectedBy = "-",
                    RejectionReason = "-"
                };

                var userRoles = await _userManager.GetRolesAsync(user);

                if (userRoles.Contains("OnMuhasebe"))
                {
                    newRequest.Status = RequestStatus.Approved;
                    newRequest.CurrentLevel = ApprovalLevel.None;
                    newRequest.ApprovedAt = DateTime.UtcNow;
                    newRequest.ApprovedBy = user.UserName;
                }
                else if (userRoles.Contains("FinansMuduru"))
                {
                    newRequest.CurrentLevel = ApprovalLevel.OnMuhasebe;
                }
                else if (userRoles.Contains("GenelMudur"))
                {
                    newRequest.CurrentLevel = ApprovalLevel.FinansMuduru;
                }
                else if (userRoles.Contains("GenelMudurYardimcisi"))
                {
                    newRequest.CurrentLevel = ApprovalLevel.GenelMudur;
                }
                else if (userRoles.Contains("Direktor"))
                {
                    newRequest.CurrentLevel = ApprovalLevel.GenelMudurYardimcisi;
                }
                else if (userRoles.Contains("BirimMuduru"))
                {
                    newRequest.CurrentLevel = ApprovalLevel.Direktor;
                }
                else
                {
                    newRequest.CurrentLevel = ApprovalLevel.BirimMuduru;
                }

                _logger.LogInformation("Yeni talep oluşturuluyor: {@Request}", newRequest);

                try
                {
                    _context.AdvanceRequests.Add(newRequest);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Talep başarıyla oluşturuldu: {RequestId}", newRequest.Id);

                    TempData["SuccessMessage"] = "Avans talebi başarıyla oluşturuldu.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, "Veritabanı güncelleme hatası: {Message}", ex.Message);
                    if (ex.InnerException != null)
                    {
                        _logger.LogError(ex.InnerException, "İç hata: {Message}", ex.InnerException.Message);
                    }
                    ModelState.AddModelError("", "Veritabanı güncelleme hatası oluştu. Lütfen daha sonra tekrar deneyin.");
                    return View(request);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Avans talebi oluşturulurken hata oluştu");
                ModelState.AddModelError("", "Avans talebi oluşturulurken bir hata oluştu. Lütfen daha sonra tekrar deneyin.");
                return View(request);
            }
        }

        [HttpGet]
        public async Task<IActionResult> CheckLimits()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Json(new { success = false, message = "Kullanıcı bilgisi bulunamadı." });
                }

                var userRoles = await _userManager.GetRolesAsync(user);
                var currentRole = userRoles.FirstOrDefault();

                
                var currentLevel = currentRole switch
                {
                    "BirimMuduru" => ApprovalLevel.BirimMuduru,
                    "Direktor" => ApprovalLevel.Direktor,
                    "GenelMudurYardimcisi" => ApprovalLevel.GenelMudurYardimcisi,
                    "GenelMudur" => ApprovalLevel.GenelMudur,
                    "FinansMuduru" => ApprovalLevel.FinansMuduru,
                    "OnMuhasebe" => ApprovalLevel.OnMuhasebe,
                    _ => ApprovalLevel.None
                };

                
                var allLevels = new[]
                {
                    ApprovalLevel.BirimMuduru,
                    ApprovalLevel.Direktor,
                    ApprovalLevel.GenelMudurYardimcisi,
                    ApprovalLevel.GenelMudur,
                    ApprovalLevel.FinansMuduru,
                    ApprovalLevel.OnMuhasebe
                };

                var limitCheckResults = new List<LimitCheckResult>();

                foreach (var level in allLevels)
                {
                    
                    var departmentLimits = await _advanceLimitService.GetByDepartmentAndLevelAsync(user.DepartmentId, level);
                    if (departmentLimits != null)
                    {
                        limitCheckResults.Add(new LimitCheckResult
                        {
                            Level = level,
                            IsDepartmentLimit = true,
                            IsExceeded = false,
                            Message = $"{level} seviyesi için departman limitleri: Min: {departmentLimits.MinAmount:C2}, Max: {departmentLimits.MaxAmount:C2}"
                        });
                    }

                    var generalLimits = await _advanceLimitService.GetByDepartmentAndLevelAsync(null, level);
                    if (generalLimits != null)
                    {
                        limitCheckResults.Add(new LimitCheckResult
                        {
                            Level = level,
                            IsDepartmentLimit = false,
                            IsExceeded = false,
                            Message = $"{level} seviyesi için genel limitler: Min: {generalLimits.MinAmount:C2}, Max: {generalLimits.MaxAmount:C2}"
                        });
                    }
                }

                return Json(new { 
                    success = true, 
                    message = "Limit bilgileri başarıyla getirildi.",
                    limitChecks = limitCheckResults.Select(x => new {
                        level = x.Level,
                        isDepartmentLimit = x.IsDepartmentLimit,
                        isExceeded = x.IsExceeded,
                        message = x.Message
                    }).ToList()
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Limit bilgileri getirilirken bir hata oluştu: " + ex.Message });
            }
        }
    }
} 