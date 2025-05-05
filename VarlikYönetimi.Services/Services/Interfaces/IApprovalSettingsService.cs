using System.Collections.Generic;
using System.Threading.Tasks;
using VarlikYönetimi.Core.Entities;
using VarlikYönetimi.Core.Enums;

namespace VarlikYönetimi.Services.Services.Interfaces
{
    public interface IApprovalSettingsService
    {
        Task<ApprovalSettings> GetByKeyAsync(string key);
        Task<ApprovalSettings> GetSettingsByAmountAsync(decimal amount);
        Task<ApprovalSettings> GetSettingsByLevelAsync(ApprovalLevel level);
        Task<IEnumerable<ApprovalSettings>> GetAllSettingsAsync();
        Task<bool> UpdateSettingsAsync(ApprovalSettings settings);
        Task<bool> CreateSettingsAsync(ApprovalSettings settings);
        Task<bool> DeleteSettingsAsync(int id);
        Task<decimal> GetBmLimitAsync();
        Task<decimal> GetDirektorLimitAsync();
        Task<decimal> GetGmyLimitAsync();
        Task<int> GetApprovalTimeoutDaysAsync();
        Task<int> GetPaymentTimeoutDaysAsync();
        Task<int> GetRepaymentTimeoutDaysAsync();
        
        Task UpdateBmLimitAsync(decimal limit, int userId);
        Task UpdateDirektorLimitAsync(decimal limit, int userId);
        Task UpdateGmyLimitAsync(decimal limit, int userId);
        Task UpdateApprovalTimeoutDaysAsync(int days, int userId);
        Task UpdatePaymentTimeoutDaysAsync(int days, int userId);
        Task UpdateRepaymentTimeoutDaysAsync(int days, int userId);
    }
} 