using System.Linq;

namespace Santorini
{
    public class Player
    {
        public string Name { get; private set; }

        public Figure[] Figuren { get; private set; }

        public Player(string name)
        {
            Name = name;
            Figuren = new Figure[0];
        }

        public void AddFigure(Position currentPosition)
        {
            Figuren = Figuren.Append(new Figure(Figuren.Length, currentPosition)).ToArray();
        }
    }
}
