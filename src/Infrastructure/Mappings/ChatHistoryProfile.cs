using AutoMapper;
using CleanArchitecture.Application.Interfaces.Chat;
using CleanArchitecture.Application.Models.Chat;
using CleanArchitecture.Infrastructure.Models.Identity;

namespace CleanArchitecture.Infrastructure.Mappings
{
    public class ChatHistoryProfile : Profile
    {
        public ChatHistoryProfile()
        {
            CreateMap<ChatHistory<IChatUser>, ChatHistory<User>>().ReverseMap();
        }
    }
}