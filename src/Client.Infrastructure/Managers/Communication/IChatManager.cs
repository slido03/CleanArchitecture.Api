using CleanArchitecture.Application.Interfaces.Chat;
using CleanArchitecture.Application.Models.Chat;
using CleanArchitecture.Application.Responses.Identity;
using CleanArchitecture.Shared.Wrapper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CleanArchitecture.Client.Infrastructure.Managers.Communication
{
    public interface IChatManager : IManager
    {
        Task<IResult<IEnumerable<ChatUserResponse>>> GetChatUsersAsync();

        Task<IResult> SaveMessageAsync(ChatHistory<IChatUser> chatHistory);

        Task<IResult<IEnumerable<ChatHistoryResponse>>> GetChatHistoryAsync(string cId);
    }
}