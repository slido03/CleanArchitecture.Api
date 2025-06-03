using CleanArchitecture.Shared.Constants.Localization;
using CleanArchitecture.Shared.Settings;
using System.Linq;

namespace CleanArchitecture.Server.Settings
{
    public record ServerPreference : IPreference
    {
        public string LanguageCode { get; set; } = LocalizationConstants.SupportedLanguages.FirstOrDefault()?.Code ?? "fr-FR";

        //TODO - add server preferences
    }
}