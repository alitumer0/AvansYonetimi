using System.Collections.Generic;
using VarlikYönetimi.Core.Entities;
using VarlikYönetimi.Core.Enums;

namespace VarlikYönetimi.Core.ViewModels
{
    public class DepartmentLimitsViewModel
    {
        public List<Department> Departments { get; set; }
        public List<ApprovalLevel> ApprovalLevels { get; set; }
        public List<AdvanceLimit> DepartmentLimits { get; set; }
    }
} 