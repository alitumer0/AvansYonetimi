using System.ComponentModel.DataAnnotations;
using VarlikYönetimi.Core.Entities;

namespace VarlikYönetimi.Core.ViewModels
{
    public class PaymentViewModelDuplicate
    {
        public int Id { get; set; }
        public int AdvanceRequestId { get; set; }
        public string AdvanceRequestTitle { get; set; }
        public decimal Amount { get; set; }
        public string PaymentType { get; set; }
        public string Status { get; set; }
        public string EnteredByUserId { get; set; }
        public string EnteredByUserName { get; set; }
        public string DeliveredByUserId { get; set; }
        public string DeliveredByUserName { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? DeliveredDate { get; set; }
    }

    public class PaymentCreateViewModel
    {
        [Required(ErrorMessage = "Avans talebi zorunludur.")]
        public int AdvanceRequestId { get; set; }

        [Required(ErrorMessage = "Ödeme tutarı zorunludur.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Ödeme tutarı 0'dan büyük olmalıdır.")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Ödeme tipi zorunludur.")]
        public string PaymentType { get; set; }

        [Required(ErrorMessage = "Durum zorunludur.")]
        public string Status { get; set; }

        [Required(ErrorMessage = "Giren kullanıcı zorunludur.")]
        public string EnteredByUserId { get; set; }

        public string DeliveredByUserId { get; set; }
    }

    public class PaymentEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Avans talebi zorunludur.")]
        public int AdvanceRequestId { get; set; }

        [Required(ErrorMessage = "Ödeme tutarı zorunludur.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Ödeme tutarı 0'dan büyük olmalıdır.")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Ödeme tipi zorunludur.")]
        public string PaymentType { get; set; }

        [Required(ErrorMessage = "Durum zorunludur.")]
        public string Status { get; set; }

        [Required(ErrorMessage = "Giren kullanıcı zorunludur.")]
        public string EnteredByUserId { get; set; }

        public string DeliveredByUserId { get; set; }
    }
} 