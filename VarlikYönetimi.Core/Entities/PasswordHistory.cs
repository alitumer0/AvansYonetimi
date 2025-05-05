using System;

namespace VarlikYönetimi.Core.Entities
{
    public class PasswordHistory
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string HashedPassword { get; set; }
        public DateTime CreatedAt { get; set; }
    }
} 