using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VarlikYönetimi.Core.Entities;
using VarlikYönetimi.Core.Enums;

namespace VarlikYönetimi.Core.ViewModels
{
    public class RoleViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public int UserCount { get; set; }
    }

    public class RoleCreateViewModel
    {
        [Required(ErrorMessage = "Rol adı zorunludur.")]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage = "Aktiflik durumu zorunludur.")]
        public bool IsActive { get; set; }
    }

    public class RoleEditViewModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "Rol adı zorunludur.")]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage = "Aktiflik durumu zorunludur.")]
        public bool IsActive { get; set; }

        public List<ApprovalLevelViewModel> ApprovalLevels { get; set; }
    }
} 