using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VarlikYönetimi.Core.Entities;
using VarlikYönetimi.Core.Enums;

namespace VarlikYönetimi.Data.Repositories.Interfaces
{
    public interface IApprovalProcessRepository : IGenericRepository<ApprovalProcess>
    {
        Task<IEnumerable<ApprovalProcess>> GetProcessesByRequestIdAsync(int requestId);
        Task<IEnumerable<ApprovalProcess>> GetPendingProcessesByUserIdAsync(int userId);
        Task<ApprovalProcess> GetProcessWithDetailsAsync(int id);
        Task<bool> UpdateProcessStatusAsync(int id, ApprovalStatus status);
        Task<bool> CreateProcessAsync(ApprovalProcess process);
        Task<IEnumerable<ApprovalProcess>> GetApprovalsByRequestIdAsync(int requestId);
        Task<IEnumerable<ApprovalProcess>> GetPendingApprovalsAsync(int userId);
        Task<IEnumerable<ApprovalProcess>> GetPendingApprovalsByUserIdAsync(int userId);
        Task<IEnumerable<ApprovalProcess>> GetApprovalsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<ApprovalProcess>> GetOverdueApprovalsAsync();
        Task<ApprovalProcess> GetLastApprovalByRequestIdAsync(int requestId);
        Task<bool> HasUserApprovedRequestAsync(int userId, int requestId);
        Task<IEnumerable<ApprovalProcess>> GetByAdvanceRequestIdAsync(int advanceRequestId);
        Task<IEnumerable<ApprovalProcess>> GetByApproverUserIdAsync(int approverUserId);
    }
} 