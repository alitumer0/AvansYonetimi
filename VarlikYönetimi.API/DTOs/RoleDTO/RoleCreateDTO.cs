using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VarlikYönetimi.API.DTOs.RoleDTO
{
    public class RoleCreateDTO
    {
        [Required(ErrorMessage = "Rol adı zorunludur.")]
        [StringLength(50, ErrorMessage = "Rol adı en fazla 50 karakter olabilir.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Açıklama zorunludur.")]
        [StringLength(200, ErrorMessage = "Açıklama en fazla 200 karakter olabilir.")]
        public string Description { get; set; }

        public ICollection<string> Permissions { get; set; }
    }
} 