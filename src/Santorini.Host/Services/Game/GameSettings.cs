using FluentValidation;

namespace Santorini.Host
{
    public class GameSettings
    {
        public static string Section = nameof(GameSettings);

        public PlayerSettings BluePlayer { get; set; }
        public PlayerSettings WhitePlayer { get; set; }
        public GameOutputFormat OutputFormat { get; set; }

        public bool IsValid
            => new GameSettingsValitation().Validate(this).IsValid;

        private class GameSettingsValitation : AbstractValidator<GameSettings>
        {
            public GameSettingsValitation()
            {
                RuleFor(p => p.BluePlayer).NotNull().Must(p => p.IsValid);
                RuleFor(p => p.WhitePlayer).NotNull().Must(p => p.IsValid);
                RuleFor(p => p.OutputFormat).NotNull();
            }
        }
    }
}
