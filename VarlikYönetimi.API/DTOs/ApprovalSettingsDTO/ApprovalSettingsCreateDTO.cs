using System.ComponentModel.DataAnnotations;

namespace VarlikYönetimi.API.DTOs.ApprovalSettingsDTO
{
    public class ApprovalSettingsCreateDTO
    {
        [Required(ErrorMessage = "Ayar tipi zorunludur.")]
        [StringLength(50, ErrorMessage = "Ayar tipi en fazla 50 karakter olabilir.")]
        public string SettingType { get; set; }

        [Required(ErrorMessage = "Ayar değeri zorunludur.")]
        [StringLength(500, ErrorMessage = "Ayar değeri en fazla 500 karakter olabilir.")]
        public string SettingValue { get; set; }

        [StringLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Güncelleyen kullanıcı ID'si zorunludur.")]
        public int UpdatedByUserId { get; set; }
    }
} 