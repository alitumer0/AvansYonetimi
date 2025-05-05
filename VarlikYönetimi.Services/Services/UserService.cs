using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using VarlikYönetimi.Core.Entities;
using VarlikYönetimi.Data.Repositories;
using VarlikYönetimi.Services.Services.Interfaces;
using VarlikYönetimi.Data.Context;
using Microsoft.EntityFrameworkCore;
using VarlikYönetimi.Core.ViewModels;
using VarlikYönetimi.Services.Interfaces;
using VarlikYönetimi.Core.Models;
using VarlikYönetimi.Core.Services;
using Microsoft.AspNetCore.Identity;
using VarlikYönetimi.Data.Repositories.Interfaces;

namespace VarlikYönetimi.Services.Services
{
    public class UserService : GenericService<User>, IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly AppDbContext _context;
        private readonly ISecurityService _securityService;
        private readonly UserManager<User> _userManager;

        public UserService(IUserRepository userRepository, AppDbContext context, ISecurityService securityService, UserManager<User> userManager) : base(userRepository)
        {
            _userRepository = userRepository;
            _context = context;
            _securityService = securityService;
            _userManager = userManager;
        }

        public async Task<User> GetUserWithRolesAsync(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user != null)
            {
                var roles = await _userManager.GetRolesAsync(user);
                user.RoleNames = roles.ToList();
            }
            return user;
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            return await _userRepository.GetUserByUsernameAsync(username);
        }

        public async Task<IEnumerable<User>> GetUsersByRoleAsync(string roleName)
        {
            return await _userRepository.GetUsersByRoleAsync(roleName);
        }

        public async Task<bool> AddUserRoleAsync(int userId, int roleId)
        {
            return await _context.ExecuteInTransactionAsync(async () =>
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null) return false;

                var userRole = new UserRole
                {
                    UserId = userId,
                    RoleId = roleId,
                    CreatedAt = DateTime.Now
                };

                await _userRepository.AddUserRoleAsync(userId, roleId);
                return true;
            });
        }

        public async Task<bool> RemoveUserRoleAsync(int userId, int roleId)
        {
            return await _context.ExecuteInTransactionAsync(async () =>
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null) return false;

                return await _userRepository.RemoveUserRoleAsync(userId, roleId);
            });
        }

        public async Task<bool> ValidateUserAsync(string username, string password)
        {
            return await _context.ExecuteInTransactionAsync(async () =>
            {
                var user = await _userRepository.GetUserByUsernameAsync(username);
                if (user == null) return false;

                return _securityService.VerifyPassword(user.PasswordHash, password);
            });
        }

        public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user != null)
            {
                var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
                return result.Succeeded;
            }
            return false;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllAsync();
        }

        public async Task<User> AuthenticateAsync(string email, string password)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user == null || !_securityService.VerifyPassword(user.PasswordHash, password))
                return null;

            return user;
        }

        public async Task<ServiceResult> RegisterAsync(RegisterViewModel model)
        {
            if (await _context.Users.AnyAsync(u => u.Email == model.Email))
            {
                return new ServiceResult
                {
                    Success = false,
                    Message = "Bu email adresi zaten kullanılıyor"
                };
            }

            var user = new User
            {
                Email = model.Email,
                PasswordHash = _securityService.HashPassword(model.Password),
                CreatedAt = DateTime.UtcNow
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return new ServiceResult
            {
                Success = true,
                Message = "Kayıt başarılı"
            };
        }

        public async Task<User> GetByIdAsync(int id)
        {
            return await _userManager.FindByIdAsync(id.ToString());
        }

        public async Task<bool> UpdateAsync(User user)
        {
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return false;

            _context.Users.Remove(user);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<List<string>> GetUserRolesAsync(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user != null)
            {
                return (await _userManager.GetRolesAsync(user)).ToList();
            }
            return new List<string>();
        }

        public async Task<int> GetTotalUsersCount()
        {
            return await _repository.CountAsync();
        }
    }
} 