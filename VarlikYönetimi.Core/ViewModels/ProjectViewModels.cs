using System.ComponentModel.DataAnnotations;
using VarlikYönetimi.Core.Entities;

namespace VarlikYönetimi.Core.ViewModels
{
    public class ProjectViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public int AdvanceRequestCount { get; set; }
    }

    public class ProjectCreateViewModel
    {
        [Required(ErrorMessage = "Proje adı zorunludur.")]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage = "Aktiflik durumu zorunludur.")]
        public bool IsActive { get; set; }
    }

    public class ProjectEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Proje adı zorunludur.")]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage = "Aktiflik durumu zorunludur.")]
        public bool IsActive { get; set; }
    }
} 