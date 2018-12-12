using System;

namespace Santorini
{
    public class Game
    {
        public Gameboard Board { get; private set; }

        public Game()
        {
            Board = new Gameboard();
        }

        public void AddPlayer(string name)
        {
            Board.AddPlayer(new Player(name));
        }

        public void AddFigure(string playerName, int xPosition, int yPosition)
        {
            if (!RuleChecker.PositionIsPossible(xPosition, yPosition, 0))
                throw new Exception();

            Position position = new Position(xPosition, yPosition, 0);

            Board.GetPlayer(playerName).AddFigure(position);
        }

        public MoveResult TryMove(Move move)
        {
            return RuleChecker.TryMove(Board, move);
        }
    }
}
