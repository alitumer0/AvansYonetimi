using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VarlikYönetimi.Core.Entities;
using System.ComponentModel.DataAnnotations;
using VarlikYönetimi.Core.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace VarlikYönetimi.Core.Entities
{
    public class User : IdentityUser<int>
    {
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }

        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";

        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        public bool IsActive { get; set; } = true;

        public int? DepartmentId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public virtual ICollection<AdvanceRequest> AdvanceRequests { get; set; } = new List<AdvanceRequest>();
        public virtual ICollection<ApprovalProcess> ApprovalProcesses { get; set; } = new List<ApprovalProcess>();
        public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
        public virtual ICollection<LegalAction> LegalActions { get; set; } = new List<LegalAction>();

        [NotMapped]
        public List<string> RoleNames { get; set; } = new List<string>();
    }
}
