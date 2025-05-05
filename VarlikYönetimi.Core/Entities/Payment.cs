using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VarlikYönetimi.Core.Entities;
using VarlikYönetimi.Core.Enums;

namespace VarlikYönetimi.Core.Entities
{
    public class Payment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int AdvanceRequestId { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public PaymentStatus Status { get; set; }

        public DateTime? PaymentDate { get; set; }

        public DateTime? DueDate { get; set; }

        [Required]
        public int EnteredByUserId { get; set; }

        public int? DeliveredByUserId { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        [ForeignKey("AdvanceRequestId")]
        public AdvanceRequest AdvanceRequest { get; set; }

        [ForeignKey("EnteredByUserId")]
        public User EnteredByUser { get; set; }

        [ForeignKey("DeliveredByUserId")]
        public User DeliveredByUser { get; set; }

        [StringLength(50, ErrorMessage = "Fiş numarası en fazla 50 karakter olabilir.")]
        public string ReceiptNumber { get; set; }

        public string Notes { get; set; }
        public bool IsOverdue { get; set; }
        public int? OverdueDays { get; set; }
        public decimal? LatePaymentInterest { get; set; }

        public static ValidationResult ValidatePaymentDate(DateTime? paymentDate, ValidationContext context)
        {
            var payment = (Payment)context.ObjectInstance;
            
            if (paymentDate.HasValue)
            {
                if (paymentDate.Value.Date < payment.CreatedAt.Date)
                {
                    return new ValidationResult("Ödeme tarihi, oluşturma tarihinden önce olamaz.");
                }

                if (paymentDate.Value.Date > payment.CreatedAt.Date.AddMonths(3))
                {
                    return new ValidationResult("Ödeme tarihi, oluşturma tarihinden en fazla 3 ay sonra olabilir.");
                }
            }

            return ValidationResult.Success;
        }

        public static ValidationResult ValidateDueDate(DateTime? dueDate, ValidationContext context)
        {
            var payment = (Payment)context.ObjectInstance;
            
            if (dueDate.HasValue)
            {
                if (dueDate.Value.Date < payment.CreatedAt.Date)
                {
                    return new ValidationResult("Vade tarihi, oluşturma tarihinden önce olamaz.");
                }

                if (dueDate.Value.Date > payment.CreatedAt.Date.AddMonths(3))
                {
                    return new ValidationResult("Vade tarihi, oluşturma tarihinden en fazla 3 ay sonra olabilir.");
                }
            }

            return ValidationResult.Success;
        }

        public static ValidationResult ValidateRepaymentDate(DateTime? repaymentDate, ValidationContext context)
        {
            var payment = (Payment)context.ObjectInstance;
            
            if (repaymentDate.HasValue)
            {
                if (repaymentDate.Value.Date < payment.CreatedAt.Date)
                {
                    return new ValidationResult("Ödeme tarihi, oluşturma tarihinden önce olamaz.");
                }

                if (repaymentDate.Value.Date > payment.CreatedAt.Date.AddMonths(3))
                {
                    return new ValidationResult("Ödeme tarihi, oluşturma tarihinden en fazla 3 ay sonra olabilir.");
                }
            }

            return ValidationResult.Success;
        }
    }
}
