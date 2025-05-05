using System;

namespace VarlikYönetimi.API.DTOs.LegalActionDTO
{
    public class LegalActionDTO
    {
        public int Id { get; set; }
        public int RequestId { get; set; }
        public string Description { get; set; }
        public DateTime ActionDate { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
} 