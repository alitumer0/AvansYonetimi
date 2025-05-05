using System.Threading.Tasks;
using VarlikYönetimi.Core.Entities;

namespace VarlikYönetimi.Services.Services.Interfaces
{
    public interface ILegalActionService
    {
        Task<LegalAction> CreateLegalActionAsync(LegalAction legalAction);
        Task<LegalAction> GetByIdAsync(int id);
        Task<bool> UpdateAsync(LegalAction legalAction);
        Task<bool> CompleteAsync(int id);
        Task<bool> CancelAsync(int id);
    }
} 