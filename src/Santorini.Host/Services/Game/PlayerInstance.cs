using System.Threading.Tasks;
using Flurl.Http;
using Flurl.Http.Configuration;
using Microsoft.Extensions.Logging;

namespace Santorini.Host
{
    public class PlayerInstance
    {
        private readonly IFlurlClient _client;
        private readonly PlayerSettings _settings;
        private readonly ILogger _logger;

        public string PlayerName => _settings.Name;

        public PlayerInstance(PlayerSettings settings, IFlurlClientFactory clientFactory, ILogger logger)
        {
            _logger = logger;
            _settings = settings;

            _client = clientFactory.Get(_settings.BaseUrl);
            _client.BaseUrl = _settings.BaseUrl;
            _client.Configure(http =>
            {
                //TODO: The JsonSerializerSettings required is the ConstructorHandling which need to be set to AllowNonPublicDefaultConstructor. 
                http.BeforeCall = call => _logger.LogDebug("Before HTTP {@call}", call);
                http.AfterCall = call => _logger.LogDebug("After HTTP {@call}", call);
                http.OnError = call => _logger.LogError("Error HTTP {@call}", call);
            });
        }

        public Task<PlaceWorkersCommand> PlaceWorkersRequestAsync(Game game)
            => _client
                .Request(_settings.AddWorkerEndpoint)
                .PostJsonAsync(game)
                .ReceiveJson<PlaceWorkersCommand>();


        public Task<MoveCommand> MoveRequest(Game game)
            => _client
                .Request(_settings.MoveEndpoint)
                .PostJsonAsync(game)
                .ReceiveJson<MoveCommand>();

        public Task ReportMatch(Game match)
            => _client
                .Request(_settings.GameReportEndpoint)
                .PostJsonAsync(match);
    }
}
