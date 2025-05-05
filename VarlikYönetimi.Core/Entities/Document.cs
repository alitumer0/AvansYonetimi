using System;
using System.ComponentModel.DataAnnotations;

namespace VarlikYönetimi.Core.Entities
{
    public class Document
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public long FileSize { get; set; }
        public string ContentType { get; set; }
        public int AdvanceRequestId { get; set; }
        public DateTime CreatedAt { get; set; }

        public AdvanceRequest AdvanceRequest { get; set; }
    }
} 