using System;

namespace Santorini
{
    public class Location : IEquatable<Location>
    {
        public int X { get; private set; }

        public int Y { get; private set; }

        public Location(int x, int y)
        {
            X = x;
            Y = y;
        }

        public bool Equals(Location other)
        {
            return X == other.X && Y == other.Y;
        }
    }
}
