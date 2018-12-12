namespace Santorini
{
    public class Figure
    {
        public int Number { get; private set; }

        public Position CurrentPosition { get; private set; }

        public Figure(int number, Position currentPosition)
        {
            Number = number;
            CurrentPosition = currentPosition;
        }

        internal void ChangePosition(Position position)
        {
            CurrentPosition = position;
        }
    }
}
