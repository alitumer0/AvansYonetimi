using Microsoft.EntityFrameworkCore;
using VarlikYönetimi.Core.Entities;
using VarlikYönetimi.Core.Enums;
using VarlikYönetimi.Data.Context;
using VarlikYönetimi.Data.Repositories.Interfaces;

namespace VarlikYönetimi.Data.Repositories
{
    public class DepartmentApprovalLimitRepository : GenericRepository<DepartmentApprovalLimit>, IDepartmentApprovalLimitRepository
    {
        private readonly AppDbContext _context;

        public DepartmentApprovalLimitRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<DepartmentApprovalLimit> GetByDepartmentAndLevelAsync(int departmentId, ApprovalLevel approvalLevel)
        {
            return await _context.DepartmentApprovalLimits
                .Where(x => x.DepartmentId == departmentId && x.ApprovalLevel == approvalLevel)
                .OrderByDescending(x => x.UpdatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<List<DepartmentApprovalLimit>> GetByDepartmentAsync(int departmentId)
        {
            return await _context.DepartmentApprovalLimits
                .Where(x => x.DepartmentId == departmentId)
                .OrderByDescending(x => x.UpdatedAt)
                .ToListAsync();
        }

        public async Task<List<DepartmentApprovalLimit>> GetByApprovalLevelAsync(ApprovalLevel approvalLevel)
        {
            return await _context.DepartmentApprovalLimits
                .Where(x => x.ApprovalLevel == approvalLevel)
                .OrderByDescending(x => x.UpdatedAt)
                .ToListAsync();
        }
    }
} 