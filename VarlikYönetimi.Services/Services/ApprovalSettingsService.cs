using System;
using System.Linq;
using System.Threading.Tasks;
using VarlikYönetimi.Core.Entities;
using VarlikYönetimi.Services.Services.Interfaces;
using VarlikYönetimi.Data.Context;
using Microsoft.EntityFrameworkCore;
using VarlikYönetimi.Core.Enums;
using VarlikYönetimi.Data.Repositories.Interfaces;
using VarlikYönetimi.Services.Services;

namespace VarlikYönetimi.Core.Services
{
    public class ApprovalSettingsService : GenericService<ApprovalSettings>, IApprovalSettingsService
    {
        private readonly IApprovalSettingsRepository _approvalSettingsRepository;
        private readonly AppDbContext _context;
        private const string BM_LIMIT_KEY = "BM_LIMIT";
        private const string DIREKTOR_LIMIT_KEY = "DIREKTOR_LIMIT";
        private const string GMY_LIMIT_KEY = "GMY_LIMIT";
        private const string APPROVAL_TIMEOUT_DAYS_KEY = "APPROVAL_TIMEOUT_DAYS";
        private const string PAYMENT_TIMEOUT_DAYS_KEY = "PAYMENT_TIMEOUT_DAYS";
        private const string REPAYMENT_TIMEOUT_DAYS_KEY = "REPAYMENT_TIMEOUT_DAYS";

        public ApprovalSettingsService(IApprovalSettingsRepository approvalSettingsRepository, AppDbContext context)
            : base(approvalSettingsRepository)
        {
            _approvalSettingsRepository = approvalSettingsRepository;
            _context = context;
        }

        public async Task<decimal> GetBmLimitAsync()
        {
            return await _context.ExecuteInTransactionAsync(async () =>
            {
                var setting = await _approvalSettingsRepository.GetByKeyAsync("BM_LIMIT");
                return setting != null ? decimal.Parse(setting.SettingValue) : 10000;
            });
        }

        public async Task<decimal> GetDirektorLimitAsync()
        {
            return await _context.ExecuteInTransactionAsync(async () =>
            {
                var setting = await _approvalSettingsRepository.GetByKeyAsync("DIREKTOR_LIMIT");
                return setting != null ? decimal.Parse(setting.SettingValue) : 50000;
            });
        }

        public async Task<decimal> GetGmyLimitAsync()
        {
            return await _context.ExecuteInTransactionAsync(async () =>
            {
                var setting = await _approvalSettingsRepository.GetByKeyAsync("GMY_LIMIT");
                return setting != null ? decimal.Parse(setting.SettingValue) : 100000;
            });
        }

        public async Task<int> GetApprovalTimeoutDaysAsync()
        {
            return await _context.ExecuteInTransactionAsync(async () =>
            {
                var setting = await _approvalSettingsRepository.GetByKeyAsync("APPROVAL_TIMEOUT_DAYS");
                return setting != null ? int.Parse(setting.SettingValue) : 7;
            });
        }

        public async Task<int> GetPaymentTimeoutDaysAsync()
        {
            return await _context.ExecuteInTransactionAsync(async () =>
            {
                var setting = await _approvalSettingsRepository.GetByKeyAsync("PAYMENT_TIMEOUT_DAYS");
                return setting != null ? int.Parse(setting.SettingValue) : 30;
            });
        }

        public async Task<int> GetRepaymentTimeoutDaysAsync()
        {
            return await _context.ExecuteInTransactionAsync(async () =>
            {
                var setting = await _approvalSettingsRepository.GetByKeyAsync("REPAYMENT_TIMEOUT_DAYS");
                return setting != null ? int.Parse(setting.SettingValue) : 90;
            });
        }

        public async Task UpdateBmLimitAsync(decimal limit, int userId)
        {
            await _context.ExecuteInTransactionAsync(async () =>
            {
                await UpdateSettingValueAsync("BM_LIMIT", limit.ToString(), userId);
            });
        }

        public async Task UpdateDirektorLimitAsync(decimal limit, int userId)
        {
            await _context.ExecuteInTransactionAsync(async () =>
            {
                await UpdateSettingValueAsync("DIREKTOR_LIMIT", limit.ToString(), userId);
            });
        }

        public async Task UpdateGmyLimitAsync(decimal limit, int userId)
        {
            await _context.ExecuteInTransactionAsync(async () =>
            {
                await UpdateSettingValueAsync("GMY_LIMIT", limit.ToString(), userId);
            });
        }

        public async Task UpdateApprovalTimeoutDaysAsync(int days, int userId)
        {
            await _context.ExecuteInTransactionAsync(async () =>
            {
                await UpdateSettingValueAsync("APPROVAL_TIMEOUT_DAYS", days.ToString(), userId);
            });
        }

        public async Task UpdatePaymentTimeoutDaysAsync(int days, int userId)
        {
            await _context.ExecuteInTransactionAsync(async () =>
            {
                await UpdateSettingValueAsync("PAYMENT_TIMEOUT_DAYS", days.ToString(), userId);
            });
        }

        public async Task UpdateRepaymentTimeoutDaysAsync(int days, int userId)
        {
            await _context.ExecuteInTransactionAsync(async () =>
            {
                await UpdateSettingValueAsync("REPAYMENT_TIMEOUT_DAYS", days.ToString(), userId);
            });
        }

        public async Task<ApprovalSettings> GetSettingsByAmountAsync(decimal amount)
        {
            return await _context.ExecuteInTransactionAsync(async () =>
            {
                var settings = await _approvalSettingsRepository.GetAllAsync();
                return settings.FirstOrDefault(s => amount >= s.MinAmount && amount <= s.MaxAmount);
            });
        }

        public async Task<ApprovalSettings> GetSettingsByLevelAsync(ApprovalLevel level)
        {
            return await _context.ExecuteInTransactionAsync(async () =>
            {
                var settings = await _approvalSettingsRepository.GetAllAsync();
                return settings.FirstOrDefault(s => s.ApprovalLevel == level);
            });
        }

        private async Task UpdateSettingValueAsync(string key, string value, int userId)
        {
            var setting = await _approvalSettingsRepository.GetByKeyAsync(key);
            if (setting == null)
            {
                setting = new ApprovalSettings
                {
                    SettingKey = key,
                    SettingValue = value,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    UpdatedByUserId = userId
                };
                await _approvalSettingsRepository.AddAsync(setting);
            }
            else
            {
                setting.SettingValue = value;
                setting.UpdatedAt = DateTime.Now;
                setting.UpdatedByUserId = userId;
                _approvalSettingsRepository.UpdateAsync(setting);
            }
        }

        private string GetDefaultDescription(string key)
        {
            return key switch
            {
                BM_LIMIT_KEY => "Birim Müdürü onay limiti",
                DIREKTOR_LIMIT_KEY => "Direktör onay limiti",
                GMY_LIMIT_KEY => "GMY onay limiti",
                APPROVAL_TIMEOUT_DAYS_KEY => "Onay zaman aşımı süresi (gün)",
                PAYMENT_TIMEOUT_DAYS_KEY => "Ödeme zaman aşımı süresi (gün)",
                REPAYMENT_TIMEOUT_DAYS_KEY => "Geri ödeme zaman aşımı süresi (gün)",
                _ => string.Empty
            };
        }

        public async Task<ApprovalSettings> GetByKeyAsync(string key)
        {
            return await _approvalSettingsRepository.GetByKeyAsync(key);
        }

        public async Task<int> GetTimeoutDaysAsync()
        {
            var setting = await GetByKeyAsync("TimeoutDays");
            return int.Parse(setting?.SettingValue ?? "3");
        }

        public async Task<int> GetMaxAdvanceAmountAsync()
        {
            var setting = await GetByKeyAsync("MaxAdvanceAmount");
            return int.Parse(setting?.SettingValue ?? "10000");
        }

        public async Task<int> GetMaxAdvanceDaysAsync()
        {
            var setting = await GetByKeyAsync("MaxAdvanceDays");
            return int.Parse(setting?.SettingValue ?? "30");
        }

        public async Task<int> GetMaxRepaymentDaysAsync()
        {
            var setting = await GetByKeyAsync("MaxRepaymentDays");
            return int.Parse(setting?.SettingValue ?? "60");
        }

        public Task<IEnumerable<ApprovalSettings>> GetAllSettingsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateSettingsAsync(ApprovalSettings settings)
        {
            throw new NotImplementedException();
        }

        public Task<bool> CreateSettingsAsync(ApprovalSettings settings)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteSettingsAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
} 