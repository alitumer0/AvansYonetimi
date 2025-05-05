using System;
using System.ComponentModel.DataAnnotations;
using VarlikYönetimi.Core.Enums;

namespace VarlikYönetimi.Core.Entities
{
    public class Notification
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [Required]
        [StringLength(500)]
        public string Message { get; set; }

        [Required]
        public NotificationType Type { get; set; }

        public bool IsRead { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? ReadAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public int? AdvanceRequestId { get; set; }

        public User User { get; set; }
        public AdvanceRequest AdvanceRequest { get; set; }
    }
}
