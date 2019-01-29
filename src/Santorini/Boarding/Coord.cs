using System;

namespace Santorini
{
    public class Coord : IEquatable<Coord>
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Coord(int x, int y)
        {
            X = x;
            Y = y;
        }

        public bool IsValid
            => Island.IsValidPosition(X, Y);
        
        public bool Equals(Coord obj)
        {
            if (obj is null) return false;
            return X == obj.X && Y == obj.Y;
        }

        public override bool Equals(object obj)
        {
            if (obj is null || obj.GetType() != GetType())
                return false;

            return (obj as Coord).Equals(this);
        }

        public static bool operator ==(Coord coord1, Coord coord2)
        {
            if (coord1 is null && coord2 is null) return true;
            if (coord1 is null || coord2 is null) return false;
            return coord1.X == coord2.X && coord1.Y == coord2.Y;
        }

        public static bool operator !=(Coord coord1, Coord coord2)
            => !(coord1 == coord2);

        public override int GetHashCode()
        {
            var hashCode = -665186484;
            hashCode = hashCode * -1521134295 + X.GetHashCode();
            hashCode = hashCode * -1521134295 + Y.GetHashCode();
            return hashCode;
        }
    }
}
