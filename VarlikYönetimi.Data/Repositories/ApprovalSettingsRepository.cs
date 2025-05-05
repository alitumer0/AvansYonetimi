using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VarlikYönetimi.Core.Entities;
using VarlikYönetimi.Core.Enums;
using VarlikYönetimi.Data.Repositories;
using VarlikYönetimi.Data.Context;
using VarlikYönetimi.Data.Repositories.Interfaces;

namespace VarlikYönetimi.Data.Repositories
{
    public class ApprovalSettingsRepository : GenericRepository<ApprovalSettings>, IApprovalSettingsRepository
    {
        public ApprovalSettingsRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<ApprovalSettings> GetByKeyAsync(string key)
        {
            return await _context.ApprovalSettings
                .Include(a => a.Role)
                .Include(a => a.UpdatedByUser)
                .FirstOrDefaultAsync(a => a.SettingKey == key);
        }

        public async Task<ApprovalSettings> GetByLevelAsync(int level)
        {
            return await _context.ApprovalSettings
                .Include(a => a.Role)
                .Include(a => a.UpdatedByUser)
                .FirstOrDefaultAsync(a => a.Level == level);
        }

        public async Task<ApprovalSettings> GetByAmountAsync(decimal amount)
        {
            return await _context.ApprovalSettings
                .Include(a => a.Role)
                .Include(a => a.UpdatedByUser)
                .FirstOrDefaultAsync(a => amount >= a.MinAmount && amount <= a.MaxAmount);
        }

        public Task<ApprovalSettings> GetSettingsByAmountAsync(decimal amount)
        { throw new NotImplementedException(); }

        public async Task<IEnumerable<ApprovalSettings>> GetAllSettingsAsync()
        {
            return await _context.ApprovalSettings
                .Include(a => a.Role)
                .Include(a => a.UpdatedByUser)
                .ToListAsync();
        }

        public async Task<bool> UpdateSettingsAsync(ApprovalSettings settings)
        {
            try
            {
                _context.Entry(settings).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> CreateSettingsAsync(ApprovalSettings settings)
        {
            try
            {
                await _context.ApprovalSettings.AddAsync(settings);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteSettingsAsync(int id)
        {
            try
            {
                var settings = await _context.ApprovalSettings.FindAsync(id);
                if (settings != null)
                {
                    _context.ApprovalSettings.Remove(settings);
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public async Task<ApprovalSettings> GetByRoleIdAsync(int roleId)
        {
            return await _context.ApprovalSettings
                .Include(a => a.Role)
                .Include(a => a.UpdatedByUser)
                .FirstOrDefaultAsync(a => a.RoleId == roleId);
        }

        public async Task<IEnumerable<ApprovalSettings>> GetAllWithRolesAsync()
        {
            return await _context.ApprovalSettings
                .Include(a => a.Role)
                .Include(a => a.UpdatedByUser)
                .ToListAsync();
        }

        public async Task<bool> UpdateApprovalSettingsAsync(ApprovalSettings settings)
        {
            try
            {
                _context.Entry(settings).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteApprovalSettingsAsync(int id)
        {
            try
            {
                var settings = await _context.ApprovalSettings.FindAsync(id);
                if (settings != null)
                {
                    _context.ApprovalSettings.Remove(settings);
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
} 