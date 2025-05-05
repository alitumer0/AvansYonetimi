using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VarlikYönetimi.Core.Entities;
using VarlikYönetimi.Core.Enums;
using VarlikYönetimi.Data.Repositories;
using VarlikYönetimi.Data.Repositories.Interfaces;
using VarlikYönetimi.Services.Services.Interfaces;

namespace VarlikYönetimi.Services.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IUserService _userService;

        public NotificationService(
            INotificationRepository notificationRepository,
            IUserService userService)
        {
            _notificationRepository = notificationRepository;
            _userService = userService;
        }

        public async Task<Notification> GetByIdAsync(int id)
        {
            return await _notificationRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Notification>> GetByUserIdAsync(int userId)
        {
            return await _notificationRepository.GetNotificationsByUserIdAsync(userId);
        }

        public async Task<IEnumerable<Notification>> GetUnreadNotificationsAsync(int userId)
        {
            return await _notificationRepository.GetUnreadNotificationsByUserIdAsync(userId);
        }

        public async Task<int> GetUnreadCountAsync(int userId)
        {
            return await _notificationRepository.GetUnreadNotificationCountAsync(userId);
        }

        public async Task<bool> MarkAsReadAsync(int id)
        {
            var notification = await _notificationRepository.GetByIdAsync(id);
            if (notification == null)
            {
                return false;
            }

            notification.IsRead = true;
            notification.UpdatedAt = DateTime.UtcNow;
            await _notificationRepository.UpdateAsync(notification);
            return true;
        }

        public async Task<bool> MarkAllAsReadAsync(int userId)
        {
            var notifications = await _notificationRepository.GetUnreadNotificationsByUserIdAsync(userId);
            foreach (var notification in notifications)
            {
                notification.IsRead = true;
                notification.UpdatedAt = DateTime.UtcNow;
                await _notificationRepository.UpdateAsync(notification);
            }
            return true;
        }

        public async Task<bool> CreateAsync(Notification notification)
        {
            notification.CreatedAt = DateTime.UtcNow;
            await _notificationRepository.AddAsync(notification);
            return true;
        }

        public async Task<bool> UpdateAsync(Notification notification)
        {
            notification.UpdatedAt = DateTime.UtcNow;
            await _notificationRepository.UpdateAsync(notification);
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var notification = await _notificationRepository.GetByIdAsync(id);
            if (notification == null)
            {
                return false;
            }

            await _notificationRepository.RemoveAsync(notification);
            return true;
        }

        public async Task SendApprovalNotificationAsync(AdvanceRequest request)
        {
            var notification = new Notification
            {
                UserId = request.UserId,
                Title = "Avans Talebi Onaylandı",
                Message = $"Avans talebiniz {request.CurrentLevel} seviyesinde onaylandı.",
                Type = NotificationType.ApprovalCompleted,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            await CreateAsync(notification);
        }

        public async Task SendRejectionNotificationAsync(AdvanceRequest request)
        {
            var notification = new Notification
            {
                UserId = request.UserId,
                Title = "Avans Talebi Reddedildi",
                Message = $"Avans talebiniz reddedildi. Red Nedeni: {request.RejectionReason}",
                Type = NotificationType.Rejection,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            await CreateAsync(notification);
        }

        public async Task SendPaymentCompletedNotificationAsync(AdvanceRequest request)
        {
            var notification = new Notification
            {
                UserId = request.UserId,
                Title = "Ödeme Tamamlandı",
                Message = $"Avans talebiniz için ödeme tamamlandı.",
                Type = NotificationType.PaymentCompleted,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            await CreateAsync(notification);
        }

        public async Task SendOverduePaymentNotificationAsync(Payment payment)
        {
            var notification = new Notification
            {
                UserId = payment.AdvanceRequest.UserId,
                Title = "Gecikmiş Ödeme",
                Message = $"Ödemeniz gecikti. Lütfen en kısa sürede ödemenizi yapın.",
                Type = NotificationType.OverduePayment,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            await CreateAsync(notification);
        }

        public async Task SendNotificationAsync(int userId, string message)
        {
            var notification = new Notification
            {
                UserId = userId,
                Message = message,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            await _notificationRepository.AddAsync(notification);
        }
    }
} 