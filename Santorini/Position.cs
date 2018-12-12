using System;

namespace Santorini
{
    public class Position : Location, IEquatable<Position>
    {
        public int Z { get; private set; }

        public Position(int x, int y, int z)
            : base(x, y)
        {
            Z = z;
        }

        public Location GetLocation()
        {
            return new Location(X, Y);
        }

        public bool Equals(Position other)
        {
            return base.Equals(other) && Z == other.Z;
        }
    }
}
