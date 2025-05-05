using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VarlikYönetimi.Core.Entities
{
    public class AdvanceRequestProject
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int AdvanceRequestId { get; set; }

        [Required]
        public int ProjectId { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [ForeignKey("AdvanceRequestId")]
        public AdvanceRequest AdvanceRequest { get; set; }

        [ForeignKey("ProjectId")]
        public Project Project { get; set; }
    }
}
