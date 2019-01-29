using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Santorini
{
    [DebuggerDisplay("[{Coord.X},{Coord.Y}], IsUnoccupied:{IsUnoccupied}, HasTower:{HasTower}, HasWorker:{HasWorker}")]
    public class Land : IEquatable<Land>
    {
        public readonly Coord Coord;

        private readonly List<Piece> _pieces;
        public Piece[] Pieces => _pieces.ToArray();

        public Island Island { get; }

        internal Land(Island island, int x, int y)
        {
            if (!Island.IsValidPosition(x, y))
                throw new ArgumentOutOfRangeException();

            if (island is null)
                throw new ArgumentNullException(nameof(island));

            Island = island;
            Coord = new Coord(x, y);
            _pieces = new List<Piece>();
        }

        public bool TryPutPiece(Piece piece)
        {
            if (piece is Tower && (HasTower || HasWorker))
                return false;

            if (piece is Worker)
            {
                if (HasWorker) return false;

                var worker = piece as Worker;

                if (worker.CurrentLand != null && LandLevel > worker.LandLevel + 1)
                    return false;
            }

            _pieces.Add(piece);
            piece.SetLand(this);

            return true;
        }

        public bool TryRemoveWorker(Worker worker)
        {
            if(!HasWorker) return false;

            if (!Worker.Equals(worker)) return false;

            _pieces.Remove(Worker as Piece);

            return true;
        }
        
        public bool IsUnoccupied
            => !HasWorker && !MaxLevelReached;

        public bool HasTower
            => _pieces.Any(p => p is Tower);

        public Tower Tower
            => _pieces.SingleOrDefault(p => p is Tower) as Tower;

        public bool HasWorker
            => _pieces.Any(p => p is Worker);

        public Worker Worker
            => _pieces.SingleOrDefault(p => p is Worker) as Worker;

        public int LandLevel
            => HasTower
            ? Tower.Level
            : 0;

        public bool MaxLevelReached
            => HasTower && Tower.IsComplete;

        public bool Equals(Land other)
        {
            if (other is null) return false;
            return Coord.X == other.Coord.X && Coord.Y == other.Coord.Y;
        }

        public override bool Equals(object obj)
        {
            if (obj is null || obj.GetType() != GetType())
                return false;

            return (obj as Land).Equals(this);
        }

        public static bool operator ==(Land land1, Land land2)
        {
            if (land1 is null && land2 is null) return true;
            if (land1 is null || land2 is null) return false;
            return land1.Coord == land2.Coord;
        }

        public static bool operator !=(Land land1, Land land2)
            => !(land1 == land2);

        public override int GetHashCode()
            => Coord.GetHashCode();
    }
}
