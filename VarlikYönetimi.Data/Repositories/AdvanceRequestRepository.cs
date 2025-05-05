using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VarlikYönetimi.Core.Entities;
using VarlikYönetimi.Data.Repositories.Interfaces;
using VarlikYönetimi.Data.Context;
using VarlikYönetimi.Core.Enums;

namespace VarlikYönetimi.Data.Repositories
{
    public class AdvanceRequestRepository : GenericRepository<AdvanceRequest>, IAdvanceRequestRepository
    {
        public AdvanceRequestRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<AdvanceRequest> GetAdvanceRequestWithDetailsAsync(int id)
        {
            return await _context.AdvanceRequests
                .Include(a => a.User)
                .Include(a => a.AdvanceRequestProjects)
                    .ThenInclude(arp => arp.Project)
                .Include(a => a.Approvals)
                    .ThenInclude(ap => ap.ApproverUser)
                .Include(a => a.Payments)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<AdvanceRequest>> GetPendingApprovalsAsync(int userId)
        {
            return await _context.AdvanceRequests
                .Include(a => a.User)
                .Include(a => a.Approvals)
                .Where(a => a.Approvals.Any(ap => ap.ApproverUserId == userId && ap.Status == ApprovalStatus.Pending))
                .ToListAsync();
        }

        public async Task<IEnumerable<AdvanceRequest>> GetUserAdvanceRequestsAsync(int userId)
        {
            return await _context.AdvanceRequests
                .Include(a => a.Approvals)
                .Include(a => a.Payments)
                .Where(a => a.UserId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<AdvanceRequest>> GetPendingPaymentDateRequestsAsync()
        {
            return await _context.AdvanceRequests
                .Include(a => a.User)
                .Include(a => a.Approvals)
                .Where(a => a.Status == RequestStatus.Approved && a.Payments == null)
                .ToListAsync();
        }

        public async Task<IEnumerable<AdvanceRequest>> GetPendingPaymentsAsync()
        {
            return await _context.AdvanceRequests
                .Include(a => a.User)
                .Include(a => a.Payments)
                .Where(a => a.Status == RequestStatus.Approved && a.Payments != null && a.Payments.Any(p => p.Status == PaymentStatus.Pending))
                .ToListAsync();
        }

        public async Task<IEnumerable<AdvanceRequest>> GetOverduePaymentsAsync()
        {
            return await _context.AdvanceRequests
                .Include(a => a.User)
                .Include(a => a.Payments)
                .Where(a => a.Status == RequestStatus.Approved && a.Payments != null && a.Payments.Any(p => p.Status == PaymentStatus.Overdue))
                .ToListAsync();
        }
    }
} 