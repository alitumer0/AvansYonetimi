using System.Collections.Generic;
using System.Threading.Tasks;
using VarlikYönetimi.Core.Entities;

namespace VarlikYönetimi.Data.Repositories.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User> GetUserWithRolesAsync(int id);
        Task<User> GetUserByUsernameAsync(string username);
        Task<IEnumerable<User>> GetUsersByRoleAsync(string roleName);
        Task<bool> AddUserRoleAsync(int userId, int roleId);
        Task<bool> RemoveUserRoleAsync(int userId, int roleId);
    }
} 