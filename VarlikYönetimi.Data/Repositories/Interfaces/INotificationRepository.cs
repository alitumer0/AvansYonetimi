using System.Collections.Generic;
using System.Threading.Tasks;
using VarlikYönetimi.Core.Entities;

namespace VarlikYönetimi.Data.Repositories.Interfaces
{
    public interface INotificationRepository : IGenericRepository<Notification>
    {
        Task<int> GetUnreadNotificationCountAsync(int userId);
        Task<List<Notification>> GetUnreadNotificationsByUserIdAsync(int userId);
        Task<List<Notification>> GetNotificationsByUserIdAsync(int userId);
        Task<List<Notification>> GetNotificationsByAdvanceRequestAsync(int advanceRequestId);
        Task MarkAsReadAsync(int notificationId);
        Task MarkAllAsReadAsync(int userId);
        Task SendApprovalNotificationAsync(AdvanceRequest request, User approver);
        Task SendApprovalCompletedNotificationAsync(AdvanceRequest request);
        Task SendRejectionNotificationAsync(AdvanceRequest request, string comments);
        Task SendPaymentDateSetNotificationAsync(AdvanceRequest request);
        Task SendPaymentCompletedNotificationAsync(AdvanceRequest request);
        Task SendRepaymentCompletedNotificationAsync(AdvanceRequest request);
        Task SendPaymentReminderAsync(Payment payment);
        Task SendOverduePaymentNotificationAsync(Payment payment);
    }
} 