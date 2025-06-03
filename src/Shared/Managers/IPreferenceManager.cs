using CleanArchitecture.Shared.Settings;
using System.Threading.Tasks;
using CleanArchitecture.Shared.Wrapper;

namespace CleanArchitecture.Shared.Managers
{
    public interface IPreferenceManager
    {
        Task SetPreference(IPreference preference);

        Task<IPreference> GetPreference();

        Task<IResult> ChangeLanguageAsync(string languageCode);
    }
}