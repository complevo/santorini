using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Santorini.Tests")]
namespace Santorini
{
    public class Match
    {
        public Island Island { get; }

        private List<Player> _players;
        public IReadOnlyCollection<Player> Players => _players;

        private List<MoveCommand> _movesHistory;
        public IReadOnlyCollection<MoveCommand> MovesHistory => _movesHistory;

        public IEnumerable<Worker> Workers
        {
            get
            {
                foreach (var land in Island.Board)
                    if (land.HasWorker)
                        yield return land.Worker;
            }
        }

        public bool GameIsOver { get; private set; }

        public Match()
        {
            Island = new Island();
            _players = new List<Player>();
            _movesHistory = new List<MoveCommand>();
        }

        public bool TryAddPlayer(string name)
        {
            if (_players.Count >= 2)
                return false;

            if (_players.Any(p => p.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)))
                return false;

            _players.Add(new Player(name));

            return true;
        }

        public bool TryAddWorker(string playerName, int workerNumber, int posX, int posY)
        {
            if (Players.Count != 2) return false;

            var player = Players.SingleOrDefault(p => p.Name.Equals(playerName, StringComparison.InvariantCultureIgnoreCase));
            if (player is null) return false;

            var worker = player.Workers.SingleOrDefault(b => b.Number == workerNumber);
            if (worker is null || worker.IsPlaced) return false;

            var opponentWorkersOnBoard = Workers.Count(b => !b.Player.Name.Equals(playerName, StringComparison.InvariantCultureIgnoreCase));
            if (opponentWorkersOnBoard == 1) return false;

            return Island.TryAddPiece(worker, posX, posY);
        }

        public bool TryMoveWorker(MoveCommand command)
        {
            if (!IsMoveCommandAllowed(command))
                return false;

            var worker = Island.GetWorker(command.PlayerName, command.WorkerNumber);
            if (worker is null) return false;

            var success = worker.TryMoveTo(command.MoveTo.X, command.MoveTo.Y)
                && (command.BuildAt != null && worker.TryBuildAt(command.BuildAt.X, command.BuildAt.Y));

            if (success)            
                _movesHistory.Add(command);

            AssertGameIsOver();

            return success;
        }

        private bool IsMoveCommandAllowed(MoveCommand command)
        {
            // is command a valid one
            if (!command.IsValid) return false;

            // validate the worker owns the player
            var player = Players.SingleOrDefault(p => p.Name == command.PlayerName);
            if (player is null) return false;

            var worker = player.Workers.SingleOrDefault(b => b.Number == command.WorkerNumber);
            if (worker is null) return false;

            // validate if worker can move to destination
            if (Island.TryGetLand(command.MoveTo.X, command.MoveTo.Y, out var moveToLand)
                && moveToLand.IsUnoccupied)
            {
                // validate if worker can build after moving
                if (Island.TryGetLand(command.BuildAt.X, command.BuildAt.Y, out var buildAtLand))
                {
                    return buildAtLand.IsUnoccupied || buildAtLand == worker.CurrentLand;
                }
            }

            return false;
        }

        private void AssertGameIsOver()
        {
            if (GameIsOver) return;

            foreach (var land in Island.Board)
            {
                if (land.HasWorker && land.HasTower && land.Tower.Level == 3)
                {
                    GameIsOver = true;
                    return;
                }
            }
        }
    }
}
