using CleanArchitecture.Application.Requests.SMS;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Interfaces.Services
{
    public interface ISMSService
    {
        Task SendAsync(SMSRequest request);
    }
}
