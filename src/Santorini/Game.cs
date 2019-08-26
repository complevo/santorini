using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Santorini.Tests")]
namespace Santorini
{
    public class Game
    {
        public Island Island { get; set; }

        private List<Player> _players;
        public IReadOnlyCollection<Player> Players => _players;

        private List<MoveCommand> _movesHistory;
        public IReadOnlyCollection<MoveCommand> MovesHistory => _movesHistory;

        public Player Winner { get; private set; }

        public IEnumerable<Worker> Workers
        {
            get
            {
                foreach (var land in Island.Board)
                    if (land.HasWorker)
                        yield return land.Worker;
            }
        }

        public bool GameIsOver
            => Winner != null;

        public Game()
        {
            Island = new Island();
            _players = new List<Player>();
            _movesHistory = new List<MoveCommand>();
            Winner = default;
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

            var success = worker.TryMoveTo(command.MoveTo.X, command.MoveTo.Y);

            if (success && worker.LandLevel == 3)
            {
                Winner = worker.Player;
                _movesHistory.Add(command);

                return true;
            }

            success = worker.TryBuildAt(command.BuildAt.X, command.BuildAt.Y);

            if (success)            
                _movesHistory.Add(command);

            return success;
        }

        public bool TryUndoCommand(MoveCommand command, Coord from)
        {
            var worker = Island.GetWorker(command.PlayerName, command.WorkerNumber);

            Island.TryGetLand(command.MoveTo.X, command.MoveTo.Y, out Land movedTo);
            Island.TryGetLand(from.X, from.Y, out Land movedFrom);

            Island.TryGetLand(command.BuildAt.X, command.BuildAt.Y, out Land builtAt);

            if (builtAt.LandLevel == 1)
                builtAt.RemovePiece(builtAt.Tower);
            else
                builtAt.Tower.DecreaseLevel();

            movedTo.RemovePiece(worker);
            movedFrom.TryPutPiece(worker);

            return true;

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
    }
}
