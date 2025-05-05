using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;
using VarlikYönetimi.Services.Services.Interfaces;

namespace VarlikYönetimi.Services.Services
{
    public class AdvanceRequestTimeoutService : BackgroundService
    {
        private readonly ILogger<AdvanceRequestTimeoutService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(15);

        public AdvanceRequestTimeoutService(
            ILogger<AdvanceRequestTimeoutService> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var advanceRequestService = scope.ServiceProvider.GetRequiredService<IAdvanceRequestService>();
                        await advanceRequestService.CheckTimeoutsAsync();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Avans talebi zaman aşımı kontrolü sırasında bir hata oluştu");
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }
        }
    }
} 