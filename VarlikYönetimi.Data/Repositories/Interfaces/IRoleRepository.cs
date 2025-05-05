using System.Collections.Generic;
using System.Threading.Tasks;
using VarlikYönetimi.Core.Entities;

namespace VarlikYönetimi.Data.Repositories.Interfaces
{
    public interface IRoleRepository : IGenericRepository<Role>
    {
        Task<Role> GetRoleWithUsersAsync(int id);
        Task<Role> GetRoleByNameAsync(string roleName);
        Task<IEnumerable<Role>> GetRolesByMaxApprovalAmountAsync(decimal amount);
    }
} 