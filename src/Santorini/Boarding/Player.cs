using System;

namespace Santorini
{
    public class Player
    {
        public string Name { get; }
        
        public Worker[] Workers { get; }

        internal Player(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

            Name = name;
            Workers = new[]
            {
                new Worker(this, 1),
                new Worker(this, 2),
            };
        }
    }
}
