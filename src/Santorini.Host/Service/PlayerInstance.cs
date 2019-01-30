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
            _client = clientFactory.Get(_settings.MoveEndpoint);
        }

        public Task<PlaceWorkersCommand> PlaceWorkersRequestAsync(Match match)
            => _client
                .Request(_settings.AddWorkerEndpoint)
                .PostJsonAsync(match)
                .ReceiveJson<PlaceWorkersCommand>();


        public Task<MoveCommand> MoveRequest(Match match)
            => _client
                .Request(_settings.MoveEndpoint)
                .PostJsonAsync(match)
                .ReceiveJson<MoveCommand>();

        public Task ReportMatch(Match match)
            => _client
                .Request(_settings.GameReportEndpoint)
                .PostJsonAsync(match);
    }
}
