using System;

namespace Santorini
{
    public class MoveCommand
    {
        public Coord MoveTo { get; private set; }
        public Coord BuildAt { get; private set; }

        public string PlayerName { get; private set; }
        public int WorkerNumber { get; private set; }

        public MoveCommand(string playerName, int workerNumber, Coord moveTo, Coord buildAt)
        {
            if (moveTo is null) throw new ArgumentNullException(nameof(moveTo));
            if (buildAt is null) throw new ArgumentNullException(nameof(buildAt));

            PlayerName = playerName;
            WorkerNumber = workerNumber;
            MoveTo = moveTo;
            BuildAt = buildAt;
        }

        public bool IsValid
        {
            get
            {
                if (string.IsNullOrEmpty(PlayerName))
                    return false;

                if (WorkerNumber < 1 || WorkerNumber > 2)
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
}
