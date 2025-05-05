using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VarlikYönetimi.Core.Entities;
using VarlikYönetimi.Services.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using VarlikYönetimi.Core.Enums;
using VarlikYönetimi.Data.Context;
using VarlikYönetimi.Core.ViewModels;
using Microsoft.Extensions.Logging;

namespace VarlikYönetimi.MVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AdvanceLimitsController : Controller
    {
        private readonly IAdvanceLimitService _advanceLimitService;
        private readonly UserManager<User> _userManager;
        private readonly AppDbContext _context;
        private readonly INotificationService _notificationService;

        public AdvanceLimitsController(
            IAdvanceLimitService advanceLimitService,
            UserManager<User> userManager,
            AppDbContext context,
            INotificationService notificationService)
        {
            _advanceLimitService = advanceLimitService;
            _userManager = userManager;
            _context = context;
            _notificationService = notificationService;
        }

        public async Task<IActionResult> Index()
        {
            var departments = await _context.Departments.ToListAsync();
            var approvalLevels = Enum.GetValues(typeof(ApprovalLevel))
                .Cast<ApprovalLevel>()
                .Where(x => x != ApprovalLevel.None && x != ApprovalLevel.Personel)
                .ToList();

            var viewModel = new DepartmentLimitsViewModel
            {
                Departments = departments,
                ApprovalLevels = approvalLevels,
                DepartmentLimits = (await _advanceLimitService.GetAllAsync()).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateLimit([FromBody] LimitUpdateModel model)
        {
            try
            {
                if (model.MinAmount <= 0 || model.MaxAmount <= 0)
                {
                    return Json(new { success = false, message = "Minimum ve maksimum tutar 0'dan büyük olmalıdır." });
                }

                if (model.MinAmount >= model.MaxAmount)
                {
                    return Json(new { success = false, message = "Minimum tutar maksimum tutardan küçük olmalıdır." });
                }

                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Json(new { success = false, message = "Kullanıcı bilgisi bulunamadı." });
                }

                var options = new DbContextOptionsBuilder<AppDbContext>()
                    .UseSqlServer(_context.Database.GetDbConnection().ConnectionString)
                    .Options;

                using (var newContext = new AppDbContext(options))
                {
                    
                    if (!model.DepartmentId.HasValue && model.ApprovalLevel == ApprovalLevel.None)
                    {
                        
                        var allDepartmentLimits = await newContext.AdvanceLimits
                            .Where(x => x.IsActive && x.DepartmentId != null)
                            .ToListAsync();

                        foreach (var limit in allDepartmentLimits)
                        {
                            limit.IsActive = false;
                            newContext.AdvanceLimits.Update(limit);

                            var departmentNewLimit = new AdvanceLimit
                            {
                                DepartmentId = limit.DepartmentId,
                                ApprovalLevel = limit.ApprovalLevel,
                                MinAmount = model.MinAmount,
                                MaxAmount = model.MaxAmount,
                                CreatedBy = user.UserName,
                                CreatedAt = DateTime.UtcNow,
                                UpdatedBy = user.UserName,
                                UpdatedAt = DateTime.UtcNow,
                                IsActive = true
                            };
                            await newContext.AdvanceLimits.AddAsync(departmentNewLimit);
                        }

                       
                        var existingGeneralLimit = await newContext.AdvanceLimits
                            .FirstOrDefaultAsync(x => x.DepartmentId == null && x.ApprovalLevel == ApprovalLevel.None && x.IsActive);

                        if (existingGeneralLimit != null)
                        {
                            existingGeneralLimit.IsActive = false;
                            newContext.AdvanceLimits.Update(existingGeneralLimit);
                        }

                        var newGeneralLimit = new AdvanceLimit
                        {
                            DepartmentId = null,
                            ApprovalLevel = ApprovalLevel.None,
                            MinAmount = model.MinAmount,
                            MaxAmount = model.MaxAmount,
                            CreatedBy = user.UserName,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedBy = user.UserName,
                            UpdatedAt = DateTime.UtcNow,
                            IsActive = true
                        };

                        await newContext.AdvanceLimits.AddAsync(newGeneralLimit);
                    }
                    else
                    {
                        
                        var existingLimits = await newContext.AdvanceLimits
                            .FirstOrDefaultAsync(x => x.DepartmentId == model.DepartmentId && 
                                                    x.ApprovalLevel == model.ApprovalLevel && 
                                                    x.IsActive);

                        if (existingLimits != null)
                {
                            existingLimits.IsActive = false;
                            newContext.AdvanceLimits.Update(existingLimits);
                }

                        var newLimit = new AdvanceLimit
                {
                    DepartmentId = model.DepartmentId,
                    ApprovalLevel = model.ApprovalLevel,
                    MinAmount = model.MinAmount,
                    MaxAmount = model.MaxAmount,
                    CreatedBy = user.UserName,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedBy = user.UserName,
                            UpdatedAt = DateTime.UtcNow,
                            IsActive = true
                };

                        await newContext.AdvanceLimits.AddAsync(newLimit);

                        
                        var generalLimit = await newContext.AdvanceLimits
                            .FirstOrDefaultAsync(x => x.DepartmentId == null && 
                                                    x.ApprovalLevel == ApprovalLevel.None && 
                                                    x.IsActive);

                        if (generalLimit != null)
                        {
                            generalLimit.IsActive = false;
                            newContext.AdvanceLimits.Update(generalLimit);

                            var updatedGeneralLimit = new AdvanceLimit
                            {
                                DepartmentId = null,
                                ApprovalLevel = ApprovalLevel.None,
                                MinAmount = model.MinAmount,
                                MaxAmount = model.MaxAmount,
                                CreatedBy = user.UserName,
                                CreatedAt = DateTime.UtcNow,
                                UpdatedBy = user.UserName,
                                UpdatedAt = DateTime.UtcNow,
                                IsActive = true
                            };

                            await newContext.AdvanceLimits.AddAsync(updatedGeneralLimit);
                        }
                        else
                        {
                            
                            var newGeneralLimit = new AdvanceLimit
                            {
                                DepartmentId = null,
                                ApprovalLevel = ApprovalLevel.None,
                                MinAmount = model.MinAmount,
                                MaxAmount = model.MaxAmount,
                                CreatedBy = user.UserName,
                                CreatedAt = DateTime.UtcNow,
                                UpdatedBy = user.UserName,
                                UpdatedAt = DateTime.UtcNow,
                                IsActive = true
                            };

                            await newContext.AdvanceLimits.AddAsync(newGeneralLimit);
                        }

                        
                        var otherDepartmentLimits = await newContext.AdvanceLimits
                            .Where(x => x.IsActive && 
                                      x.DepartmentId != null && 
                                      x.DepartmentId != model.DepartmentId)
                            .ToListAsync();

                        foreach (var limit in otherDepartmentLimits)
                        {
                            limit.IsActive = false;
                            newContext.AdvanceLimits.Update(limit);

                            var departmentNewLimit = new AdvanceLimit
                            {
                                DepartmentId = limit.DepartmentId,
                                ApprovalLevel = limit.ApprovalLevel,
                                MinAmount = model.MinAmount,
                                MaxAmount = model.MaxAmount,
                                CreatedBy = user.UserName,
                                CreatedAt = DateTime.UtcNow,
                                UpdatedBy = user.UserName,
                                UpdatedAt = DateTime.UtcNow,
                                IsActive = true
                            };
                            await newContext.AdvanceLimits.AddAsync(departmentNewLimit);
                        }
                    }

                    await newContext.SaveChangesAsync();
                }

                
                string departmentName = "Genel";
                if (model.DepartmentId.HasValue)
                {
                    using (var newContext = new AppDbContext(options))
                    {
                        var department = await newContext.Departments.FindAsync(model.DepartmentId.Value);
                        departmentName = department?.Name ?? "Bilinmeyen Departman";
                    }
                }

                await _notificationService.CreateAsync(new Notification
                {
                    Title = "Avans Limitleri Güncellendi",
                    Message = $"{departmentName} için {model.ApprovalLevel} seviyesinde avans limitleri güncellendi. Yeni limitler: Min: {model.MinAmount:C2}, Max: {model.MaxAmount:C2}",
                    CreatedAt = DateTime.UtcNow,
                    UserId = user.Id
                });

                return Json(new { success = true, message = "Avans limitleri başarıyla güncellendi." });
            }
            catch (Exception ex)
            {
                var errorMessage = $"Hata Detayı: {ex.Message}";
                if (ex.InnerException != null)
                {
                    errorMessage += $"\nİç Hata: {ex.InnerException.Message}";
                }
                return Json(new { success = false, message = $"Avans limitleri güncellenirken bir hata oluştu: {errorMessage}" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetDepartmentLimits(int departmentId)
        {
            var limits = await _advanceLimitService.GetByDepartmentAsync(departmentId);
            return Json(new { success = true, data = limits });
        }

        [HttpGet]
        public async Task<IActionResult> GetGeneralLimits()
        {
            var generalLimit = await _advanceLimitService.GetByDepartmentAndLevelAsync(null, ApprovalLevel.None);
            if (generalLimit == null)
            {
                return Json(new { success = false, message = "Genel limit bulunamadı." });
            }

            return Json(new { success = true, data = new { generalLimit.MinAmount, generalLimit.MaxAmount } });
        }
    }

    public class LimitUpdateModel
    {
        public int? DepartmentId { get; set; }
        public ApprovalLevel ApprovalLevel { get; set; }
        public decimal MinAmount { get; set; }
        public decimal MaxAmount { get; set; }
    }
} 