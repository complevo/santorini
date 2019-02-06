using System.Threading.Tasks;
using Flurl.Http;
using Flurl.Http.Configuration;

namespace Santorini.Host
{
    public class PlayerInstance
    {
        private readonly IFlurlClient _client;
        private readonly PlayerSettings _settings;

        public string PlayerName => _settings.Name;

        public PlayerInstance(PlayerSettings settings, IFlurlClientFactory clientFactory)
        {
            _settings = settings;
            _client = clientFactory.Get(_settings.BaseUrl);
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
