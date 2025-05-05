namespace VarlikYönetimi.Services.Interfaces
{
    public interface IRateLimitService
    {
        Task<bool> IsRequestAllowed(string clientId, string endpoint);
        Task<int> GetRemainingRequests(string clientId, string endpoint);
        Task ResetRateLimit(string clientId, string endpoint);
    }
} 