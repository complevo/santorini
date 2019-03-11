using System;
using System.Threading.Tasks;
using Flurl.Http.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Santorini.Host
{
    public class GameService : IGameService
    {
        private readonly Game _game;
        private readonly GameSettings _settings;
        private readonly IFlurlClientFactory _clientFactory;
        private readonly ILogger<GameService> _logger;

        protected PlayerInstance PlayerOne { get; private set; }
        protected PlayerInstance PlayerTwo { get; private set; }

        protected PlayerInstance TurnPlayer { get; private set; }


        public GameService(
            IOptions<GameSettings> settings, 
            IFlurlClientFactory clientFactory,
            ILogger<GameService> logger)
        {
            _settings = settings.Value;

            if (!_settings.IsValid)
                throw new SettingsNotValidException(_settings);

            _game = new Game();
            _clientFactory = clientFactory;
            _logger = logger;
        }

        public void RegisterPlayers()
        {
            var bluePlayer = new PlayerInstance(_settings.BluePlayer, _clientFactory, _logger);
            var whitePlayer = new PlayerInstance(_settings.WhitePlayer, _clientFactory, _logger);

            var bluePlayerOk = _game.TryAddPlayer(_settings.BluePlayer.Name);
            if (!bluePlayerOk) throw new AddPlayerException(_settings.BluePlayer.Name);
            _logger.LogDebug($"BluePlayer {bluePlayer.PlayerName} added.");

            var whitePlayerOk = _game.TryAddPlayer(_settings.WhitePlayer.Name);
            if (!whitePlayerOk) throw new AddPlayerException(_settings.WhitePlayer.Name);
            _logger.LogDebug($"WhitePlayer {whitePlayer.PlayerName} added.");

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
            _logger.LogInformation($"Player {TurnPlayer.PlayerName} will start.");
        }

        public async Task PlaceWorkers()
        {
            await PlacePlayerWorker(PlayerOne);
            await PlacePlayerWorker(PlayerTwo);
        }

        private async Task PlacePlayerWorker(PlayerInstance player)
        {
            _logger.LogDebug($"Player {player.PlayerName} is trying to place its workers.");

            var workersPlaced = false;
            while (!workersPlaced)
            {
                var placeWorkerCmd = await player.PlaceWorkersRequestAsync(_game);
                if (!placeWorkerCmd.IsValid)
                {
                    _logger.LogDebug($"Player {player.PlayerName} sent invalid place worker command.", placeWorkerCmd);
                    continue;
                }

                var success1 = _game.TryAddWorker(player.PlayerName, 1, placeWorkerCmd.WorkerOne.X, placeWorkerCmd.WorkerOne.Y);
                if (!success1)
                {
                    _logger.LogDebug($"Player {player.PlayerName} couldn't place worker 1.", placeWorkerCmd.WorkerOne);
                    continue;
                }

                var success2 = _game.TryAddWorker(player.PlayerName, 2, placeWorkerCmd.WorkerOne.X, placeWorkerCmd.WorkerOne.Y);
                if (!success2)
                {
                    _logger.LogDebug($"Player {player.PlayerName} couldn't place worker 2.", placeWorkerCmd.WorkerTwo);
                    continue;
                }

                workersPlaced = true;
                _logger.LogInformation($"Player {player.PlayerName} placed its workers.", placeWorkerCmd);
            }
        }

        public async Task StartGame()
        {
            _logger.LogInformation("Game started");

            while (!_game.GameIsOver)
            {
                _logger.LogInformation($"Player {TurnPlayer.PlayerName} started its turn.");

                var moveCmd = default(MoveCommand);

                while (true)
                {
                    while (moveCmd is null || !moveCmd.IsValid)
                    {
                        moveCmd = await TurnPlayer.MoveRequest(_game);

                        if (!moveCmd.IsValid)
                            _logger.LogDebug($"Player {TurnPlayer.PlayerName} sent invalid move comand.", moveCmd);
                    }

                    var successMove = _game.TryMoveWorker(moveCmd);
                    if (successMove) break;

                    _logger.LogDebug($"Player {TurnPlayer.PlayerName} couldn't move its workers.", moveCmd);
                }

                _logger.LogInformation($"Player {TurnPlayer.PlayerName} made its move.", moveCmd);

                TurnPlayer = TurnPlayer == PlayerOne
                    ? PlayerTwo
                    : PlayerOne;
            }

            _logger.LogInformation($"Game is over. Player {_game.Winner?.Name} wins");
        }

        public Game GetGameReport()
        {
            if(_game.GameIsOver)
                return _game;

            return null;
        }
    }
}
