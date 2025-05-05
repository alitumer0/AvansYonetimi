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
    public class NotificationRepository : GenericRepository<Notification>, INotificationRepository
    {
        public NotificationRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<int> GetUnreadNotificationCountAsync(int userId)
        {
            return await _context.Notifications
                .CountAsync(n => n.UserId == userId && !n.IsRead);
        }

        public async Task<List<Notification>> GetUnreadNotificationsByUserIdAsync(int userId)
        {
            return await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Notification>> GetNotificationsByUserIdAsync(int userId)
        {
            return await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Notification>> GetNotificationsByAdvanceRequestAsync(int advanceRequestId)
        {
            return await _context.Notifications
                .Where(n => n.AdvanceRequestId == advanceRequestId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task MarkAsReadAsync(int notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification != null)
            {
                notification.IsRead = true;
                notification.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task MarkAllAsReadAsync(int userId)
        {
            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            foreach (var notification in notifications)
            {
                notification.IsRead = true;
                notification.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }

        public async Task SendApprovalNotificationAsync(AdvanceRequest request, User approver)
        {
            var notification = new Notification
            {
                UserId = approver.Id,
                Title = "Yeni Onay Talebi",
                Message = $"{request.User.FirstName} tarafından {request.Amount:C2} tutarında yeni bir avans talebi oluşturuldu.",
                Type = NotificationType.ApprovalRequest,
                AdvanceRequestId = request.Id,
                CreatedAt = DateTime.UtcNow
            };

            await AddAsync(notification);
        }

        public async Task SendApprovalCompletedNotificationAsync(AdvanceRequest request)
        {
            var notification = new Notification
            {
                UserId = request.UserId,
                Title = "Avans Talebi Onaylandı",
                Message = $"{request.Amount:C2} tutarında oluşturduğunuz avans talebi onaylandı.",
                Type = NotificationType.ApprovalCompleted,
                AdvanceRequestId = request.Id,
                CreatedAt = DateTime.UtcNow
            };

            await AddAsync(notification);
        }

        public async Task SendRejectionNotificationAsync(AdvanceRequest request, string comments)
        {
            var notification = new Notification
            {
                UserId = request.UserId,
                Title = "Avans Talebi Reddedildi",
                Message = $"{request.Amount:C2} tutarında oluşturduğunuz avans talebi reddedildi. Sebep: {comments}",
                Type = NotificationType.Rejection,
                AdvanceRequestId = request.Id,
                CreatedAt = DateTime.UtcNow
            };

            await AddAsync(notification);
        }

        public async Task SendPaymentDateSetNotificationAsync(AdvanceRequest request)
        {
            var notification = new Notification
            {
                UserId = request.UserId,
                Title = "Ödeme Tarihi Belirlendi",
                Message = $"{request.Amount:C2} tutarında oluşturduğunuz avans talebi için ödeme tarihi belirlendi: {request.Payments?.FirstOrDefault()?.PaymentDate:dd.MM.yyyy}",
                Type = NotificationType.PaymentDateSet,
                AdvanceRequestId = request.Id,
                CreatedAt = DateTime.UtcNow
            };

            await AddAsync(notification);
        }

        public async Task SendPaymentCompletedNotificationAsync(AdvanceRequest request)
        {
            var notification = new Notification
            {
                UserId = request.UserId,
                Title = "Ödeme Tamamlandı",
                Message = $"{request.Amount:C2} tutarında oluşturduğunuz avans talebi için ödeme tamamlandı.",
                Type = NotificationType.PaymentCompleted,
                AdvanceRequestId = request.Id,
                CreatedAt = DateTime.UtcNow
            };

            await AddAsync(notification);
        }

        public async Task SendRepaymentCompletedNotificationAsync(AdvanceRequest request)
        {
            var notification = new Notification
            {
                UserId = request.UserId,
                Title = "Geri Ödeme Tamamlandı",
                Message = $"{request.Amount:C2} tutarında oluşturduğunuz avans talebi için geri ödeme tamamlandı.",
                Type = NotificationType.RepaymentCompleted,
                AdvanceRequestId = request.Id,
                CreatedAt = DateTime.UtcNow
            };

            await AddAsync(notification);
        }

        public async Task SendPaymentReminderAsync(Payment payment)
        {
            var notification = new Notification
            {
                UserId = payment.AdvanceRequest.UserId,
                Title = "Ödeme Hatırlatması",
                Message = $"{payment.Amount:C2} tutarında ödemeniz için son gün yaklaşıyor. Ödeme Tarihi: {payment.PaymentDate:dd.MM.yyyy}",
                Type = NotificationType.PaymentReminder,
                AdvanceRequestId = payment.AdvanceRequestId,
                CreatedAt = DateTime.UtcNow
            };

            await AddAsync(notification);
        }

        public async Task SendOverduePaymentNotificationAsync(Payment payment)
        {
            var notification = new Notification
            {
                UserId = payment.AdvanceRequest.UserId,
                Title = "Gecikmiş Ödeme",
                Message = $"{payment.Amount:C2} tutarında ödemeniz gecikti. Lütfen en kısa sürede ödeme yapınız.",
                Type = NotificationType.OverduePayment,
                AdvanceRequestId = payment.AdvanceRequestId,
                CreatedAt = DateTime.UtcNow
            };

            await AddAsync(notification);
        }
    }
} 