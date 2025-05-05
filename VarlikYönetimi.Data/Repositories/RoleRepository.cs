using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VarlikYönetimi.Core.Entities;
using VarlikYönetimi.Data.Repositories.Interfaces;
using VarlikYönetimi.Data.Context;

namespace VarlikYönetimi.Data.Repositories
{
    public class RoleRepository : GenericRepository<Role>, IRoleRepository
    {
        public RoleRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Role> GetRoleWithUsersAsync(int id)
        {
            return await _context.Roles
                .Include(r => r.UserRoles)
                    .ThenInclude(ur => ur.User)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<Role> GetRoleByNameAsync(string roleName)
        {
            return await _context.Roles
                .Include(r => r.UserRoles)
                    .ThenInclude(ur => ur.User)
                .FirstOrDefaultAsync(r => r.Name == roleName);
        }

        public async Task<IEnumerable<Role>> GetRolesByMaxApprovalAmountAsync(decimal amount)
        {
            return await _context.Roles
                .Where(r => r.MaxApprovalAmount >= amount)
                .OrderByDescending(r => r.MaxApprovalAmount)
                .ToListAsync();
        }

        Task<Role> IRoleRepository.GetRoleWithUsersAsync(int id)
        {
            throw new NotImplementedException();
        }

        Task<Role> IRoleRepository.GetRoleByNameAsync(string roleName)
        {
            throw new NotImplementedException();
        }

        Task<IEnumerable<Role>> IRoleRepository.GetRolesByMaxApprovalAmountAsync(decimal amount)
        {
            throw new NotImplementedException();
        }
    }
} 