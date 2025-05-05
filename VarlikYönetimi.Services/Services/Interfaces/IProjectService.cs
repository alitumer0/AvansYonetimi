using System.Collections.Generic;
using System.Threading.Tasks;
using VarlikYönetimi.Core.Entities;

namespace VarlikYönetimi.Services.Services.Interfaces
{
    public interface IProjectService : IGenericService<Project>
    {
        Task<Project> GetProjectWithAdvanceRequestsAsync(int id);
        Task<IEnumerable<Project>> GetProjectsByUserAsync(int userId);
        Task<IEnumerable<Project>> GetActiveProjectsAsync();
    }
} 