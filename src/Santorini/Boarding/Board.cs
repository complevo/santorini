namespace Santorini
{
    public class Board
    {
        public Land[,] Lands { get; }

        public static (int X, int Y) MaxPositions => (4, 4);

        internal Board()
        {
            Lands = new Land[5, 5];

            for (var x = 0; x <= 4; x++)
                for (var y = 0; y <= 4; y++)
                    Lands[x, y] = new Land(this, x, y);
        }

        public bool IsEmpty(int posX, int posY)
            => Lands[posX, posY].IsEmpty;

        public bool TryGetLand(int posX, int posY, out Land land)
        {
            land = default(Land);
            if (!IsValidPosition(posX, posY)) return false;

            land = Lands[posX, posY];

            return true;
        }

        public bool TryAddPiece(Piece piece, int posX, int posY)
        {
            if (TryGetLand(posX, posY, out var land))
                return land.TryPutPiece(piece);

            return false;
        }    

        public Builder GetBuilder(string playerName, int builderNumber)
        {
            foreach (var land in Lands)
                if (land.HasBuilder 
                    && land.Builder.Player.Name == playerName 
                    && land.Builder.Number == builderNumber)
                    return land.Builder;
            return null;
        }      

        public static bool IsValidPosition(int posX, int posY)
            => posX >= 0 && posY >= 0
            && posX <= MaxPositions.X && posY <= MaxPositions.Y;
    }
}
