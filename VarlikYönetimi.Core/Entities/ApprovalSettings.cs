using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VarlikYönetimi.Core.Enums;

namespace VarlikYönetimi.Core.Entities
{
    public class ApprovalSettings
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int Level { get; set; }

        [Required]
        public decimal MinAmount { get; set; }

        [Required]
        public decimal MaxAmount { get; set; }

        [Required]
        public int RoleId { get; set; }

        [ForeignKey("RoleId")]
        public Role Role { get; set; }

        public int? UpdatedByUserId { get; set; }

        [ForeignKey("UpdatedByUserId")]
        public User UpdatedByUser { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        [Required]
        [StringLength(50)]
        public string SettingKey { get; set; }

        [Required]
        [StringLength(500)]
        public string SettingValue { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        [Required]
        public ApprovalLevel ApprovalLevel { get; set; }

        [Required]
        public int TimeoutDays { get; set; }
    }
} 