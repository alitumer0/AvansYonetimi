using System;
using System.Collections.Generic;

namespace VarlikYönetimi.API.DTOs.RoleDTO
{
    public class RoleDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public ICollection<string> Permissions { get; set; }
    }
} 