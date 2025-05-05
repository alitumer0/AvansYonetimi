using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VarlikYönetimi.Core.Entities;
using VarlikYönetimi.Core.Enums;
using VarlikYönetimi.Data.Repositories.Interfaces;
using VarlikYönetimi.Services.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using VarlikYönetimi.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using VarlikYönetimi.Data.Repositories;
using VarlikYönetimi.Services.Services.Interfaces;

namespace VarlikYönetimi.Services.Services
{
    public class AdvanceRequestService : GenericService<AdvanceRequest>, IAdvanceRequestService
    {
        private readonly IAdvanceRequestRepository _advanceRequestRepository;
        private readonly IUserService _userService;
        private readonly INotificationService _notificationService;
        private readonly IAdvanceLimitService _advanceLimitService;
        private readonly ILegalActionService _legalActionService;
        private readonly UserManager<User> _userManager;
        private readonly Dictionary<RequestStatus, TimeSpan> _statusTimeouts = new();
        private readonly Dictionary<RequestStatus, RequestStatus> _timeoutNextStatus = new();

        public AdvanceRequestService(
            IGenericRepository<AdvanceRequest> repository,
            IAdvanceRequestRepository advanceRequestRepository,
            IUserService userService,
            INotificationService notificationService,
            IAdvanceLimitService advanceLimitService,
            ILegalActionService legalActionService,
            UserManager<User> userManager) : base(repository)
        {
            _advanceRequestRepository = advanceRequestRepository;
            _userService = userService;
            _notificationService = notificationService;
            _advanceLimitService = advanceLimitService;
            _legalActionService = legalActionService;
            _userManager = userManager;

            _statusTimeouts.Add(RequestStatus.BMOnayBekliyor, TimeSpan.FromHours(8));
            _statusTimeouts.Add(RequestStatus.DirektorOnayBekliyor, TimeSpan.FromHours(8));
            _statusTimeouts.Add(RequestStatus.GMYOnayBekliyor, TimeSpan.FromHours(8));
            _statusTimeouts.Add(RequestStatus.GMOnayBekliyor, TimeSpan.FromHours(8));
            _statusTimeouts.Add(RequestStatus.FMOdemeTarihiBelirleme, TimeSpan.FromHours(8));
            _statusTimeouts.Add(RequestStatus.OdemeHazir, TimeSpan.FromHours(8));
            _statusTimeouts.Add(RequestStatus.AvansGeriOdenmeyiBekliyor, TimeSpan.FromHours(720));
            _statusTimeouts.Add(RequestStatus.HukukiIslemBaslatildi, TimeSpan.FromHours(360));

            _timeoutNextStatus.Add(RequestStatus.BMOnayBekliyor, RequestStatus.DirektorOnayBekliyor);
            _timeoutNextStatus.Add(RequestStatus.DirektorOnayBekliyor, RequestStatus.GMYOnayBekliyor);
            _timeoutNextStatus.Add(RequestStatus.GMYOnayBekliyor, RequestStatus.GMOnayBekliyor);
            _timeoutNextStatus.Add(RequestStatus.GMOnayBekliyor, RequestStatus.FMOdemeTarihiBelirleme);
            _timeoutNextStatus.Add(RequestStatus.FMOdemeTarihiBelirleme, RequestStatus.OdemeHazir);
            _timeoutNextStatus.Add(RequestStatus.AvansGeriOdenmeyiBekliyor, RequestStatus.HukukiIslemBaslatildi);
            _timeoutNextStatus.Add(RequestStatus.HukukiIslemBaslatildi, RequestStatus.AvansTalebiSonlandirildi);
        }

        public async Task<AdvanceRequest> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<AdvanceRequest>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<IEnumerable<AdvanceRequest>> GetByUserIdAsync(int userId)
        {
            return await _repository.FindAsync(a => a.UserId == userId);
        }

        public Task<IEnumerable<AdvanceRequest>> GetUserAdvanceRequestsAsync(int userId)
        { throw new NotImplementedException(); }

        public async Task<IEnumerable<AdvanceRequest>> GetPendingApprovalsAsync(int userId)
        {
            return await _advanceRequestRepository.GetPendingApprovalsAsync(userId);
        }

        public Task<IEnumerable<AdvanceRequest>> GetByStatusAsync(string status)
        { throw new NotImplementedException(); }

        public async Task<bool> CreateAsync(AdvanceRequest request)
        {
            var limits = await _advanceLimitService.GetAllAsync();
            var currentLimit = limits.OrderByDescending(x => x.UpdatedAt).FirstOrDefault();

            if (currentLimit != null)
            {
                if (request.Amount < currentLimit.MinAmount)
                {
                    throw new ValidationException($"Avans tutarı minimum {currentLimit.MinAmount:C2} olmalıdır.");
                }

                if (request.Amount > currentLimit.MaxAmount)
                {
                    throw new ValidationException($"Avans tutarı maksimum {currentLimit.MaxAmount:C2} olmalıdır.");
                }
            }

            request.CreatedAt = DateTime.UtcNow;
            request.Status = Core.Enums.RequestStatus.Pending;
            request.CurrentLevel = Core.Enums.ApprovalLevel.Personel;
            await _repository.AddAsync(request);
            return true;
        }

        public async Task<bool> UpdateAsync(AdvanceRequest request)
        {
            var limits = await _advanceLimitService.GetAllAsync();
            var currentLimit = limits.OrderByDescending(x => x.UpdatedAt).FirstOrDefault();

            if (currentLimit != null)
            {
                if (request.Amount < currentLimit.MinAmount)
                {
                    throw new Exception($"Avans tutarı minimum {currentLimit.MinAmount:C2} olmalıdır.");
                }

                if (request.Amount > currentLimit.MaxAmount)
                {
                    throw new Exception($"Avans tutarı maksimum {currentLimit.MaxAmount:C2} olmalıdır.");
                }
            }

            request.UpdatedAt = DateTime.UtcNow;
            await _repository.UpdateAsync(request);
            return true;
        }

        public Task<bool> DeleteAsync(int id)
        { throw new NotImplementedException(); }

        public async Task<bool> ApproveRequestAsync(int id, int userId, decimal? approvedAmount, string notes)
        {
            var request = await _repository.GetByIdAsync(id);
            if (request == null)
                return false;

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user != null && user.DepartmentId.HasValue)
            {
                ApprovalLevel approvalLevel = ApprovalLevel.Personel;
                if (await _userManager.IsInRoleAsync(user, "BirimMuduru"))
                    approvalLevel = ApprovalLevel.BirimMuduru;
                else if (await _userManager.IsInRoleAsync(user, "Direktor"))
                    approvalLevel = ApprovalLevel.Direktor;
                else if (await _userManager.IsInRoleAsync(user, "GenelMudurYardimcisi"))
                    approvalLevel = ApprovalLevel.GenelMudurYardimcisi;
                else if (await _userManager.IsInRoleAsync(user, "GenelMudur"))
                    approvalLevel = ApprovalLevel.GenelMudur;
                else if (await _userManager.IsInRoleAsync(user, "FinansMuduru"))
                    approvalLevel = ApprovalLevel.FinansMuduru;

                var limit = await _advanceLimitService.GetByDepartmentAndLevelAsync(user.DepartmentId.Value, approvalLevel);
                if (limit != null)
                {
                    var amount = approvedAmount ?? request.Amount;
                    if (amount < limit.MinAmount || amount > limit.MaxAmount)
                    {
                        throw new ValidationException($"Onaylanan tutar {limit.MinAmount:C2} ile {limit.MaxAmount:C2} arasında olmalıdır.");
                    }
                }
            }

            request.Status = Core.Enums.RequestStatus.Approved;
            request.ApprovedAt = DateTime.UtcNow;
            request.ApprovedBy = userId.ToString();
            request.ApprovedAmount = approvedAmount ?? request.Amount;
            request.Notes = notes;

            await _repository.UpdateAsync(request);
            return true;
        }

        public async Task<bool> RejectRequestAsync(int id, int userId, string notes)
        {
            var request = await _repository.GetByIdAsync(id);
            if (request == null)
                return false;

            request.Status = Core.Enums.RequestStatus.Rejected;
            request.RejectedAt = DateTime.UtcNow;
            request.RejectedBy = userId.ToString();
            request.RejectionReason = notes;

            await _repository.UpdateAsync(request);
            return true;
        }

        public async Task<IEnumerable<AdvanceRequest>> GetByStatusAsync(Core.Enums.RequestStatus status)
        {
            return await _repository.FindAsync(a => a.Status == status);
        }

        public async Task CheckTimeoutsAsync()
        {
            var pendingRequests = await _repository.FindAsync(r => r.Status != RequestStatus.AvansTalebiSonlandirildi);

            foreach (var request in pendingRequests)
            {
                if (_statusTimeouts.TryGetValue(request.Status, out var timeout))
                {
                    var timeSinceLastUpdate = DateTime.UtcNow - request.UpdatedAt;
                    
                    if (timeSinceLastUpdate >= timeout)
                    {
                        await HandleTimeoutAsync(request);
                    }
                }
            }
        }

        private async Task HandleTimeoutAsync(AdvanceRequest request)
        {
            var oldStatus = request.Status;
            
            if (_timeoutNextStatus.TryGetValue(oldStatus, out var newStatus))
            {
                request.Status = newStatus;
                request.UpdatedAt = DateTime.UtcNow;
                
                await _repository.UpdateAsync(request);
                
                var notificationMessage = GetTimeoutNotificationMessage(request.Id, oldStatus, newStatus);
                await _notificationService.SendNotificationAsync(request.UserId, notificationMessage);
                
                if (newStatus == RequestStatus.HukukiIslemBaslatildi)
                {
                    await _legalActionService.CreateLegalActionAsync(new LegalAction
                    {
                        AdvanceRequestId = request.Id,
                        Status = LegalActionStatus.Active,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }
        }

        private string GetTimeoutNotificationMessage(int requestId, RequestStatus oldStatus, RequestStatus newStatus)
        {
            return oldStatus switch
            {
                RequestStatus.BMOnayBekliyor => $"Talep #{requestId}: Birim Müdürü onayı için 8 saatlik süre doldu, Direktör onayına ilerlendi.",
                RequestStatus.DirektorOnayBekliyor => $"Talep #{requestId}: Direktör onayı için 8 saatlik süre doldu, GMY onayına ilerlendi.",
                RequestStatus.GMYOnayBekliyor => $"Talep #{requestId}: GMY onayı için 8 saatlik süre doldu, GM onayına ilerlendi.",
                RequestStatus.GMOnayBekliyor => $"Talep #{requestId}: GM onayı için 8 saatlik süre doldu, FM ödeme tarihi belirlemeye ilerlendi.",
                RequestStatus.FMOdemeTarihiBelirleme => $"Talep #{requestId}: FM ödeme tarihi belirleme için 8 saatlik süre doldu, ödeme hazır aşamasına ilerlendi.",
                RequestStatus.OdemeHazir => $"Talep #{requestId}: Ödeme işlemi için 8 saatlik süre doldu, lütfen ödemeyi tamamlayın.",
                RequestStatus.AvansGeriOdenmeyiBekliyor => $"Talep #{requestId}: Geri ödeme için 30 günlük süre doldu, hukuki işlem başlatıldı.",
                RequestStatus.HukukiIslemBaslatildi => $"Talep #{requestId}: Hukuki işlem için 15 günlük süre doldu, talep sonlandırıldı.",
                _ => string.Empty
            };
        }

        public async Task<List<AdvanceRequest>> GetPendingAdvanceRequestsForGM()
        {
            var requests = await _repository.FindAsync(a => a.Status == RequestStatus.GMOnayBekliyor);
            return requests.ToList();
        }

        public async Task<int> GetTotalAdvanceRequestsCount()
        {
            return await _repository.CountAsync();
        }

        public async Task<int> GetTotalApprovedAdvanceRequestsCount()
        {
            return await _repository.CountAsync(a => a.Status == RequestStatus.AvansTalebiSonlandirildi);
        }

        public async Task<List<AdvanceRequest>> GetAllAdvanceRequestsForGM()
        {
            var requests = await _repository.GetAllAsync();
            return requests.ToList();
        }

        public async Task<bool> ApproveAdvanceRequestByGM(int id)
        {
            var request = await _repository.GetByIdAsync(id);
            if (request == null || request.Status != RequestStatus.GMOnayBekliyor)
            {
                return false;
            }

            request.Status = RequestStatus.FMOdemeTarihiBelirleme;
            request.UpdatedAt = DateTime.Now;
            await _repository.UpdateAsync(request);

            
            await _notificationService.CreateAsync(new Notification
            {
                UserId = request.UserId,
                Title = "Avans Talebi Onaylandı",
                Message = $"Avans talebiniz (ID: {request.Id}) Genel Müdür tarafından onaylandı.",
                CreatedAt = DateTime.Now
            });

            return true;
        }

        public async Task<bool> RejectAdvanceRequestByGM(int id, string rejectionReason)
        {
            var request = await _repository.GetByIdAsync(id);
            if (request == null || request.Status != RequestStatus.GMOnayBekliyor)
            {
                return false;
            }

            request.Status = RequestStatus.Rejected;
            request.RejectionReason = rejectionReason;
            request.UpdatedAt = DateTime.Now;
            await _repository.UpdateAsync(request);

            await _notificationService.CreateAsync(new Notification
            {
                UserId = request.UserId,
                Title = "Avans Talebi Reddedildi",
                Message = $"Avans talebiniz (ID: {request.Id}) Genel Müdür tarafından reddedildi. Sebep: {rejectionReason}",
                CreatedAt = DateTime.Now
            });

            return true;
        }
    }
} 