using CleanArchitecture.Application.Requests.Mail;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Interfaces.Services
{
    public interface IMailService
    {
        Task SendAsync(MailRequest request);
    }
}