using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VarlikYönetimi.Core.Entities;
using VarlikYönetimi.Core.Enums;
using VarlikYönetimi.Data.Context;
using VarlikYönetimi.Services.Services.Interfaces;

namespace VarlikYönetimi.MVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AdvanceRequestsController : Controller
    {
        private readonly ILogger<AdvanceRequestsController> _logger;
        private readonly UserManager<User> _userManager;
        private readonly AppDbContext _context;
        private readonly INotificationService _notificationService;

        public AdvanceRequestsController(
            AppDbContext context, 
            ILogger<AdvanceRequestsController> logger, 
            UserManager<User> userManager,
            INotificationService notificationService)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
            _notificationService = notificationService;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    _logger.LogWarning("Kullanıcı bulunamadı");
                    return NotFound();
                }

                var userRoles = await _userManager.GetRolesAsync(user);
                _logger.LogInformation($"Kullanıcı rolleri: {string.Join(", ", userRoles)}");
                
                var requests = new List<AdvanceRequest>();

                if (userRoles.Contains("Admin"))
                {
                    
                    requests = await _context.AdvanceRequests
                        .Include(x => x.User)
                        .OrderByDescending(x => x.RequestDate)
                        .ToListAsync();
                    
                    _logger.LogInformation($"Admin için toplam {requests.Count} talep bulundu");
                }
                else if (userRoles.Contains("BirimMuduru"))
                {
                    requests = await _context.AdvanceRequests
                        .Include(x => x.User)
                        .Where(x => x.Status == RequestStatus.Pending && 
                                   x.CurrentLevel == ApprovalLevel.BirimMuduru)
                        .OrderByDescending(x => x.RequestDate)
                        .ToListAsync();
                    
                    _logger.LogInformation($"BirimMuduru için {requests.Count} talep bulundu");
                }
                else if (userRoles.Contains("Direktor"))
                {
                    requests = await _context.AdvanceRequests
                        .Include(x => x.User)
                        .Where(x => x.Status == RequestStatus.Pending && x.CurrentLevel == ApprovalLevel.Direktor)
                        .OrderByDescending(x => x.RequestDate)
                        .ToListAsync();
                }
                else if (userRoles.Contains("GenelMudurYardimcisi"))
                {
                    requests = await _context.AdvanceRequests
                        .Include(x => x.User)
                        .Where(x => x.Status == RequestStatus.Pending && x.CurrentLevel == ApprovalLevel.GenelMudurYardimcisi)
                        .OrderByDescending(x => x.RequestDate)
                        .ToListAsync();
                }
                else if (userRoles.Contains("GenelMudur"))
                {
                    requests = await _context.AdvanceRequests
                        .Include(x => x.User)
                        .Where(x => x.Status == RequestStatus.Pending && x.CurrentLevel == ApprovalLevel.GenelMudur)
                        .OrderByDescending(x => x.RequestDate)
                        .ToListAsync();
                }
                else if (userRoles.Contains("FinansMuduru"))
                {
                    requests = await _context.AdvanceRequests
                        .Include(x => x.User)
                        .Where(x => x.Status == RequestStatus.Pending && x.CurrentLevel == ApprovalLevel.FinansMuduru)
                        .OrderByDescending(x => x.RequestDate)
                        .ToListAsync();
                }
                else if (userRoles.Contains("OnMuhasebe"))
                {
                    requests = await _context.AdvanceRequests
                        .Include(x => x.User)
                        .Where(x => x.Status == RequestStatus.Pending && x.CurrentLevel == ApprovalLevel.OnMuhasebe)
                        .OrderByDescending(x => x.RequestDate)
                        .ToListAsync();
                }

                
                var groupedRequests = requests
                    .GroupBy(x => x.RequestDate.Date)
                    .OrderByDescending(g => g.Key)
                    .ToDictionary(g => g.Key, g => g.ToList());

                _logger.LogInformation($"Toplam {groupedRequests.Count} tarih grubu oluşturuldu");
                foreach (var group in groupedRequests)
                {
                    _logger.LogInformation($"Tarih: {group.Key}, Talep Sayısı: {group.Value.Count}");
                    foreach (var request in group.Value)
                    {
                        _logger.LogInformation($"Talep ID: {request.Id}, Kullanıcı: {request.User?.FullName}, Tutar: {request.Amount}");
                    }
                }

                return View(groupedRequests);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Avans talepleri listelenirken bir hata oluştu");
                TempData["ErrorMessage"] = "Avans talepleri listelenirken bir hata oluştu. Lütfen daha sonra tekrar deneyiniz.";
                return View(new Dictionary<DateTime, List<AdvanceRequest>>());
            }
        }

        [HttpGet]
        [Route("Admin/AdvanceRequests/Details/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int id)
        {
            var advanceRequest = await _context.AdvanceRequests
                .Include(a => a.User)
                .Include(a => a.Documents)
                .Include(a => a.Approvals)
                    .ThenInclude(a => a.ApproverUser)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (advanceRequest == null)
            {
                return NotFound();
            }

            return View(advanceRequest);
        }

        [HttpGet]
        [Route("Admin/AdvanceRequests/Edit/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var advanceRequest = await _context.AdvanceRequests
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (advanceRequest == null)
            {
                return NotFound();
            }

            return View(advanceRequest);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AdvanceRequest advanceRequest)
        {
            if (id != advanceRequest.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingRequest = await _context.AdvanceRequests
                        .Include(x => x.User)
                        .FirstOrDefaultAsync(x => x.Id == id);
                        
                    if (existingRequest == null)
                    {
                        return NotFound();
                    }

                    
                    bool hasChanges = existingRequest.Amount != advanceRequest.Amount ||
                                    existingRequest.Description != advanceRequest.Description ||
                                    existingRequest.DesiredDate != advanceRequest.DesiredDate ||
                                    existingRequest.Notes != advanceRequest.Notes;

                    
                    existingRequest.Amount = advanceRequest.Amount;
                    existingRequest.Description = advanceRequest.Description;
                    existingRequest.DesiredDate = advanceRequest.DesiredDate;
                    existingRequest.Notes = advanceRequest.Notes;

                    _context.Update(existingRequest);
                    await _context.SaveChangesAsync();

                    
                    if (hasChanges)
                    {
                        var notification = new Notification
                        {
                            UserId = existingRequest.UserId,
                            Title = "Avans Talebiniz Güncellendi",
                            Message = $"Avans talebiniz admin tarafından güncellendi. Yeni tutar: {advanceRequest.Amount:C2}",
                            Type = NotificationType.AdvanceRequestUpdated,
                            IsRead = false,
                            CreatedAt = DateTime.UtcNow,
                            AdvanceRequestId = existingRequest.Id
                        };

                        await _context.Notifications.AddAsync(notification);
                        await _context.SaveChangesAsync();
                    }

                    TempData["SuccessMessage"] = "Avans talebi başarıyla güncellendi.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _context.AdvanceRequests.AnyAsync(e => e.Id == id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(advanceRequest);
        }

        private ApprovalLevel GetNextApprovalLevel(ApprovalLevel currentLevel)
        {
            return currentLevel switch
            {
                ApprovalLevel.BirimMuduru => ApprovalLevel.Direktor,
                ApprovalLevel.Direktor => ApprovalLevel.GenelMudurYardimcisi,
                ApprovalLevel.GenelMudurYardimcisi => ApprovalLevel.GenelMudur,
                ApprovalLevel.GenelMudur => ApprovalLevel.FinansMuduru,
                ApprovalLevel.FinansMuduru => ApprovalLevel.OnMuhasebe,
                ApprovalLevel.OnMuhasebe => ApprovalLevel.None,
                _ => currentLevel
            };
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id, string notes)
        {
            try
            {
                var advanceRequest = await _context.AdvanceRequests
                    .Include(x => x.User)
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (advanceRequest == null)
                {
                    return NotFound("Avans talebi bulunamadı.");
                }

                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound("Kullanıcı bulunamadı.");
                }

                
                advanceRequest.Status = RequestStatus.Approved;
                advanceRequest.ApprovedBy = user.UserName;
                advanceRequest.ApprovedAt = DateTime.UtcNow;
                advanceRequest.UpdatedAt = DateTime.UtcNow;
                advanceRequest.UpdatedBy = Convert.ToInt32(user.Id);
                advanceRequest.Notes = notes;
                advanceRequest.CurrentLevel = ApprovalLevel.None; 

                _context.AdvanceRequests.Update(advanceRequest);

                
                var approval = new ApprovalProcess
                {
                    AdvanceRequestId = id,
                    ApproverUserId = user.Id,
                    Level = ApprovalLevel.Admin,
                    Status = ApprovalStatus.Approved,
                    Comments = notes,
                    CreatedAt = DateTime.UtcNow,
                    ApprovedAmount = advanceRequest.Amount 
                };
                await _context.ApprovalProcesses.AddAsync(approval);

               
                var notification = new Notification
                {
                    UserId = advanceRequest.UserId,
                    Title = "Avans Talebiniz Onaylandı",
                    Message = $"Avans talebiniz {user.FullName} tarafından onaylandı. {(string.IsNullOrEmpty(notes) ? "" : $"\nNot: {notes}")}",
                    Type = NotificationType.ApprovalCompleted,
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow,
                    AdvanceRequestId = advanceRequest.Id
                };
                await _context.Notifications.AddAsync(notification);

                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Avans talebi onaylanırken hata oluştu");
                return StatusCode(500, "İşlem sırasında bir hata oluştu.");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id, string notes)
        {
            try
            {
                var advanceRequest = await _context.AdvanceRequests
                    .Include(x => x.User)
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (advanceRequest == null)
                {
                    return NotFound("Avans talebi bulunamadı.");
                }

                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound("Kullanıcı bulunamadı.");
                }

                
                advanceRequest.Status = RequestStatus.Rejected;
                advanceRequest.RejectedAt = DateTime.UtcNow;
                advanceRequest.RejectedBy = user.UserName;
                advanceRequest.RejectionReason = notes;
                advanceRequest.UpdatedAt = DateTime.UtcNow;
                advanceRequest.UpdatedBy = Convert.ToInt32(user.Id);
                advanceRequest.CurrentLevel = ApprovalLevel.None; 

                _context.AdvanceRequests.Update(advanceRequest);

                
                var approval = new ApprovalProcess
                {
                    AdvanceRequestId = id,
                    ApproverUserId = user.Id,
                    Level = ApprovalLevel.Admin,
                    Status = ApprovalStatus.Rejected,
                    Comments = notes,
                    CreatedAt = DateTime.UtcNow
                };
                await _context.ApprovalProcesses.AddAsync(approval);

                
                var notification = new Notification
                {
                    UserId = advanceRequest.UserId,
                    Title = "Avans Talebiniz Reddedildi",
                    Message = $"Avans talebiniz {user.FullName} tarafından reddedildi. {(string.IsNullOrEmpty(notes) ? "" : $"\nNedeni: {notes}")}",
                    Type = NotificationType.Rejection,
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow,
                    AdvanceRequestId = advanceRequest.Id
                };
                await _context.Notifications.AddAsync(notification);

                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Avans talebi reddedilirken hata oluştu");
                return StatusCode(500, "İşlem sırasında bir hata oluştu.");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var request = await _context.AdvanceRequests.FindAsync(id);
            if (request == null)
            {
                return NotFound();
            }

            _context.AdvanceRequests.Remove(request);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
} 