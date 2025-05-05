using System.Collections.Generic;

namespace VarlikYönetimi.Core.ViewModels
{
    public class UserRoleViewModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string CurrentRole { get; set; }
        public List<string> AvailableRoles { get; set; }
    }
} 