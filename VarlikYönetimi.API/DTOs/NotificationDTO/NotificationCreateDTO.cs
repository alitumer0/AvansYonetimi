using System;
using System.ComponentModel.DataAnnotations;

namespace VarlikYönetimi.API.DTOs.NotificationDTO
{
    public class NotificationCreateDTO
    {
        [Required(ErrorMessage = "Kullanıcı ID'si zorunludur.")]
        public int UserId { get; set; }

        public int? AdvanceRequestId { get; set; }

        [Required(ErrorMessage = "Başlık zorunludur.")]
        [StringLength(100, ErrorMessage = "Başlık en fazla 100 karakter olabilir.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Mesaj zorunludur.")]
        [StringLength(500, ErrorMessage = "Mesaj en fazla 500 karakter olabilir.")]
        public string Message { get; set; }

        [Required(ErrorMessage = "Bildirim tipi zorunludur.")]
        public string Type { get; set; }
    }
} 