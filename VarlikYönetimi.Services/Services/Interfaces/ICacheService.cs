using System;
using System.Threading.Tasks;

namespace VarlikYönetimi.Services.Services.Interfaces
{
    public interface ICacheService
    {
        Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? slidingExpiration = null);
        void Remove(string key);
        void Clear();
    }
}