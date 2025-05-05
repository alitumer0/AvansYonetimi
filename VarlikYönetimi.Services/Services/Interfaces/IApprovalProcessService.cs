using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VarlikYönetimi.Core.Entities;
using VarlikYönetimi.Core.Enums;

namespace VarlikYönetimi.Services.Services.Interfaces
{
    public interface IApprovalProcessService
    {
        Task<ApprovalProcess> GetByIdAsync(int id);
        Task<IEnumerable<ApprovalProcess>> GetPendingApprovalsAsync(int userId);
        Task<bool> ApproveAsync(int id, int userId, string notes);
        Task<bool> RejectAsync(int id, int userId, string notes);
        Task<IEnumerable<ApprovalProcess>> GetAllApprovalProcessesAsync();
        Task<bool> CreateApprovalProcessAsync(ApprovalProcess approvalProcess);
        Task<bool> UpdateApprovalProcessAsync(ApprovalProcess approvalProcess);

        Task<bool> DeleteApprovalProcessAsync(int id);
        Task<IEnumerable<ApprovalProcess>> GetApprovalsByRequestIdAsync(int requestId);
        Task<IEnumerable<ApprovalProcess>> GetApprovalsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<ApprovalProcess>> GetOverdueApprovalsAsync();
        Task<ApprovalProcess> GetLastApprovalByRequestIdAsync(int requestId);
        Task<bool> HasUserApprovedRequestAsync(int userId, int requestId);
        Task<bool> ProcessApprovalAsync(int requestId, int userId, ApprovalStatus status, decimal? approvedAmount = null, string comments = null);
        Task<bool> IsApprovalRequiredAsync(int requestId, ApprovalLevel level);
        Task<ApprovalLevel> GetNextApprovalLevelAsync(int requestId);
    }
} 