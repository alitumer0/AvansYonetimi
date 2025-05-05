using VarlikYönetimi.Core.Entities;
using VarlikYönetimi.Core.Enums;
using VarlikYönetimi.Data.Repositories.Interfaces;
using VarlikYönetimi.Services.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace VarlikYönetimi.Services.Services
{
    public class DepartmentApprovalLimitService : GenericService<DepartmentApprovalLimit>, IDepartmentApprovalLimitService
    {
        private readonly IGenericRepository<DepartmentApprovalLimit> _departmentApprovalLimitRepository;

        public DepartmentApprovalLimitService(IGenericRepository<DepartmentApprovalLimit> repository) : base(repository)
        {
            _departmentApprovalLimitRepository = repository;
        }

        public async Task<DepartmentApprovalLimit> GetByDepartmentAndLevelAsync(int departmentId, ApprovalLevel approvalLevel)
        {
            var limits = await _departmentApprovalLimitRepository.GetAllAsync();
            return limits
                .Where(x => x.DepartmentId == departmentId && x.ApprovalLevel == approvalLevel)
                .OrderByDescending(x => x.UpdatedAt)
                .FirstOrDefault();
        }

        public async Task<List<DepartmentApprovalLimit>> GetByDepartmentAsync(int departmentId)
        {
            var limits = await _departmentApprovalLimitRepository.GetAllAsync();
            return limits
                .Where(x => x.DepartmentId == departmentId)
                .OrderByDescending(x => x.UpdatedAt)
                .ToList();
        }

        public async Task<List<DepartmentApprovalLimit>> GetByApprovalLevelAsync(ApprovalLevel approvalLevel)
        {
            var limits = await _departmentApprovalLimitRepository.GetAllAsync();
            return limits
                .Where(x => x.ApprovalLevel == approvalLevel)
                .OrderByDescending(x => x.UpdatedAt)
                .ToList();
        }

        public async Task<Dictionary<int, List<DepartmentApprovalLimit>>> GetAllDepartmentLimitsAsync()
        {
            var limits = await _departmentApprovalLimitRepository.GetAllAsync();
            return limits
                .GroupBy(x => x.DepartmentId)
                .ToDictionary(
                    g => g.Key,
                    g => g.OrderByDescending(x => x.UpdatedAt).ToList()
                );
        }

        public override async Task<DepartmentApprovalLimit> AddAsync(DepartmentApprovalLimit entity)
        {
            entity.CreatedAt = DateTime.UtcNow;
            entity.UpdatedAt = DateTime.UtcNow;
            await _departmentApprovalLimitRepository.AddAsync(entity);
            return entity;
        }

        public override async Task UpdateAsync(DepartmentApprovalLimit entity)
        {
            entity.UpdatedAt = DateTime.UtcNow;
            await _departmentApprovalLimitRepository.UpdateAsync(entity);
        }
    }
} 