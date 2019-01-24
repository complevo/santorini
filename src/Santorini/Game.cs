using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Santorini.Tests")]
namespace Santorini
{
    public class Game
    {
        public Board Board { get; }

        private List<Player> _players;
        public Player[] Players => _players.ToArray();

        private List<MoveCommand> _movesHistory;
        public MoveCommand[] MovesHistory => _movesHistory.ToArray();

        public Game()
        {
            Board = new Board();
            _players = new List<Player>();
            _movesHistory = new List<MoveCommand>();
        }

        public void AddPlayer(string name)
        {
            if (_players.Count < 2)
                _players.Add(new Player(name));
        }

        public void AddBuilder(string playerName, int builderNumber, int posX, int posY)
        {
            var player = Players.Single(p => p.Name == playerName);
            var builder = player.Builders.Single(b => b.Number == builderNumber);

            Board.TryAddPiece(builder, posX, posY);
        }

        public bool TryMoveBuilder(MoveCommand command)
        {
            if (IsMoveCommandAllowed(command))
                return false;

            var builder = Board.GetBuilder(command.PlayerName, command.BuilderNumber);

            var success = builder.TryMoveTo(command.MoveTo.X, command.MoveTo.Y)
                && builder.TryBuildAt(command.BuildAt.X, command.BuildAt.Y);

            if (success)            
                _movesHistory.Add(command);
            
            return success;
        }

        private bool IsMoveCommandAllowed(MoveCommand command)
        {
            // is command a valid one
            if (!command.IsValid) return false;

            // validate the builder owns the player            
            var player = Players.SingleOrDefault(p => p.Name == command.PlayerName);
            if (player is null) return false;

            var builder = player.Builders.Single(b => b.Number == command.BuilderNumber);
            if (builder is null) return false;
            
            // validate if builder can move to destination
            var land = Board.Lands[command.MoveTo.X, command.MoveTo.Y];

            return true;
        }
    }
}
