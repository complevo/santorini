namespace Santorini
{
    public class Move
    {
        public Location NewLocation { get; private set; }
        public Location BuildLocation { get; private set; }

        public string PlayerName { get; private set; }
        public int FigureNubmer { get; private set; }

        public Move(string playerName, int figureNumber, Location newLocation, Location buildLocation)
        {
            PlayerName = playerName;
            FigureNubmer = figureNumber;
            NewLocation = newLocation;
            BuildLocation = buildLocation;
        }
    }
}
