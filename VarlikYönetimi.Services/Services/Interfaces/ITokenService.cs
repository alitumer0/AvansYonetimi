using System.Security.Claims;
using VarlikYönetimi.Core.Entities;

namespace VarlikYönetimi.Services.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(User user);
        ClaimsPrincipal ValidateToken(string token);
        string GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
        Task<bool> RevokeToken(string token);
        Task<bool> IsTokenRevoked(string token);
    }
} 