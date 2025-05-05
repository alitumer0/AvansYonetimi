using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VarlikYönetimi.Core.Entities;
using VarlikYönetimi.Data.Repositories;
using VarlikYönetimi.Services.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using VarlikYönetimi.Data.Repositories.Interfaces;

namespace VarlikYönetimi.Services.Services
{
    public class RoleService : GenericService<Role>, IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly RoleManager<Role> _roleManager;

        public RoleService(IRoleRepository roleRepository, RoleManager<Role> roleManager) : base(roleRepository)
        {
            _roleRepository = roleRepository;
            _roleManager = roleManager;
        }

        public async Task<Role> GetRoleWithUsersAsync(int id)
        {
            return await _roleRepository.GetRoleWithUsersAsync(id);
        }

        public async Task<Role> GetRoleByNameAsync(string roleName)
        {
            return await _roleRepository.GetRoleByNameAsync(roleName);
        }

        public async Task<IEnumerable<Role>> GetRolesByMaxApprovalAmountAsync(decimal amount)
        {
            return await _roleRepository.GetRolesByMaxApprovalAmountAsync(amount);
        }

        public override async Task<Role> GetByIdAsync(int id)
        {
            return await _roleManager.FindByIdAsync(id.ToString());
        }

        public async Task<IEnumerable<Role>> GetAllRolesAsync()
        {
            return _roleManager.Roles.ToList();
        }

        public async Task<bool> CreateAsync(Role role)
        {
            var result = await _roleManager.CreateAsync(role);
            return result.Succeeded;
        }

        public async Task<bool> UpdateAsync(Role role)
        {
            var result = await _roleManager.UpdateAsync(role);
            return result.Succeeded;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var role = await _roleManager.FindByIdAsync(id.ToString());
            if (role != null)
            {
                var result = await _roleManager.DeleteAsync(role);
                return result.Succeeded;
            }
            return false;
        }
    }
} 