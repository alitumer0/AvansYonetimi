using System.ComponentModel.DataAnnotations;
using VarlikYönetimi.Core.Entities;

namespace VarlikYönetimi.Core.ViewModels
{
    public class ApprovalSettingsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int ApprovalLevel { get; set; }
        public decimal MinAmount { get; set; }
        public decimal MaxAmount { get; set; }
        public bool IsActive { get; set; }
        public string UpdatedByUserId { get; set; }
        public string UpdatedByUserName { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }

    public class ApprovalSettingsCreateViewModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int ApprovalLevel { get; set; }
        public decimal MinAmount { get; set; }
        public decimal MaxAmount { get; set; }
        public bool IsActive { get; set; }
        public string UpdatedByUserId { get; set; }
    }

    public class ApprovalSettingsEditViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int ApprovalLevel { get; set; }
        public decimal MinAmount { get; set; }
        public decimal MaxAmount { get; set; }
        public bool IsActive { get; set; }
        public string UpdatedByUserId { get; set; }
    }
} 