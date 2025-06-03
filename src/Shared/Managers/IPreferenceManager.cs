using CleanArchitecture.Shared.Settings;
using CleanArchitecture.Shared.Wrapper;
using System.Threading.Tasks;

namespace CleanArchitecture.Shared.Managers
{
    public interface IPreferenceManager
    {
        Task SetPreference(IPreference preference);

        Task<IPreference> GetPreference();

        Task<IResult> ChangeLanguageAsync(string languageCode);
    }
}