using System;
using System.Collections.Generic;
using VarlikYönetimi.Core.Enums;
using System.ComponentModel.DataAnnotations;
using VarlikYönetimi.Core.Entities;

namespace VarlikYönetimi.Core.ViewModels
{
    public class DashboardViewModel
    {
        public List<AdvanceRequest> PendingAdvanceRequests { get; set; }
        public IEnumerable<AdvanceRequest> PendingRequests { get; set; }
        public IEnumerable<Payment> PendingPayments { get; set; }
        public IEnumerable<Payment> OverduePayments { get; set; }
        public int TotalUsers { get; set; }
        public int TotalAdvanceRequests { get; set; }
        public int TotalApprovedAdvanceRequests { get; set; }
        public IEnumerable<PaymentViewModel> RecentPayments { get; set; }
        public IEnumerable<NotificationViewModel> RecentNotifications { get; set; }
        public int UnreadNotificationCount { get; set; }
        public decimal TotalPendingAmount { get; set; }
        public decimal TotalApprovedAmount { get; set; }
        public decimal TotalRejectedAmount { get; set; }
    }

    public class PaymentViewModel
    {
        public int Id { get; set; }
        public int AdvanceRequestId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public PaymentStatus Status { get; set; }
        public string Description { get; set; }
    }

    public class NotificationViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }
} 