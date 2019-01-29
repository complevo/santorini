using System;

namespace Santorini
{
    public class Worker : Piece, IEquatable<Worker>
    {
        public Player Player { get; }
        public int Number { get; set; }        

        internal Worker(Player player, int number)
        {
            if (player is null) throw new ArgumentNullException(nameof(player));
            if (number <= 0) throw new ArgumentOutOfRangeException(nameof(number), "Worker number must be equal or greater than 1");

            Player = player;
            Number = number;
        }

        public bool TryMoveTo(int posX, int posY)
        {
            if (CanMoveTo(posX, posY, out var land))
            {
                CurrentLand.TryRemoveWorker(this);
                return land.TryPutPiece(this);
            }
            return false;
        }

        public bool TryBuildAt(int posX, int posY)
        {
            if (CanBuildAt(posX, posY, out var land))
            {
                if (land.HasTower && !land.Tower.IsComplete)
                {
                    land.Tower.RaiseLevel();

                    return true;
                }

                var newBuilding = new Tower();
                return land.TryPutPiece(newBuilding);
            }

            return false;
        }

        public int LandLevel
            => CurrentLand?.LandLevel ?? -1;

        private bool CanMoveTo(int posX, int posY, out Land land)
        {
            land = default(Land);

            if (CurrentLand is null) return false;

            if (CurrentLand.Island.TryGetLand(posX, posY, out land))
            {
                var from = CurrentLand;
                var to = land;

                if (IsLandBlocked(land)) return false;

                if (IsMovingMoreThan2StepsAway(from, to))
                    return false;

                if (IsSteppingUp(from, to) && IsClimbingMoreThen1LevelUp(from, to))
                    return false;

                return true;
            }

            return false;
        }

        private bool IsClimbingMoreThen1LevelUp(Land from, Land to)
        {
            var levelDiff = to.LandLevel - from.LandLevel;
            return levelDiff > 1;
        }

        private bool IsSteppingUp(Land from, Land to)
            => to.LandLevel > from.LandLevel;

        private bool IsMovingMoreThan2StepsAway(Land from, Land to)
        {
            var posXdiff = Math.Abs(from.Coord.X - to.Coord.X);
            var posYdiff = Math.Abs(from.Coord.Y - to.Coord.Y);

            if (posXdiff > 1 || posYdiff > 1) return true;
            return false;
        }

        private bool IsLandBlocked(Land land)
            => CurrentLand.Equals(land) 
            || land.HasWorker 
            || land.MaxLevelReached;

        private bool CanBuildAt(int posX, int posY, out Land land)
        {
            if (CurrentLand.Island.TryGetLand(posX, posY, out land))
            {
                if (IsLandBlocked(land)) return false;

                var (posXDiff, posYDiff) = (Math.Abs(CurrentLand.Coord.X - land.Coord.X), Math.Abs(CurrentLand.Coord.Y - land.Coord.Y));
                if (posXDiff > 1 || posYDiff > 1) return false;

                return true;
            }

            return false;
        }
               
        public bool Equals(Worker other)
        {
            if (other is null) return false;

            return Player.Equals(other.Player)
                && Number == other.Number;
        }
    }
}
