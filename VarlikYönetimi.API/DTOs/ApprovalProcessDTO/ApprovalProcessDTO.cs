using System;

namespace VarlikYönetimi.API.DTOs.ApprovalProcessDTO
{
    public class ApprovalProcessDTO
    {
        public int Id { get; set; }
        public int AdvanceRequestId { get; set; }
        public int ApproverUserId { get; set; }
        public string Status { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string ApproverUserName { get; set; }
    }
} 