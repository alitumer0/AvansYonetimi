using System.ComponentModel.DataAnnotations;
using VarlikYönetimi.Core.Entities;

namespace VarlikYönetimi.Core.ViewModels
{
    public class LegalActionViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? CompletedDate { get; set; }
    }

    public class LegalActionCreateViewModel
    {
        [Required(ErrorMessage = "Ad zorunludur.")]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage = "Durum zorunludur.")]
        public string Status { get; set; }

        [Required(ErrorMessage = "Aktiflik durumu zorunludur.")]
        public bool IsActive { get; set; }
    }

    public class LegalActionEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ad zorunludur.")]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage = "Durum zorunludur.")]
        public string Status { get; set; }

        [Required(ErrorMessage = "Aktiflik durumu zorunludur.")]
        public bool IsActive { get; set; }
    }
} 