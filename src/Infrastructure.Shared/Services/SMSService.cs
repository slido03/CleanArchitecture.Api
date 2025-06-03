using CleanArchitecture.Application.Configurations;
using CleanArchitecture.Application.Interfaces.Services;
using CleanArchitecture.Application.Requests.SMS;
using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace CleanArchitecture.Infrastructure.Shared.Services
{
    public class SMSService : ISMSService
    {
        private readonly SMSConfiguration _config;
        private readonly ILogger<SMSService> _logger;
        public SMSService(IOptions<SMSConfiguration> config, ILogger<SMSService> logger)
        {
            _config = config.Value;
            _logger = logger;
        }
        public async Task SendAsync(SMSRequest request)
        {
            try
            {
                var _token = await _config.Host.AppendPathSegment("authenticate").PostJsonAsync(new
                {
                    username = _config.User,
                    password = _config.Password
                }).ReceiveJson<AuthResponse>();
                var response = await _config.Host.AppendPathSegment("authenticate").WithOAuthBearerToken(_token.Jwt).PostJsonAsync(new
                {
                    to = request.To,
                    text = request.Text,
                    from = _config.From,
                    dlr_url = "",
                    validy = "",
                    deffered = "",
                    origin = _config.Origin,
                });
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
        }
    }
}
