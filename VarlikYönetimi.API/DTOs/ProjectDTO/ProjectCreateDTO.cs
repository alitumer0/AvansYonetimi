using System;
using System.ComponentModel.DataAnnotations;

namespace VarlikYönetimi.API.DTOs.ProjectDTO
{
    public class ProjectCreateDTO
    {
        [Required(ErrorMessage = "Proje adı zorunludur.")]
        [StringLength(100, ErrorMessage = "Proje adı en fazla 100 karakter olabilir.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Açıklama zorunludur.")]
        [StringLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Proje kodu zorunludur.")]
        [StringLength(20, ErrorMessage = "Proje kodu en fazla 20 karakter olabilir.")]
        public string Code { get; set; }
    }
} 