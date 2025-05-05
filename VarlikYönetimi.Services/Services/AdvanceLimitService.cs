using VarlikYönetimi.Core.Entities;
using VarlikYönetimi.Data.Repositories;
using VarlikYönetimi.Services.Services.Interfaces;
using VarlikYönetimi.Core.Enums;
using Microsoft.EntityFrameworkCore;
using VarlikYönetimi.Data.Repositories.Interfaces;
using System.Linq;

namespace VarlikYönetimi.Services.Services
{
    public class AdvanceLimitService : GenericService<AdvanceLimit>, IAdvanceLimitService
    {
        private readonly IGenericRepository<AdvanceLimit> _repository;

        public AdvanceLimitService(IGenericRepository<AdvanceLimit> repository) : base(repository)
        {
            _repository = repository;
        }

        public async Task<AdvanceLimit> GetByDepartmentAndLevelAsync(int? departmentId, ApprovalLevel approvalLevel)
        {
            var limits = await _repository.GetAllAsync();
            return limits
                .Where(x => x.IsActive && 
                          x.DepartmentId == departmentId && 
                          x.ApprovalLevel == approvalLevel)
                .OrderByDescending(x => x.UpdatedAt)
                .FirstOrDefault();
        }

        public async Task<List<AdvanceLimit>> GetByDepartmentAsync(int? departmentId)
        {
            var limits = await _repository.GetAllAsync();
            return limits
                .Where(x => x.IsActive && x.DepartmentId == departmentId)
                .OrderByDescending(x => x.UpdatedAt)
                .ToList();
        }

        public async Task<List<AdvanceLimit>> GetByApprovalLevelAsync(ApprovalLevel approvalLevel)
        {
            var limits = await _repository.GetAllAsync();
            return limits
                .Where(x => x.IsActive && x.ApprovalLevel == approvalLevel)
                .OrderByDescending(x => x.UpdatedAt)
                .ToList();
        }

        public async Task<AdvanceLimit> GetGeneralLimitAsync(ApprovalLevel approvalLevel)
        {
            return await GetByDepartmentAndLevelAsync(null, approvalLevel);
        }
    }
} 