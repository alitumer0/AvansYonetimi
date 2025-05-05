using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using VarlikYönetimi.Core.Entities;
using VarlikYönetimi.Data.Context;
using VarlikYönetimi.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace VarlikYönetimi.Services.Services
{
    public class SecurityService : ISecurityService
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly int _minPasswordLength;
        private readonly bool _requireUppercase;
        private readonly bool _requireLowercase;
        private readonly bool _requireNumbers;
        private readonly bool _requireSpecialChars;
        private readonly PasswordHasher<User> _passwordHasher;

        public SecurityService(IConfiguration configuration, AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _minPasswordLength = int.Parse(_configuration["Security:PasswordPolicy:MinLength"]);
            _requireUppercase = bool.Parse(_configuration["Security:PasswordPolicy:RequireUppercase"]);
            _requireLowercase = bool.Parse(_configuration["Security:PasswordPolicy:RequireLowercase"]);
            _requireNumbers = bool.Parse(_configuration["Security:PasswordPolicy:RequireNumbers"]);
            _requireSpecialChars = bool.Parse(_configuration["Security:PasswordPolicy:RequireSpecialChars"]);
            _passwordHasher = new PasswordHasher<User>();
        }

        public string HashPassword(string password)
        {
            var user = new User();
            return _passwordHasher.HashPassword(user, password);
        }

        public bool VerifyPassword(string hashedPassword, string providedPassword)
        {
            var user = new User { PasswordHash = hashedPassword };
            var result = _passwordHasher.VerifyHashedPassword(user, hashedPassword, providedPassword);
            return result != PasswordVerificationResult.Failed;
        }

        public string GenerateSalt()
        {
            var salt = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(salt);
            return Convert.ToBase64String(salt);
        }

        public bool IsPasswordStrong(string password)
        {
            if (string.IsNullOrEmpty(password) || password.Length < _minPasswordLength)
                return false;

            if (_requireUppercase && !Regex.IsMatch(password, "[A-Z]"))
                return false;

            if (_requireLowercase && !Regex.IsMatch(password, "[a-z]"))
                return false;

            if (_requireNumbers && !Regex.IsMatch(password, "[0-9]"))
                return false;

            if (_requireSpecialChars && !Regex.IsMatch(password, "[^a-zA-Z0-9]"))
                return false;

            return true;
        }

        public async Task<bool> ValidatePasswordPolicy(string password)
        {
            if (!IsPasswordStrong(password))
                return false;

            return true;
        }

        public async Task<bool> LogSecurityEvent(string eventType, string description, int? userId = null)
        {
            try
            {
                var securityEvent = new SecurityEvent
                {
                    EventType = eventType,
                    Description = description,
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow,
                    IpAddress = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString()
                };

                await _context.SecurityEvents.AddAsync(securityEvent);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
} 