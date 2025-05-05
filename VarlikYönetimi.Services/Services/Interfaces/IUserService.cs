using System.Collections.Generic;
using System.Threading.Tasks;
using VarlikYönetimi.Core.Entities;
using VarlikYönetimi.Core.Models;
using VarlikYönetimi.Core.ViewModels;

namespace VarlikYönetimi.Services.Services.Interfaces
{
    public interface IUserService : IGenericService<User>
    {
        Task<User> GetUserWithRolesAsync(int id);
        Task<User> GetUserByUsernameAsync(string username);
        Task<IEnumerable<User>> GetUsersByRoleAsync(string roleName);
        Task<bool> AddUserRoleAsync(int userId, int roleId);
        Task<bool> RemoveUserRoleAsync(int userId, int roleId);
        Task<bool> ValidateUserAsync(string username, string password);
        Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User> AuthenticateAsync(string email, string password);
        Task<ServiceResult> RegisterAsync(RegisterViewModel model);
        Task<User> GetByIdAsync(int id);
        Task<bool> UpdateAsync(User user);
        Task<bool> DeleteAsync(int id);
        Task<List<string>> GetUserRolesAsync(int userId);
        Task<int> GetTotalUsersCount();
    }
}