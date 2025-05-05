using System;
using System.ComponentModel.DataAnnotations;

namespace VarlikYönetimi.API.DTOs.LegalActionDTO
{
    public class LegalActionCreateDTO
    {
        [Required(ErrorMessage = "Talep ID'si zorunludur.")]
        public int RequestId { get; set; }

        [Required(ErrorMessage = "Açıklama zorunludur.")]
        [StringLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "İşlem tarihi zorunludur.")]
        public DateTime ActionDate { get; set; }

        [Required(ErrorMessage = "Durum zorunludur.")]
        public string Status { get; set; }
    }
} 