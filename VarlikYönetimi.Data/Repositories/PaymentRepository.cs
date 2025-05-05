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
    public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
    {
        public PaymentRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Payment> GetPaymentWithDetailsAsync(int id)
        {
            return await _context.Payments
                .Include(p => p.AdvanceRequest)
                    .ThenInclude(a => a.User) 
                .Include(p => p.EnteredByUser)
                .Include(p => p.DeliveredByUser)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Payment>> GetPaymentsByUserAsync(int userId)
        {
            return await _context.Payments
                .Include(p => p.AdvanceRequest)
                .Where(p => p.AdvanceRequest.UserId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Payment>> GetPendingPaymentsAsync()
        {
            return await _context.Payments
                .Include(p => p.AdvanceRequest)
                    .ThenInclude(a => a.User) 
                .Where(p => p.Status == PaymentStatus.Pending)
                .ToListAsync();
        }

        public async Task<IEnumerable<Payment>> GetOverduePaymentsAsync()
        {
            return await _context.Payments
                .Include(p => p.AdvanceRequest)
                    .ThenInclude(a => a.User) 
                .Where(p => p.Status == PaymentStatus.Overdue)
                .ToListAsync();
        }

        public async Task<IEnumerable<Payment>> GetPaymentsByStatusAsync(PaymentStatus status)
        {
            return await _context.Payments
                .Include(p => p.AdvanceRequest)
                    .ThenInclude(a => a.User)
                .Where(p => p.Status == status)
                .OrderBy(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Payment>> GetPaymentsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Payments
                .Include(p => p.AdvanceRequest)
                    .ThenInclude(a => a.User) 
                .Where(p => p.CreatedAt >= startDate && p.CreatedAt <= endDate)
                .OrderBy(p => p.CreatedAt)
                .ToListAsync();
        }
    }
} 