using System.Collections.Generic;
using System.Threading.Tasks;
using VarlikYönetimi.Core.Entities;

namespace VarlikYönetimi.Services.Services.Interfaces
{
    public interface INotificationService
    {
        Task SendNotificationAsync(int userId, string message);
        Task<Notification> GetByIdAsync(int id);
        Task<IEnumerable<Notification>> GetByUserIdAsync(int userId);
        Task<IEnumerable<Notification>> GetUnreadNotificationsAsync(int userId);
        Task<int> GetUnreadCountAsync(int userId);
        Task<bool> MarkAsReadAsync(int id);
        Task<bool> MarkAllAsReadAsync(int userId);
        Task<bool> CreateAsync(Notification notification);
        Task<bool> UpdateAsync(Notification notification);
        Task<bool> DeleteAsync(int id);
        Task SendApprovalNotificationAsync(AdvanceRequest request);
        Task SendRejectionNotificationAsync(AdvanceRequest request);
        Task SendPaymentCompletedNotificationAsync(AdvanceRequest request);
        Task SendOverduePaymentNotificationAsync(Payment payment);
    }
} 