using VarlikYönetimi.Core.Entities;

namespace VarlikYönetimi.Services.Interfaces
{
    public interface ISecurityService
    {
        string HashPassword(string password);
        bool VerifyPassword(string hashedPassword, string providedPassword);
        string GenerateSalt();
        bool IsPasswordStrong(string password);
        Task<bool> ValidatePasswordPolicy(string password);
        Task<bool> LogSecurityEvent(string eventType, string description, int? userId = null);
    }
} 