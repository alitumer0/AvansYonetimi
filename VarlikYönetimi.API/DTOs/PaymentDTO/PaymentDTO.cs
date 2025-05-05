using System;

namespace VarlikYönetimi.API.DTOs.PaymentDTO
{
    public class PaymentDTO
    {
        public int Id { get; set; }
        public int AdvanceRequestId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentMethod { get; set; }
        public string Status { get; set; }
        public int EnteredByUserId { get; set; }
        public int? DeliveredByUserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string EnteredByUserName { get; set; }
        public string DeliveredByUserName { get; set; }
    }
} 