using System;
using VarlikYönetimi.Core.Enums;

namespace VarlikYönetimi.API.DTOs.AdvanceRequestDTO
{
    public class AdvanceRequestDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public decimal Amount { get; set; }
        public decimal? ApprovedAmount { get; set; }
        public string Description { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime DesiredDate { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? LastReminderDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public DateTime? RepaymentDueDate { get; set; }
        public RequestStatus Status { get; set; }
        public ApprovalLevel CurrentLevel { get; set; }
        public string RejectionReason { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

   

    
}