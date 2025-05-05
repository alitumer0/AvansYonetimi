using System;
using System.Net.Mail;
using System.Threading.Tasks;
using VarlikYönetimi.Services.Services.Interfaces;

namespace VarlikYönetimi.Core.Services
{
    public class EmailService : IEmailService
    {
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _smtpUsername;
        private readonly string _smtpPassword;
        private readonly string _fromEmail;

        public EmailService(string smtpServer, int smtpPort, string smtpUsername, string smtpPassword, string fromEmail)
        {
            _smtpServer = smtpServer;
            _smtpPort = smtpPort;
            _smtpUsername = smtpUsername;
            _smtpPassword = smtpPassword;
            _fromEmail = fromEmail;
        }

        public async Task<bool> SendEmailAsync(string to, string subject, string body)
        {
            try
            {
                using (var client = new SmtpClient(_smtpServer, _smtpPort))
                {
                    client.EnableSsl = true;
                    client.Credentials = new System.Net.NetworkCredential(_smtpUsername, _smtpPassword);

                    var message = new MailMessage
                    {
                        From = new MailAddress(_fromEmail),
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = true
                    };
                    message.To.Add(to);

                    await client.SendMailAsync(message);
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
} 