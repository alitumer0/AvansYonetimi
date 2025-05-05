using System;

namespace VarlikYönetimi.API.DTOs.ApprovalSettingsDTO
{
    public class ApprovalSettingsDTO
    {
        public int Id { get; set; }
        public string SettingType { get; set; }
        public string SettingValue { get; set; }
        public string Description { get; set; }
        public int UpdatedByUserId { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
} 