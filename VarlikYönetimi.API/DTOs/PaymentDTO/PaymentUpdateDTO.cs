using System;
using System.ComponentModel.DataAnnotations;

namespace VarlikYönetimi.API.DTOs.PaymentDTO
{
    public class PaymentUpdateDTO
    {
        [Required(ErrorMessage = "ID zorunludur.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Avans talebi ID'si zorunludur.")]
        public int AdvanceRequestId { get; set; }

        [Required(ErrorMessage = "Ödeme tutarı zorunludur.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Ödeme tutarı 0'dan büyük olmalıdır.")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Açıklama zorunludur.")]
        [StringLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Ödeme tarihi zorunludur.")]
        public DateTime PaymentDate { get; set; }

        [Required(ErrorMessage = "Ödeme yöntemi zorunludur.")]
        public string PaymentMethod { get; set; }

        [Required(ErrorMessage = "Durum zorunludur.")]
        public string Status { get; set; }

        public int? DeliveredByUserId { get; set; }
    }
} 