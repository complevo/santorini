using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Santorini.Host
{
    internal class SantoriniGameHostedService : IHostedService
    {
        private readonly ILogger _logger;
        private readonly IGameService _gameService;

        public SantoriniGameHostedService(
            ILogger<SantoriniGameHostedService> logger,
            IGameService gameService)
        {
            _logger = logger;
            _gameService = gameService;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Registering players");
            _gameService.RegisterPlayers();

            _logger.LogInformation("Placing workers");
            await _gameService.PlaceWorkers();

            _logger.LogInformation("Ready? Steady! Go!");
            await _gameService.StartGame();

            _logger.LogInformation("Game is over!", _gameService.GetGameReport());
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private void OnStarted()
        {
            _logger.LogInformation("OnStarted has been called.");

            // Perform post-startup activities here

            
        }

        private void OnStopping()
        {
            _logger.LogInformation("OnStopping has been called.");

            // Perform on-stopping activities here
        }

        private void OnStopped()
        {
            _logger.LogInformation("OnStopped has been called.");

            // Perform post-stopped activities here
        }
    }
}
