using System;
using System.Runtime.Serialization;

namespace Santorini
{
    public abstract class Piece
    {
        public Guid Id { get; }

        [IgnoreDataMember]
        public Land CurrentLand { get; private set; }

        protected Piece()
        {
            Id = Guid.NewGuid();
            CurrentLand = null;
        }

        public bool IsPlaced
            => CurrentLand != null;

        internal void SetLand(Land land)
        {
            if (land is null) throw new ArgumentNullException(nameof(land));

            CurrentLand = land;
        }
    }
}
