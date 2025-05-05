using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using VarlikYönetimi.Core.Enums;

namespace VarlikYönetimi.Core.Entities
{
    public class ApprovalProcess
    {
        public int Id { get; set; }

        [Required]
        public int AdvanceRequestId { get; set; }

        [Required]
        public int ApproverUserId { get; set; }

        [Required]
        public ApprovalLevel Level { get; set; }

        [Required]
        public ApprovalStatus Status { get; set; }

        public decimal? ApprovedAmount { get; set; }

        [StringLength(500)]
        public string Comments { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public int? UpdatedBy { get; set; }

        public DateTime? ApprovedAt { get; set; }

        public virtual AdvanceRequest AdvanceRequest { get; set; }
        public virtual User ApproverUser { get; set; }
        public ApprovalLevel CurrentLevel { get; set; }

    }
}
