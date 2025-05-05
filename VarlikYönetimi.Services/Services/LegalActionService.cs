using System;
using System.Threading.Tasks;
using VarlikYönetimi.Core.Entities;
using VarlikYönetimi.Data.Repositories;
using VarlikYönetimi.Data.Repositories.Interfaces;
using VarlikYönetimi.Services.Services.Interfaces;

namespace VarlikYönetimi.Services.Services
{
    public class LegalActionService : GenericService<LegalAction>, ILegalActionService
    {
        public LegalActionService(IGenericRepository<LegalAction> repository) : base(repository)
        {
        }

        public async Task<LegalAction> CreateLegalActionAsync(LegalAction legalAction)
        {
            legalAction.CreatedAt = DateTime.UtcNow;
            await _repository.AddAsync(legalAction);
            return legalAction;
        }

        public async Task<LegalAction> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<bool> UpdateAsync(LegalAction legalAction)
        {
            legalAction.UpdatedAt = DateTime.UtcNow;
            await _repository.UpdateAsync(legalAction);
            return true;
        }

        public async Task<bool> CompleteAsync(int id)
        {
            var legalAction = await _repository.GetByIdAsync(id);
            if (legalAction == null)
                return false;

            legalAction.Status = Core.Enums.LegalActionStatus.Completed;
            legalAction.CompletedAt = DateTime.UtcNow;
            legalAction.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(legalAction);
            return true;
        }

        public async Task<bool> CancelAsync(int id)
        {
            var legalAction = await _repository.GetByIdAsync(id);
            if (legalAction == null)
                return false;

            legalAction.Status = Core.Enums.LegalActionStatus.Cancelled;
            legalAction.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(legalAction);
            return true;
        }
    }
} 