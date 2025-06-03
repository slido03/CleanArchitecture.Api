using CleanArchitecture.Application.Responses.Identity;
using CleanArchitecture.Shared.Wrapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using CleanArchitecture.Application.Interfaces.Chat;
using CleanArchitecture.Application.Models.Chat;

namespace CleanArchitecture.Application.Interfaces.Services
{
    public interface IChatService
    {
        Task<Result<IEnumerable<ChatUserResponse>>> GetChatUsersAsync(string userId);

        Task<IResult> SaveMessageAsync(ChatHistory<IChatUser> message);

        Task<Result<IEnumerable<ChatHistoryResponse>>> GetChatHistoryAsync(string userId, string contactId);
    }
}