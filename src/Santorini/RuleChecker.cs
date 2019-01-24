//using System.Linq;

//namespace Santorini
//{
//    public class RuleChecker
//    {
//        public static bool TryMove(Board board, MoveCommand move)
//        {
//            var oldPossition = board.GetPlayer(move.PlayerName).Figuren[move.BuilderNumber].CurrentPosition;
//            var newPosZ = board.Buildings[move.NewLocation.X][move.NewLocation.Y];
//            var newPosition = new Land(move.NewLocation.X, move.NewLocation.Y, newPosZ);

//            return StepToPositionIsAllowed(oldPossition, newPosition, board);

//        }

//        public static bool TryBuild(Board board, MoveCommand move)
//        {
//            var buildIsPossible = BuildIsPossible(board, move);

//            if (buildIsPossible)
//            {
//                DoMove(board, move);
//                return true;
//            }
//            return false;
//        }

//        private static bool HasWon(Board board, MoveCommand move)
//        {
//            return board.Buildings[move.NewLocation.X][move.NewLocation.Y] == 3;
//        }

//        private static void DoMove(Board board, MoveCommand move)
//        {
//            int newPosZ = board.Buildings[move.NewLocation.X][move.NewLocation.Y];
//            Land position = new Land(move.NewLocation.X, move.NewLocation.Y, newPosZ);
//            board.GetPlayer(move.PlayerName).Figuren[move.BuilderNumber].ChangePosition(position);
//            board.Buildings[move.BuildAt.X][move.BuildAt.Y]++;
//        }

//        public static bool BuildIsPossible(Board board, MoveCommand move)
//        {
//            if (!IsValidPosition(move.BuildAt.X, move.BuildAt.Y, 0) ||
//                move.BuildAt.Equals(move.NewLocation))
//            {
//                return false;
//            }

//            int difX = move.BuildAt.X - move.NewLocation.X;
//            int difY = move.BuildAt.Y - move.NewLocation.Y;
//            if (difX < -1 || difX > 1 || difY < -1 || difY > 1)
//            {
//                return false;
//            }

//            if (board.Buildings[move.BuildAt.X][move.BuildAt.Y] == 4)
//            {
//                return false;
//            }

//            Builder currentFigure = board.GetPlayer(move.PlayerName).Figuren[move.BuilderNumber];
//            foreach (Player player in board.Players)
//            {
//                foreach (Builder figure in player.Figuren.Where(f => f != currentFigure))
//                {
//                    if (move.BuildAt.Equals(figure.CurrentPosition.GetLocation()))
//                    {
//                        return false;
//                    }
//                }
//            }

//            return true;
//        }

//        private static bool StepToPositionIsAllowed(Land oldPosition, Land newPosition, Board board)
//        {
//            if (!PositionIsPossible(newPosition.X, newPosition.Y, newPosition.Z))
//            {
//                return false;
//            }

//            foreach (var player in board.Players)
//            {
//                foreach (var figure in player.Figuren)
//                {
//                    if (newPosition.GetLocation().Equals(figure.CurrentPosition.GetLocation()))
//                    {
//                        return false;
//                    }
//                }
//            }

//            int difX = oldPosition.X - newPosition.X;
//            int difY = oldPosition.Y - newPosition.Y;
//            int difZ = oldPosition.Z - board.Buildings[newPosition.X][newPosition.Y];
//            return difX >= -1 && difX <= 1 && difY >= -1 && difY <= 1 && difZ >= -1;
//        }

//        public static bool IsValidPosition(int x, int y)
//        {
//            return x >= 0 && x <= 4 &&
//                y >= 0 && y <= 4;
//        }
//    }
//}
