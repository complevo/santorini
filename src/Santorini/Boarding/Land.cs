using System;
using System.Collections.Generic;
using System.Linq;

namespace Santorini
{
    public class Land : IEquatable<Land>
    {
        public int X { get; }
        public int Y { get; }

        private readonly List<Piece> _pieces;
        public Piece[] Pieces => _pieces.ToArray();

        public Board Board { get; }

        internal Land(Board board, int x, int y)
        {
            if (!Board.IsValidPosition(x, y))
                throw new ArgumentOutOfRangeException();

            if (board is null)
                throw new ArgumentNullException(nameof(board));

            Board = board;
            X = x;
            Y = y;
            _pieces = new List<Piece>();
        }

        public bool Equals(Land other)
        {
            if (other is null) return false;
            return X == other.X && Y == other.Y;
        }

        public bool TryPutPiece(Piece piece)
        {
            if (piece is Building && !IsEmpty)
                return false;

            if (piece is Builder)
            {
                if (HasBuilder) return false;

                var builder = piece as Builder;

                if (piece.CurrentLand != null && LandLevel > builder.LandLevel + 1)
                    return false;
            }

            _pieces.Add(piece);
            piece.SetLand(this);

            return true;
        }

        public bool TryRemoveBuilder(Builder builder)
        {
            if(!HasBuilder) return false;

            if (!Builder.Equals(builder)) return false;

            _pieces.Remove(Builder as Piece);

            return true;
        }

        public bool IsEmpty
            => _pieces.Count == 0;

        public bool HasBuilding
            => _pieces.Any(p => p is Building);

        public Building Building
            => _pieces.SingleOrDefault(p => p is Building) as Building;

        public bool HasBuilder
            => _pieces.Any(p => p is Builder);

        public Builder Builder
            => _pieces.SingleOrDefault(p => p is Builder) as Builder;

        public int LandLevel
            => HasBuilding
            ? Building.Level
            : 0;

        public bool MaxLevelReached
            => HasBuilding && Building.MaxLevelReached;
    }
}
