using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VarlikYönetimi.Core.Entities;
using VarlikYönetimi.Core.Enums;
using VarlikYönetimi.Data.Repositories;
using VarlikYönetimi.Data.Context;
using VarlikYönetimi.Core.Services;
using VarlikYönetimi.Data.Repositories.Interfaces;
using VarlikYönetimi.Services.Services.Interfaces;

namespace VarlikYönetimi.Services.Services
{
    public class PaymentService : GenericService<Payment>, IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IAdvanceRequestRepository _advanceRequestRepository;
        private readonly INotificationService _notificationService;
        private readonly AppDbContext _context;

        public PaymentService(
            IPaymentRepository paymentRepository,
            IAdvanceRequestRepository advanceRequestRepository,
            INotificationService notificationService,
            AppDbContext context) : base(paymentRepository)
        {
            _paymentRepository = paymentRepository;
            _advanceRequestRepository = advanceRequestRepository;
            _notificationService = notificationService;
            _context = context;
        }

        public async Task<Payment> GetPaymentByIdAsync(int id)
        {
            return await _paymentRepository.GetPaymentWithDetailsAsync(id);
        }

        public async Task<IEnumerable<Payment>> GetPaymentsByUserAsync(int userId)
        {
            return await _paymentRepository.GetPaymentsByUserAsync(userId);
        }

        public async Task<IEnumerable<Payment>> GetPendingPaymentsAsync()
        {
            return await _paymentRepository.GetPendingPaymentsAsync();
        }

        public async Task<IEnumerable<Payment>> GetOverduePaymentsAsync()
        {
            return await _paymentRepository.GetOverduePaymentsAsync();
        }

        public Task<IEnumerable<Payment>> GetPaymentsByStatusAsync(PaymentStatus status)
        { throw new NotImplementedException(); }
        
        public async Task<IEnumerable<Payment>> GetPaymentsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _paymentRepository.GetPaymentsByDateRangeAsync(startDate, endDate);
        }

        public async Task<Payment> CreatePaymentAsync(AdvanceRequest advanceRequest, int enteredByUserId)
        {
            return await _context.ExecuteInTransactionAsync(async () =>
            {
                var payment = new Payment
                {
                    AdvanceRequestId = advanceRequest.Id,
                    Amount = advanceRequest.Amount,
                    Status = PaymentStatus.Pending,
                    EnteredByUserId = enteredByUserId,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                await _paymentRepository.AddAsync(payment);
                await _context.SaveChangesAsync();
                return payment;
            });
        }

        Task IPaymentService.UpdatePaymentStatusAsync(int paymentId, PaymentStatus newStatus, int? deliveredByUserId)
        { return UpdatePaymentStatusAsync(paymentId, newStatus, deliveredByUserId); }

        public async Task UpdatePaymentStatusAsync(int paymentId, PaymentStatus newStatus, int? deliveredByUserId = null)
        {
            await _context.ExecuteInTransactionAsync(async () =>
            {
                var payment = await _paymentRepository.GetByIdAsync(paymentId);
                if (payment == null)
                    throw new ArgumentException("Ödeme bulunamadı.");

                payment.Status = newStatus;
                payment.UpdatedAt = DateTime.Now;

                if (deliveredByUserId.HasValue)
                    payment.DeliveredByUserId = deliveredByUserId.Value;

                _paymentRepository.UpdateAsync(payment);
                await _context.SaveChangesAsync();

                if (newStatus == PaymentStatus.Paid)
                    await _notificationService.SendPaymentCompletedNotificationAsync(payment.AdvanceRequest);
                else if (newStatus == PaymentStatus.Overdue)
                    await _notificationService.SendOverduePaymentNotificationAsync(payment);
            });
        }

        public async Task MarkPaymentAsOverdueAsync(int paymentId)
        {
            await _context.ExecuteInTransactionAsync(async () =>
            {
                var payment = await _paymentRepository.GetByIdAsync(paymentId);
                if (payment == null)
                    throw new ArgumentException("Ödeme bulunamadı.");

                if (payment.Status == PaymentStatus.Pending && payment.PaymentDate.HasValue && payment.PaymentDate.Value.Date < DateTime.Now.Date)
                {
                    payment.Status = PaymentStatus.Overdue;
                    payment.UpdatedAt = DateTime.Now;
                    _paymentRepository.UpdateAsync(payment);
                    await _context.SaveChangesAsync();
                    await _notificationService.SendOverduePaymentNotificationAsync(payment);
                }
            });
        }

        public async Task<IEnumerable<Payment>> GetPaymentsByUserIdAsync(int userId)
        {
            return await _paymentRepository.FindAsync(p => p.AdvanceRequest.UserId == userId);
        }

        public Task<Payment> CreateAsync(Payment payment)
        { throw new NotImplementedException(); }
        public Task<Payment> UpdateAsync(Payment payment)
        { throw new NotImplementedException(); }
        public async Task<bool> DeleteAsync(int id)
        {
            var payment = await _paymentRepository.GetByIdAsync(id);
            if (payment != null)
            {
                _paymentRepository.RemoveAsync(payment);
                return true;
            }
            return false;
        }
        public Task UpdatePaymentStatusAsync(int id, PaymentStatus newStatus, int userId)
        { throw new NotImplementedException(); }
    }
} 