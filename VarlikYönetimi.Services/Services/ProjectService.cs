using System.Collections.Generic;
using System.Threading.Tasks;
using VarlikYönetimi.Core.Entities;
using VarlikYönetimi.Data.Repositories;
using VarlikYönetimi.Data.Repositories.Interfaces;
using VarlikYönetimi.Services.Services;
using VarlikYönetimi.Services.Services.Interfaces;

namespace VarlikYönetimi.Core.Services
{
    public class ProjectService : GenericService<Project>, IProjectService
    {
        private readonly IProjectRepository _projectRepository;

        public ProjectService(IProjectRepository projectRepository) : base(projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public async Task<Project> GetProjectWithAdvanceRequestsAsync(int id)
        {
            return await _projectRepository.GetProjectWithAdvanceRequestsAsync(id);
        }

        public async Task<IEnumerable<Project>> GetProjectsByUserAsync(int userId)
        {
            return await _projectRepository.GetProjectsByUserAsync(userId);
        }

        public async Task<IEnumerable<Project>> GetActiveProjectsAsync()
        {
            return await _projectRepository.GetActiveProjectsAsync();
        }
    }
} 