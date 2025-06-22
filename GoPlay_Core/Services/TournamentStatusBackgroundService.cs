using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GoPlay_Core.Services
{
    public class TournamentStatusBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<TournamentStatusBackgroundService> _logger;

        public TournamentStatusBackgroundService(IServiceProvider serviceProvider, ILogger<TournamentStatusBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("TournamentStatusBackgroundService iniciado.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var handler = scope.ServiceProvider.GetRequiredService<TournamentStatusHandler>();
                        await handler.UpdateTournamentStatusesAsync();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro no TournamentStatusBackgroundService.");
                }

                // Esperar 24 horas
                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            }
        }
    }
}
