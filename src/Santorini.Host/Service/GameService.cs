using System;
using System.Threading.Tasks;
using Flurl.Http.Configuration;

namespace Santorini.Host
{
    public class GameService
    {
        private readonly Match _match;
        private readonly GameSettings _settings;
        private readonly IFlurlClientFactory _clientFactory;

        protected PlayerInstance PlayerOne { get; private set; }
        protected PlayerInstance PlayerTwo { get; private set; }


        public GameService(GameSettings settings, IFlurlClientFactory clientFactory)
        {
            _settings = settings;
            _match = new Match();
            _clientFactory = clientFactory;

            if (!_settings.IsValid)
                throw new SettingsNotValidException(_settings);
        }

        public void RegisterPlayers()
        {
            var bluePlayer = new PlayerInstance(_settings.BluePlayer, _clientFactory);
            var whitePlayer = new PlayerInstance(_settings.WhitePlayer, _clientFactory);

            var bluePlayerOk =_match.TryAddPlayer(_settings.BluePlayer.Name);
            if (!bluePlayerOk) throw new AddPlayerException(_settings.BluePlayer.Name);

            var whitePlayerOk = _match.TryAddPlayer(_settings.WhitePlayer.Name);
            if (!whitePlayerOk) throw new AddPlayerException(_settings.WhitePlayer.Name);

            if (new Random().Next(1) > 0)
            {
                PlayerOne = bluePlayer;
                PlayerTwo = whitePlayer;
            }
            else
            {
                PlayerOne = whitePlayer;
                PlayerTwo = bluePlayer;
            }
        }

        public async Task PlaceWorkers()
        {
            var workersPlaced = false;
            while (!workersPlaced)
            {
                var placeWorkerCmd = await PlayerOne.PlaceWorkersRequestAsync(_match);
                if (!placeWorkerCmd.IsValid) continue;

                var success1 = _match.TryAddWorker(PlayerOne.PlayerName, 1, placeWorkerCmd.WorkerOne.X, placeWorkerCmd.WorkerOne.Y);
                if (!success1) continue;

                var success2 = _match.TryAddWorker(PlayerOne.PlayerName, 2, placeWorkerCmd.WorkerOne.X, placeWorkerCmd.WorkerOne.Y);
                if (!success2) continue;
            }

        }
    }
}
