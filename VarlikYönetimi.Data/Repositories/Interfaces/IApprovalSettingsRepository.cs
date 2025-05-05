using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using VarlikYönetimi.Core.Entities;
using VarlikYönetimi.Core.Enums;

namespace VarlikYönetimi.Data.Repositories.Interfaces
{
    public interface IApprovalSettingsRepository : IGenericRepository<ApprovalSettings>
    {
        Task<ApprovalSettings> GetByKeyAsync(string key);
        Task<ApprovalSettings> GetByLevelAsync(int level);
        Task<ApprovalSettings> GetByAmountAsync(decimal amount);
        Task<ApprovalSettings> GetSettingsByAmountAsync(decimal amount);
        Task<IEnumerable<ApprovalSettings>> GetAllSettingsAsync();
        Task<bool> UpdateSettingsAsync(ApprovalSettings settings);
        Task<bool> CreateSettingsAsync(ApprovalSettings settings);
        Task<bool> DeleteSettingsAsync(int id);
        Task<ApprovalSettings> GetByRoleIdAsync(int roleId);
        Task<IEnumerable<ApprovalSettings>> GetAllWithRolesAsync();
        Task<bool> UpdateApprovalSettingsAsync(ApprovalSettings settings);
        Task<bool> DeleteApprovalSettingsAsync(int id);
    }
} 