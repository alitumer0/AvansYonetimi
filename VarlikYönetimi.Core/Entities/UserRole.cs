using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VarlikYönetimi.Core.Entities
{
    public class UserRole : IdentityUserRole<int>
    {
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual User User { get; set; }
        public virtual Role Role { get; set; }
    }
}