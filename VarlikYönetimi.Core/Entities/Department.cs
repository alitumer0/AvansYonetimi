using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VarlikYönetimi.Core.Entities
{
    public class Department
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [StringLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir.")]
        public string Description { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public string UpdatedBy { get; set; }      

        public virtual ICollection<User> Users { get; set; } = new List<User>();
        public virtual ICollection<AdvanceLimit> AdvanceLimits { get; set; }
    }
} 