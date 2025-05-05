using System;
using System.ComponentModel.DataAnnotations;

namespace VarlikYönetimi.API.DTOs.ApprovalProcessDTO
{
    public class ApprovalProcessUpdateDTO
    {
        [Required(ErrorMessage = "ID zorunludur.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Avans talebi ID'si zorunludur.")]
        public int AdvanceRequestId { get; set; }

        [Required(ErrorMessage = "Onaylayan kullanıcı ID'si zorunludur.")]
        public int ApproverUserId { get; set; }

        [Required(ErrorMessage = "Durum zorunludur.")]
        public string Status { get; set; }

        [StringLength(500, ErrorMessage = "Yorum en fazla 500 karakter olabilir.")]
        public string Comment { get; set; }
    }
} 