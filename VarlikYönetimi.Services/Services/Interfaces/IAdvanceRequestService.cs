using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VarlikYönetimi.Core.Entities;
using VarlikYönetimi.Core.Enums;

namespace VarlikYönetimi.Services.Services.Interfaces
{
    public interface IAdvanceRequestService : IGenericService<AdvanceRequest>
    {
        Task CheckTimeoutsAsync();
        Task<AdvanceRequest> GetByIdAsync(int id);
        Task<IEnumerable<AdvanceRequest>> GetAllAsync();
        Task<IEnumerable<AdvanceRequest>> GetByUserIdAsync(int userId);
        Task<IEnumerable<AdvanceRequest>> GetUserAdvanceRequestsAsync(int userId);
        Task<IEnumerable<AdvanceRequest>> GetPendingApprovalsAsync(int userId);
        Task<IEnumerable<AdvanceRequest>> GetByStatusAsync(RequestStatus status);
        Task<bool> CreateAsync(AdvanceRequest request);
        Task<bool> UpdateAsync(AdvanceRequest request);
        Task<bool> DeleteAsync(int id);
        Task<bool> ApproveRequestAsync(int id, int userId, decimal? approvedAmount, string notes);
        Task<bool> RejectRequestAsync(int id, int userId, string notes);

        // GM için ek metodlar
        Task<List<AdvanceRequest>> GetPendingAdvanceRequestsForGM();
        Task<int> GetTotalAdvanceRequestsCount();
        Task<int> GetTotalApprovedAdvanceRequestsCount();
        Task<List<AdvanceRequest>> GetAllAdvanceRequestsForGM();
        Task<bool> ApproveAdvanceRequestByGM(int id);
        Task<bool> RejectAdvanceRequestByGM(int id, string rejectionReason);
    }
}