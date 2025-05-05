using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VarlikYönetimi.Core.Enums;

namespace VarlikYönetimi.Core.Entities
{
    public class LegalAction : BaseEntity
    {
        public int Id { get; set; }
        public int AdvanceRequestId { get; set; }
        public AdvanceRequest AdvanceRequest { get; set; }
        public LegalActionStatus Status { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}
