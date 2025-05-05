using System.Collections.Generic;
using System.Threading.Tasks;
using VarlikYönetimi.Core.Entities;
using VarlikYönetimi.Core.Enums;

namespace VarlikYönetimi.Data.Repositories.Interfaces
{
    public interface IAdvanceRequestRepository : IGenericRepository<AdvanceRequest>
    {
        Task<AdvanceRequest> GetAdvanceRequestWithDetailsAsync(int id);
        Task<IEnumerable<AdvanceRequest>> GetPendingApprovalsAsync(int userId);
        Task<IEnumerable<AdvanceRequest>> GetUserAdvanceRequestsAsync(int userId);
        Task<IEnumerable<AdvanceRequest>> GetPendingPaymentDateRequestsAsync();
        Task<IEnumerable<AdvanceRequest>> GetPendingPaymentsAsync();
        Task<IEnumerable<AdvanceRequest>> GetOverduePaymentsAsync();
    }
} 