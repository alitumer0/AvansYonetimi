using System;
using System.Linq;
using System.Threading.Tasks;
using VarlikYönetimi.Core.Entities;
using VarlikYönetimi.Core.Enums;
using VarlikYönetimi.Services.Services.Interfaces;
using VarlikYönetimi.Data.Context;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;

namespace VarlikYönetimi.Core.Services
{
    public class BackgroundJobService : BackgroundService
    {
        private readonly ILogger<BackgroundJobService> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private Timer _timer;

        public BackgroundJobService(ILogger<BackgroundJobService> logger, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("BackgroundJobService başlatıldı.");
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var advanceRequestService = scope.ServiceProvider.GetRequiredService<IAdvanceRequestService>();
                var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
                var paymentService = scope.ServiceProvider.GetRequiredService<IPaymentService>();

                try
                {
                    _logger.LogInformation("Arka plan işleri çalıştırılıyor...");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Arka plan işlerinde hata oluştu.");
                }
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("BackgroundJobService durduruluyor.");
            _timer?.Change(Timeout.Infinite, 0);
            return base.StopAsync(cancellationToken);
        }

        public override void Dispose()
        {
            _timer?.Dispose();
            base.Dispose();
        }
    }
} 