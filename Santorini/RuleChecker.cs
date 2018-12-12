using System.Linq;

namespace Santorini
{
    public class RuleChecker
    {
        public static MoveResult TryMove(Gameboard board, Move move)
        {
            bool moveIsPossible = MoveIsPossible(board, move);
            if (!moveIsPossible)
                return MoveResult.StepError;

            bool hasWon = HasWon(board, move);

            if (hasWon)
                return MoveResult.Won;

            bool buildIsPossible = BuildIsPossible(board, move);

            if (buildIsPossible)
            {
                DoMove(board, move);
                return MoveResult.Success;
            }
            return MoveResult.BuildError;
        }

        private static bool HasWon(Gameboard board, Move move)
        {
            return board.Buildings[move.NewLocation.X][move.NewLocation.Y] == 3;
        }

        private static void DoMove(Gameboard board, Move move)
        {
            int newPosZ = board.Buildings[move.NewLocation.X][move.NewLocation.Y];
            Position position = new Position(move.NewLocation.X, move.NewLocation.Y, newPosZ);
            board.GetPlayer(move.PlayerName).Figuren[move.FigureNubmer].ChangePosition(position);
            board.Buildings[move.BuildLocation.X][move.BuildLocation.Y]++;
        }

        public static bool BuildIsPossible(Gameboard board, Move move)
        {
            if (!PositionIsPossible(move.BuildLocation.X, move.BuildLocation.Y, 0) ||
                move.BuildLocation.Equals(move.NewLocation))
                return false;
            int defX = move.BuildLocation.X - move.NewLocation.X;
            int defY = move.BuildLocation.Y - move.NewLocation.Y;
            if (defX < -1 || defX > 1 || defY < -1 || defY > 1)
                return false;
            if (board.Buildings[move.BuildLocation.X][move.BuildLocation.Y] == 4)
                return false;
            Figure currentFigure = board.GetPlayer(move.PlayerName).Figuren[move.FigureNubmer];
            foreach (Player player in board.Players)
                foreach (Figure figure in player.Figuren.Where(f => f != currentFigure))
                    if (move.BuildLocation.Equals(figure.CurrentPosition.GetLocation()))
                        return false;
            return true;
        }

        public static bool MoveIsPossible(Gameboard board, Move move)
        {
            Position oldPossition = board.GetPlayer(move.PlayerName).Figuren[move.FigureNubmer].CurrentPosition;
            int newPosZ = board.Buildings[move.NewLocation.X][move.NewLocation.Y];
            Position newPosition = new Position(move.NewLocation.X, move.NewLocation.Y, newPosZ);
            return StepToPositionIsAllowed(oldPossition, newPosition, board);
        }

        private static bool StepToPositionIsAllowed(Position oldPosition, Position newPosition, Gameboard board)
        {
            if (!PositionIsPossible(newPosition.X, newPosition.Y, newPosition.Z))
                return false;
            foreach (Player player in board.Players)
                foreach (Figure figure in player.Figuren)
                    if (newPosition.GetLocation().Equals(figure.CurrentPosition.GetLocation()))
                        return false;
            int defX = oldPosition.X - newPosition.X;
            int defY = oldPosition.Y - newPosition.Y;
            int defZ = oldPosition.Z - board.Buildings[newPosition.X][newPosition.Y];
            return defX >= -1 && defX <= 1 && defY >= -1 && defY <= 1 && defZ >= -1;
        }

        public static bool PositionIsPossible(int x, int y, int z)
        {
            return x >= 0 && x <= 4 &&
                y >= 0 && y <= 4 &&
                z >= 0 && z <= 3;
        }
    }
}
