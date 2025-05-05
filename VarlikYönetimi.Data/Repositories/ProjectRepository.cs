using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VarlikYönetimi.Core.Entities;
using VarlikYönetimi.Data.Repositories.Interfaces;
using VarlikYönetimi.Data.Context;

namespace VarlikYönetimi.Data.Repositories
{
    public class ProjectRepository : GenericRepository<Project>, IProjectRepository
    {
        public ProjectRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Project> GetProjectWithAdvanceRequestsAsync(int id)
        {
            return await _context.Projects
                .Include(p => p.AdvanceRequestProjects)
                    .ThenInclude(arp => arp.AdvanceRequest)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Project>> GetProjectsByUserAsync(int userId)
        {
            return await _context.Projects
                .Include(p => p.AdvanceRequestProjects)
                    .ThenInclude(arp => arp.AdvanceRequest)
                .Where(p => p.AdvanceRequestProjects.Any(arp => arp.AdvanceRequest.UserId == userId))
                .ToListAsync();
        }

        public async Task<IEnumerable<Project>> GetActiveProjectsAsync()
        {
            return await _context.Projects
                .Include(p => p.AdvanceRequestProjects)
                    .ThenInclude(arp => arp.AdvanceRequest)
                .Where(p => p.Status == "Active")
                .ToListAsync();
        }
    }
} 