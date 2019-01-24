using System;

namespace Santorini
{
    public class Player
    {
        public string Name { get; }
        
        public Builder[] Builders { get; }

        internal Player(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

            Name = name;
            Builders = new[]
            {
                new Builder(this, 1),
                new Builder(this, 2),
            };
        }
    }
}
