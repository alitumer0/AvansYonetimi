using System.ComponentModel.DataAnnotations;
using VarlikYönetimi.Core.Entities;

namespace VarlikYönetimi.Core.ViewModels
{
    public class NotificationDetailsViewModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string Type { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ReadDate { get; set; }
        public int? AdvanceRequestId { get; set; }
        public string AdvanceRequestTitle { get; set; }
    }

    public class NotificationCreateViewModel
    {
        [Required(ErrorMessage = "Kullanıcı zorunludur.")]
        public string UserId { get; set; }

        [Required(ErrorMessage = "Başlık zorunludur.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Mesaj zorunludur.")]
        public string Message { get; set; }

        [Required(ErrorMessage = "Tip zorunludur.")]
        public string Type { get; set; }

        public int? AdvanceRequestId { get; set; }
    }

    public class NotificationEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Kullanıcı zorunludur.")]
        public string UserId { get; set; }

        [Required(ErrorMessage = "Başlık zorunludur.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Mesaj zorunludur.")]
        public string Message { get; set; }

        [Required(ErrorMessage = "Tip zorunludur.")]
        public string Type { get; set; }

        public bool IsRead { get; set; }
        public int? AdvanceRequestId { get; set; }
    }
} 