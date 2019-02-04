using System;
using System.Threading.Tasks;
using Flurl.Http.Configuration;

namespace Santorini.Host
{
    public class GameService
    {
        private readonly Game _game;
        private readonly GameSettings _settings;
        private readonly IFlurlClientFactory _clientFactory;

        protected PlayerInstance PlayerOne { get; private set; }
        protected PlayerInstance PlayerTwo { get; private set; }

        protected PlayerInstance TurnPlayer { get; private set; }


        public GameService(GameSettings settings, IFlurlClientFactory clientFactory)
        {
            _settings = settings;
            _game = new Game();
            _clientFactory = clientFactory;

            if (!_settings.IsValid)
                throw new SettingsNotValidException(_settings);
        }

        public void RegisterPlayers()
        {
            var bluePlayer = new PlayerInstance(_settings.BluePlayer, _clientFactory);
            var whitePlayer = new PlayerInstance(_settings.WhitePlayer, _clientFactory);

            var bluePlayerOk = _game.TryAddPlayer(_settings.BluePlayer.Name);
            if (!bluePlayerOk) throw new AddPlayerException(_settings.BluePlayer.Name);

            var whitePlayerOk = _game.TryAddPlayer(_settings.WhitePlayer.Name);
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

            TurnPlayer = PlayerOne;
        }

        public async Task PlaceWorkers()
        {
            await PlacePlayerWorker(PlayerOne);
            await PlacePlayerWorker(PlayerTwo);
        }

        private async Task PlacePlayerWorker(PlayerInstance player)
        {
            var workersPlaced = false;
            while (!workersPlaced)
            {
                var placeWorkerCmd = await player.PlaceWorkersRequestAsync(_game);
                if (!placeWorkerCmd.IsValid) continue;

                var success1 = _game.TryAddWorker(player.PlayerName, 1, placeWorkerCmd.WorkerOne.X, placeWorkerCmd.WorkerOne.Y);
                if (!success1) continue;

                var success2 = _game.TryAddWorker(player.PlayerName, 2, placeWorkerCmd.WorkerOne.X, placeWorkerCmd.WorkerOne.Y);
                if (!success2) continue;

                workersPlaced = true;
            }
        }

        public async Task StartGame()
        {
            while (!_game.GameIsOver)
            {
                var moveCmd = default(MoveCommand);

                while (true)
                {
                    while (moveCmd is null || !moveCmd.IsValid)
                        moveCmd = await TurnPlayer.MoveRequest(_game);

                    if (_game.TryMoveWorker(moveCmd)) break;
                }

                TurnPlayer = TurnPlayer == PlayerOne
                    ? PlayerTwo
                    : PlayerOne;
            }
        }
    }
}
