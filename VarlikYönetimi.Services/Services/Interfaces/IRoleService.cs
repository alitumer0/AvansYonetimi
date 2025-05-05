using System.Collections.Generic;
using System.Threading.Tasks;
using VarlikYönetimi.Core.Entities;

namespace VarlikYönetimi.Services.Services.Interfaces
{
    public interface IRoleService : IGenericService<Role>
    {
        Task<Role> GetRoleWithUsersAsync(int id);
        Task<Role> GetRoleByNameAsync(string roleName);
        Task<IEnumerable<Role>> GetRolesByMaxApprovalAmountAsync(decimal amount);
        Task<IEnumerable<Role>> GetAllRolesAsync();
        Task<bool> CreateAsync(Role role);
        Task<bool> UpdateAsync(Role role);
        Task<bool> DeleteAsync(int id);
    }
} 