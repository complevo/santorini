using System;

namespace Santorini
{
    public abstract class Piece
    {
        public Guid Id { get; }
        public Land CurrentLand { get; protected set; }

        protected Piece()
        {
            Id = Guid.NewGuid();
            CurrentLand = null;
        }

        internal void SetLand(Land land)
        {
            CurrentLand = land;
        }
    }
}
