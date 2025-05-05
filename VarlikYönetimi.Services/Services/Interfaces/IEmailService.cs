using System.Threading.Tasks;

namespace VarlikYönetimi.Services.Services.Interfaces
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string to, string subject, string body);
    }
} 