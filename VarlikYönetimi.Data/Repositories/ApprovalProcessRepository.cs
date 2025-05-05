using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VarlikYönetimi.Core.Entities;
using VarlikYönetimi.Core.Enums;
using VarlikYönetimi.Data.Repositories.Interfaces;
using VarlikYönetimi.Data.Context;

namespace VarlikYönetimi.Data.Repositories
{
    public class ApprovalProcessRepository : GenericRepository<ApprovalProcess>, IApprovalProcessRepository
    {
        private readonly AppDbContext _context;

        public ApprovalProcessRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ApprovalProcess>> GetApprovalsByRequestIdAsync(int requestId)
        {
            return await _context.ApprovalProcesses
                .Include(ap => ap.ApproverUser)
                .Where(ap => ap.AdvanceRequestId == requestId)
                .OrderBy(ap => ap.ApprovedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<ApprovalProcess>> GetPendingApprovalsAsync(int userId)
        {
            return await _context.ApprovalProcesses
                .Include(ap => ap.AdvanceRequest)
                .Where(ap => ap.ApproverUserId == userId && ap.Status == Core.Enums.ApprovalStatus.Pending)
                .OrderByDescending(ap => ap.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<ApprovalProcess>> GetPendingApprovalsByUserIdAsync(int userId)
        {
            return await _context.ApprovalProcesses
                .Include(ap => ap.AdvanceRequest)
                .Where(ap => ap.ApproverUserId == userId && ap.Status == ApprovalStatus.Pending)
                .OrderBy(ap => ap.ApprovedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<ApprovalProcess>> GetApprovalsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.ApprovalProcesses
                .Include(ap => ap.ApproverUser)
                .Include(ap => ap.AdvanceRequest)
                .Where(ap => ap.ApprovedAt >= startDate && ap.ApprovedAt <= endDate)
                .OrderBy(ap => ap.ApprovedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<ApprovalProcess>> GetOverdueApprovalsAsync()
        {
            var timeoutDays = await _context.ApprovalSettings
                .Where(s => s.SettingKey == "APPROVAL_TIMEOUT_DAYS")
                .Select(s => int.Parse(s.SettingValue))
                .FirstOrDefaultAsync();

            var timeoutDate = DateTime.Now.AddDays(-timeoutDays);

            return await _context.ApprovalProcesses
                .Include(ap => ap.ApproverUser)
                .Include(ap => ap.AdvanceRequest)
                .Where(ap => ap.Status == ApprovalStatus.Pending && ap.ApprovedAt <= timeoutDate)
                .OrderBy(ap => ap.ApprovedAt)
                .ToListAsync();
        }

        public async Task<ApprovalProcess> GetLastApprovalByRequestIdAsync(int requestId)
        {
            return await _context.ApprovalProcesses
                .Include(ap => ap.ApproverUser)
                .Where(ap => ap.AdvanceRequestId == requestId)
                .OrderByDescending(ap => ap.ApprovedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> HasUserApprovedRequestAsync(int userId, int requestId)
        {
            return await _context.ApprovalProcesses
                .AnyAsync(ap => ap.AdvanceRequestId == requestId && 
                              ap.ApproverUserId == userId && 
                              ap.Status == ApprovalStatus.Approved);
        }

        public async Task<IEnumerable<ApprovalProcess>> GetByAdvanceRequestIdAsync(int advanceRequestId)
        {
            return await _context.ApprovalProcesses
                .Where(x => x.AdvanceRequestId == advanceRequestId)
                .ToListAsync();
        }

        public async Task<IEnumerable<ApprovalProcess>> GetByApproverUserIdAsync(int approverUserId)
        {
            return await _context.ApprovalProcesses
                .Where(x => x.ApproverUserId == approverUserId)
                .ToListAsync();
        }

        public async Task<IEnumerable<ApprovalProcess>> GetProcessesByRequestIdAsync(int requestId)
        {
            return await _context.ApprovalProcesses
                .Include(ap => ap.ApproverUser)
                .Where(ap => ap.AdvanceRequestId == requestId)
                .OrderBy(ap => ap.ApprovedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<ApprovalProcess>> GetPendingProcessesByUserIdAsync(int userId)
        {
            return await _context.ApprovalProcesses
                .Include(ap => ap.AdvanceRequest)
                .Where(ap => ap.ApproverUserId == userId && ap.Status == ApprovalStatus.Pending)
                .OrderByDescending(ap => ap.CreatedAt)
                .ToListAsync();
        }

        public async Task<ApprovalProcess> GetProcessWithDetailsAsync(int id)
        {
            return await _context.ApprovalProcesses
                .Include(ap => ap.ApproverUser)
                .Include(ap => ap.AdvanceRequest)
                .FirstOrDefaultAsync(ap => ap.Id == id);
        }

        public async Task<bool> UpdateProcessStatusAsync(int id, ApprovalStatus status)
        {
            var process = await _context.ApprovalProcesses.FindAsync(id);
            if (process == null) return false;

            process.Status = status;
            process.ApprovedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CreateProcessAsync(ApprovalProcess process)
        {
            try
            {
                await _context.ApprovalProcesses.AddAsync(process);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
} 