using System;

namespace Santorini.Host
{
    public class AddPlayerException : Exception
    {
        public string PlayerName { get; private set; }

        public AddPlayerException(string playerName)
            => PlayerName = playerName;
    }

    public class SettingsNotValidException : Exception
    {
        public GameSettings Settings { get; private set; }

        public SettingsNotValidException(GameSettings settings)
            => Settings = settings;
    }
}
