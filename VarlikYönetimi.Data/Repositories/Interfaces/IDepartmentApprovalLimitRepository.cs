using VarlikYönetimi.Core.Entities;
using VarlikYönetimi.Core.Enums;

namespace VarlikYönetimi.Data.Repositories.Interfaces
{
    public interface IDepartmentApprovalLimitRepository : IGenericRepository<DepartmentApprovalLimit>
    {
        Task<DepartmentApprovalLimit> GetByDepartmentAndLevelAsync(int departmentId, ApprovalLevel approvalLevel);
        Task<List<DepartmentApprovalLimit>> GetByDepartmentAsync(int departmentId);
        Task<List<DepartmentApprovalLimit>> GetByApprovalLevelAsync(ApprovalLevel approvalLevel);
    }
} 