using System;

namespace VarlikYönetimi.Core.Entities
{
    public class SecurityEvent
    {
        public int Id { get; set; }
        public string EventType { get; set; }
        public string Description { get; set; }
        public int? UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string IpAddress { get; set; }
    }
} 