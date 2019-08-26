namespace Santorini
{
    public class Tower : Piece
    {
        public int Level { get; private set; }

        internal Tower()
        {
            Level = 1;
        }

        public int RaiseLevel()
        {
            if (!IsComplete) Level++;
            return Level;
        }

        public int DecreaseLevel()
        {
            if (Level > 1) Level--;
            return Level;
        }

        public bool IsComplete
            => Level >= 4;
    }
}
