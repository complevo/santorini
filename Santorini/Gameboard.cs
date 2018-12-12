using System.Linq;

namespace Santorini
{
    public class Gameboard
    {
        public int[][] Buildings { get; private set; }

        public Player[] Players { get; private set; }

        public Gameboard()
        {
            Buildings = new int[5][];
            for (int i = 0; i < 5; i++)
            {
                Buildings[i] = new int[5];
                for (int j = 0; j < 5; j++)
                    Buildings[i][j] = 0;
            }
            Players = new Player[0];
        }

        public void AddPlayer(Player player)
        {
            Players = Players.Append(player).ToArray();
        }

        public Player GetPlayer(string name)
        {
            return Players.FirstOrDefault(p => p.Name.Equals(name));
        }
    }
}
