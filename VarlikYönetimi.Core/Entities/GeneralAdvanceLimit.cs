using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VarlikYönetimi.Core.Entities
{
    public class GeneralAdvanceLimit
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Minimum tutar zorunludur.")]
        [Range(0, double.MaxValue, ErrorMessage = "Minimum tutar 0'dan büyük olmalıdır.")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal MinAmount { get; set; }

        [Required(ErrorMessage = "Maksimum tutar zorunludur.")]
        [Range(0, double.MaxValue, ErrorMessage = "Maksimum tutar 0'dan büyük olmalıdır.")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal MaxAmount { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? CreatedBy { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public string? UpdatedBy { get; set; }
        public bool IsActive { get; set; } = true;
    }
} 