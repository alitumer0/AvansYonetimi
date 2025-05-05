using VarlikYönetimi.Core.Entities;
using VarlikYönetimi.Core.Enums;

namespace VarlikYönetimi.Services.Services.Interfaces
{
    public interface IDepartmentApprovalLimitService : IGenericService<DepartmentApprovalLimit>
    {
        Task<DepartmentApprovalLimit> GetByDepartmentAndLevelAsync(int departmentId, ApprovalLevel approvalLevel);
        Task<List<DepartmentApprovalLimit>> GetByDepartmentAsync(int departmentId);
        Task<List<DepartmentApprovalLimit>> GetByApprovalLevelAsync(ApprovalLevel approvalLevel);
        Task<Dictionary<int, List<DepartmentApprovalLimit>>> GetAllDepartmentLimitsAsync();
    }
} 