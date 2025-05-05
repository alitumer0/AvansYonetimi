using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VarlikYönetimi.Core.Enums;

namespace VarlikYönetimi.Core.Entities
{
    public class AdvanceRequest : BaseEntity
    {
        public int Id { get; set; }
        
        [Required]
        public int UserId { get; set; }
        
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Tutar 0'dan büyük olmalıdır.")]
        public decimal Amount { get; set; }
        
        public decimal? ApprovedAmount { get; set; }
        
        [Required]
        public string Description { get; set; }
        
        [Required]
        public DateTime RequestDate { get; set; }
        
        [Required]
        public DateTime DesiredDate { get; set; }
        
        public DateTime? DueDate { get; set; }
        
        public DateTime? LastReminderDate { get; set; }
        
        public DateTime? ExpiryDate { get; set; }
        
        public DateTime? RepaymentDueDate { get; set; }
        
        public RequestStatus Status { get; set; }
        
        public ApprovalLevel CurrentLevel { get; set; }
        
        public string? RejectionReason { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime? UpdatedAt { get; set; }

        public DateTime? ApprovedAt { get; set; }

        public string? RequestNumber { get; set; }

        public string? ApprovedBy { get; set; }

        public DateTime? RejectedAt { get; set; }

        public string? RejectedBy { get; set; }

        public int? UpdatedBy { get; set; }

        public string? DocumentPath { get; set; }

        public string? Notes { get; set; }      
        public User? User { get; set; }
        public ICollection<AdvanceRequestProject>? AdvanceRequestProjects { get; set; }
        public ICollection<ApprovalProcess>? Approvals { get; set; }
        public ICollection<Payment>? Payments { get; set; }
        public ICollection<Notification>? Notifications { get; set; }
        public ICollection<LegalAction>? LegalActions { get; set; }
        public ICollection<Document>? Documents { get; set; }

        public void ValidateDesiredDate()
        {
            if (DesiredDate < DateTime.Today)
            {
                throw new ValidationException("İstenilen tarih bugünden önce olamaz.");
            }
        }

        public void ValidateAmount(decimal minAmount, decimal maxAmount)
        {
            if (Amount < minAmount)
            {
                throw new ValidationException($"Avans tutarı minimum {minAmount:C2} olmalıdır.");
            }
            if (Amount > maxAmount)
            {
                throw new ValidationException($"Avans tutarı maksimum {maxAmount:C2} olmalıdır.");
            }
        }
    }
}