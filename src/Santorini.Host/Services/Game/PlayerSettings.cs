using FluentValidation;
using Flurl;

namespace Santorini.Host
{
    public class PlayerSettings
    {
        public string Name { get; set; }
        public string BaseUrl { get; set; }
        public string MoveEndpoint { get; set; }
        public string AddWorkerEndpoint { get; set; }
        public string GameReportEndpoint { get; set; }

        public bool IsValid
            => new PlayerSettingsValidation().Validate(this).IsValid;

        private class PlayerSettingsValidation : AbstractValidator<PlayerSettings>
        {
            public PlayerSettingsValidation()
            {
                RuleFor(p => p.Name).NotNull().NotEmpty();
                RuleFor(p => p.BaseUrl).Must(url => Url.IsValid(url));
                RuleFor(p => p.MoveEndpoint).Must(moveEp => Url.IsValid("http://complevo.de".AppendPathSegment(moveEp)));
                RuleFor(p => p.AddWorkerEndpoint).Must(addWorkerEp => Url.IsValid("http://complevo.de".AppendPathSegment(addWorkerEp)));
                RuleFor(p => p.GameReportEndpoint).Must(reportEp => Url.IsValid("http://complevo.de".AppendPathSegment(reportEp)));
            }
        }
    }
}
