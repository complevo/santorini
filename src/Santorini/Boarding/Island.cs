using System.Collections.Generic;

namespace Santorini
{
    public class Island
    {
        public readonly Land[,] Board;

        public static (int X, int Y) MaxPositions => (4, 4);

        internal Island()
        {
            Board = new Land[5, 5];

            for (var x = 0; x <= 4; x++)
                for (var y = 0; y <= 4; y++)
                    Board[x, y] = new Land(this, x, y);
        }

        public bool IsUnoccupied(int posX, int posY)
            => Board[posX, posY].IsUnoccupied;

        public bool TryGetLand(int posX, int posY, out Land land)
        {
            land = default(Land);
            if (!IsValidPosition(posX, posY)) return false;

            land = Board[posX, posY];

            return true;
        }

        public bool TryAddPiece(Piece piece, int posX, int posY)
        {
            if (TryGetLand(posX, posY, out var land))
                return land.TryPutPiece(piece);

            return false;
        }    

        public Worker GetWorker(string playerName, int workerNumber)
        {
            foreach (var land in Board)
                if (land.HasWorker 
                    && land.Worker.Player.Name == playerName 
                    && land.Worker.Number == workerNumber)
                    return land.Worker;
            return null;
        }

        public IEnumerable<Worker> GetAllWorkers()
        {
            List<Worker> allWorkers = new List<Worker>();
            foreach (var land in Board)
                if (land.HasWorker)
                    allWorkers.Add(land.Worker);

            return allWorkers;
        }

        public static bool IsValidPosition(int posX, int posY)
            => posX >= 0 && posY >= 0
            && posX <= MaxPositions.X && posY <= MaxPositions.Y;
    }
}
