using System;

namespace Santorini
{
    public class MoveCommand
    {
        public Coord MoveTo { get; private set; }
        public Coord BuildAt { get; private set; }

        public string PlayerName { get; private set; }
        public int BuilderNumber { get; private set; }

        public MoveCommand(string playerName, int builderNumber, Coord moveTo, Coord buildAt)
        {
            if (moveTo is null) throw new ArgumentNullException(nameof(moveTo));
            if (buildAt is null) throw new ArgumentNullException(nameof(buildAt));

            PlayerName = playerName;
            BuilderNumber = builderNumber;
            MoveTo = moveTo;
            BuildAt = buildAt;
        }

        public bool IsValid
        {
            get
            {
                if (string.IsNullOrEmpty(PlayerName))
                    return false;

                if (BuilderNumber < 1 || BuilderNumber > 2)
                    return false;

                if (MoveTo.Equals(BuildAt))
                    return false;

                if (!MoveTo.IsValid)
                    return false;

                if (!BuildAt.IsValid)
                    return false;


                return true;
            }
        }
    }

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
            => Board.IsValidPosition(X, Y);

        public bool Equals(Coord other)
        {
            if (other is null) return false;
            return X == other.X && Y == other.Y;
        }
    }
}
