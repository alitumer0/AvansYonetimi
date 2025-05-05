using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VarlikYönetimi.Core.Entities;
using VarlikYönetimi.Core.Enums;
using VarlikYönetimi.Services.Services.Interfaces;

namespace VarlikYönetimi.Services.Services.Interfaces
{
    public interface IPaymentService : IGenericService<Payment>
    {
        Task<Payment> GetPaymentByIdAsync(int id);
        Task<IEnumerable<Payment>> GetPaymentsByUserAsync(int userId);
        Task<IEnumerable<Payment>> GetPendingPaymentsAsync();
        Task<IEnumerable<Payment>> GetOverduePaymentsAsync();
        Task<IEnumerable<Payment>> GetPaymentsByStatusAsync(PaymentStatus status);
        Task<IEnumerable<Payment>> GetPaymentsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<Payment> CreatePaymentAsync(AdvanceRequest advanceRequest, int enteredByUserId);
        Task UpdatePaymentStatusAsync(int paymentId, PaymentStatus newStatus, int? deliveredByUserId = null);
        Task MarkPaymentAsOverdueAsync(int paymentId);
        Task<Payment> GetByIdAsync(int id);
        Task<IEnumerable<Payment>> GetAllAsync();
        Task<IEnumerable<Payment>> GetPaymentsByUserIdAsync(int userId);
        Task<Payment> CreateAsync(Payment payment);
        Task<Payment> UpdateAsync(Payment payment);
        Task<bool> DeleteAsync(int id);
        Task UpdatePaymentStatusAsync(int id, PaymentStatus newStatus, int userId);
    }
} 