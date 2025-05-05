using System;

namespace VarlikYönetimi.API.DTOs.NotificationDTO
{
    public class NotificationDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int? AdvanceRequestId { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string Type { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ReadAt { get; set; }
        public string UserName { get; set; }
    }
} 