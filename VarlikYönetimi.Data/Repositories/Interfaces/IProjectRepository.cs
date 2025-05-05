using System.Collections.Generic;
using System.Threading.Tasks;
using VarlikYönetimi.Core.Entities;

namespace VarlikYönetimi.Data.Repositories.Interfaces
{
    public interface IProjectRepository : IGenericRepository<Project>
    {
        Task<Project> GetProjectWithAdvanceRequestsAsync(int id);
        Task<IEnumerable<Project>> GetProjectsByUserAsync(int userId);
        Task<IEnumerable<Project>> GetActiveProjectsAsync();
    }
} 