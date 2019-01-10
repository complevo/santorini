namespace Santorini
{
    public class Building : Piece
    {
        public int Level { get; private set; }

        internal Building()
        {
            Level = 1;
        }

        public int RaiseLevel()
        {
            if (Level < 4) Level++;
            return Level;
        }

        public bool MaxLevelReached
            => Level >= 4;
    }
}
