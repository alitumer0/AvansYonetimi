using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VarlikYönetimi.Core.Entities;
using VarlikYönetimi.Core.Enums;
using VarlikYönetimi.Services.Services.Interfaces;
using VarlikYönetimi.Data.Context;
using VarlikYönetimi.Data.Repositories.Interfaces;
using VarlikYönetimi.Services.Interfaces;

namespace VarlikYönetimi.Services.Services
{
    public class ApprovalProcessService : IApprovalProcessService
    {
        private readonly IGenericRepository<ApprovalProcess> _repository;
        private readonly IApprovalProcessRepository _approvalProcessRepository;
        private readonly IApprovalSettingsRepository _approvalSettingsRepository;
        private readonly AppDbContext _context;
        private readonly INotificationService _notificationService;
        private readonly IAdvanceRequestRepository _advanceRequestRepository;

        public ApprovalProcessService(
            IGenericRepository<ApprovalProcess> repository,
            IApprovalProcessRepository approvalProcessRepository,
            IApprovalSettingsRepository approvalSettingsRepository,
            AppDbContext context,
            INotificationService notificationService,
            IAdvanceRequestRepository advanceRequestRepository)
        {
            _repository = repository;
            _approvalProcessRepository = approvalProcessRepository;
            _approvalSettingsRepository = approvalSettingsRepository;
            _context = context;
            _notificationService = notificationService;
            _advanceRequestRepository = advanceRequestRepository;
        }

        public async Task<IEnumerable<ApprovalProcess>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<ApprovalProcess> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<ApprovalProcess> CreateAsync(ApprovalProcess approvalProcess)
        {
            return await _repository.AddAsync(approvalProcess);
        }

        public async Task<ApprovalProcess> UpdateAsync(ApprovalProcess approvalProcess)
        {
            await _repository.UpdateAsync(approvalProcess);
            return approvalProcess;
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity != null)
            {
                await _repository.RemoveAsync(entity);
            }
        }

        public async Task<IEnumerable<ApprovalProcess>> GetByAdvanceRequestIdAsync(int advanceRequestId)
        {
            return await _repository.GetAllAsync(x => x.AdvanceRequestId == advanceRequestId);
        }

        public async Task<IEnumerable<ApprovalProcess>> GetByApproverUserIdAsync(int approverUserId)
        {
            return await _repository.GetAllAsync(x => x.ApproverUserId == approverUserId);
        }

        public async Task<IEnumerable<ApprovalProcess>> GetApprovalsByRequestIdAsync(int requestId)
        {
            return await _approvalProcessRepository.GetApprovalsByRequestIdAsync(requestId);
        }

        public async Task<IEnumerable<ApprovalProcess>> GetPendingApprovalsByUserIdAsync(int userId)
        {
            return await _approvalProcessRepository.GetPendingApprovalsByUserIdAsync(userId);
        }

        public async Task<IEnumerable<ApprovalProcess>> GetApprovalsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _approvalProcessRepository.GetApprovalsByDateRangeAsync(startDate, endDate);
        }

        public async Task<IEnumerable<ApprovalProcess>> GetOverdueApprovalsAsync()
        {
            return await _approvalProcessRepository.GetOverdueApprovalsAsync();
        }

        public async Task<ApprovalProcess> GetLastApprovalByRequestIdAsync(int requestId)
        {
            return await _approvalProcessRepository.GetLastApprovalByRequestIdAsync(requestId);
        }

        public async Task<bool> HasUserApprovedRequestAsync(int userId, int requestId)
        {
            return await _approvalProcessRepository.HasUserApprovedRequestAsync(userId, requestId);
        }

        public Task<IEnumerable<ApprovalProcess>> GetAllApprovalProcessesAsync()
        { throw new NotImplementedException(); }
        public Task<ApprovalProcess> GetApprovalProcessByIdAsync(int id)
        { throw new NotImplementedException(); }

        public async Task<bool> CreateApprovalProcessAsync(ApprovalProcess approvalProcess)
        {
            return await _context.ExecuteInTransactionAsync(async () =>
            {
                await _approvalProcessRepository.AddAsync(approvalProcess);
                return true;
            });
        }

        public async Task<bool> UpdateApprovalProcessAsync(ApprovalProcess approvalProcess)
        {
            return await _context.ExecuteInTransactionAsync(async () =>
            {
                _approvalProcessRepository.UpdateAsync(approvalProcess);
                return true;
            });
        }

        public async Task<bool> DeleteApprovalProcessAsync(int id)
        {
            return await _context.ExecuteInTransactionAsync(async () =>
            {
                var approvalProcess = await _approvalProcessRepository.GetByIdAsync(id);
                if (approvalProcess == null) return false;

                _approvalProcessRepository.RemoveAsync(approvalProcess);
                return true;
            });
        }

        public async Task<bool> ProcessApprovalAsync(int requestId, int userId, ApprovalStatus status, decimal? approvedAmount = null, string comments = null)
        {
            return await _context.ExecuteInTransactionAsync(async () =>
            {
                var request = await _repository.GetByIdAsync(requestId);
                if (request == null) return false;

                var approvalProcess = new ApprovalProcess
                {
                    AdvanceRequestId = requestId,
                    ApproverUserId = userId,
                    Level = request.CurrentLevel,
                    Status = status,
                    ApprovedAmount = approvedAmount,
                    Comments = comments,
                    CreatedAt = DateTime.UtcNow
                };

                if (status == ApprovalStatus.Approved)
                {
                    approvalProcess.ApprovedAt = DateTime.UtcNow;
                    request.CurrentLevel = await GetNextApprovalLevelAsync(requestId);
                }

                await _approvalProcessRepository.AddAsync(approvalProcess);
                return true;
            });
        }

        public async Task<bool> IsApprovalRequiredAsync(int requestId, ApprovalLevel level)
        {
            var request = await _advanceRequestRepository.GetAdvanceRequestWithDetailsAsync(requestId);
            if (request == null) return false;

            var settings = await _approvalSettingsRepository.GetSettingsByAmountAsync(request.Amount);
            return settings != null && settings.ApprovalLevel >= level;
        }

        public async Task<ApprovalLevel> GetNextApprovalLevelAsync(int requestId)
        {
            var request = await _advanceRequestRepository.GetAdvanceRequestWithDetailsAsync(requestId);
            if (request == null) return ApprovalLevel.None;

            var settings = await _approvalSettingsRepository.GetSettingsByAmountAsync(request.Amount);
            if (settings == null) return ApprovalLevel.None;

            return request.CurrentLevel < settings.ApprovalLevel ? request.CurrentLevel + 1 : ApprovalLevel.None;
        }

        public async Task<bool> ApproveAsync(int id, int userId, string notes)
        {
            var approvalProcess = await _approvalProcessRepository.GetByIdAsync(id);
            if (approvalProcess == null)
            {
                return false;
            }

            var request = await _advanceRequestRepository.GetAdvanceRequestWithDetailsAsync(approvalProcess.AdvanceRequestId);
            if (request == null)
            {
                return false;
            }

            approvalProcess.Status = ApprovalStatus.Approved;
            approvalProcess.Comments = notes;
            approvalProcess.ApprovedAt = DateTime.UtcNow;
            approvalProcess.UpdatedAt = DateTime.UtcNow;
            approvalProcess.UpdatedBy = userId;

            request.CurrentLevel = GetNextApprovalLevel(request.CurrentLevel);
            request.UpdatedAt = DateTime.UtcNow;
            request.UpdatedBy = userId;

            await _approvalProcessRepository.UpdateAsync(approvalProcess);
            await _advanceRequestRepository.UpdateAsync(request);
            await _notificationService.SendApprovalNotificationAsync(request);
            return true;
        }

        public async Task<bool> RejectAsync(int id, int userId, string notes)
        {
            var approvalProcess = await _approvalProcessRepository.GetByIdAsync(id);
            if (approvalProcess == null)
            {
                return false;
            }

            var request = await _advanceRequestRepository.GetAdvanceRequestWithDetailsAsync(approvalProcess.AdvanceRequestId);
            if (request == null)
            {
                return false;
            }

            approvalProcess.Status = ApprovalStatus.Rejected;
            approvalProcess.Comments = notes;
            approvalProcess.UpdatedAt = DateTime.UtcNow;
            approvalProcess.UpdatedBy = userId;

            request.Status = RequestStatus.Rejected;
            request.RejectionReason = notes;
            request.UpdatedAt = DateTime.UtcNow;
            request.UpdatedBy = userId;

            await _approvalProcessRepository.UpdateAsync(approvalProcess);
            await _advanceRequestRepository.UpdateAsync(request);
            await _notificationService.SendRejectionNotificationAsync(request);
            return true;
        }

        private ApprovalLevel GetNextApprovalLevel(ApprovalLevel currentLevel)
        {
            return currentLevel switch
            {
                ApprovalLevel.Personel => ApprovalLevel.BirimMuduru,
                ApprovalLevel.BirimMuduru => ApprovalLevel.Direktor,
                ApprovalLevel.Direktor => ApprovalLevel.GenelMudurYardimcisi,
                ApprovalLevel.GenelMudurYardimcisi => ApprovalLevel.GenelMudur,
                ApprovalLevel.GenelMudur => ApprovalLevel.FinansMuduru,
                ApprovalLevel.FinansMuduru => ApprovalLevel.OnMuhasebe,
                _ => currentLevel
            };
        }

        public async Task<IEnumerable<ApprovalProcess>> GetPendingApprovalsAsync(int userId)
        {
            return await _approvalProcessRepository.GetPendingApprovalsAsync(userId);
        }
    }
} 