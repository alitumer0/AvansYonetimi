using VarlikYönetimi.Core.Entities;
using VarlikYönetimi.Core.Enums;

namespace VarlikYönetimi.Services.Services.Interfaces
{
    public interface IAdvanceLimitService : IGenericService<AdvanceLimit>
    {
        Task<AdvanceLimit> GetByDepartmentAndLevelAsync(int? departmentId, ApprovalLevel approvalLevel);
        Task<List<AdvanceLimit>> GetByDepartmentAsync(int? departmentId);
        Task<List<AdvanceLimit>> GetByApprovalLevelAsync(ApprovalLevel approvalLevel);
        Task<AdvanceLimit> GetGeneralLimitAsync(ApprovalLevel approvalLevel);
    }
} 