using Blazored.LocalStorage;
using CleanArchitecture.Shared.Constants.Application;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace CleanArchitecture.Client.Extensions
{
    public static class HubExtensions
    {
        public static HubConnection TryInitialize(this HubConnection hubConnection, NavigationManager navigationManager, ILocalStorageService _localStorage)
        {
            hubConnection ??= new HubConnectionBuilder()
                                  .WithUrl(navigationManager.ToAbsoluteUri(ApplicationConstants.SignalR.HubUrl), options =>
                                  {
                                      options.AccessTokenProvider = async () => (await _localStorage.GetItemAsync<string>("authToken"));
                                  })
                                  .WithAutomaticReconnect()
                                  .Build();
            return hubConnection;
        }
        public static HubConnection TryInitialize(this HubConnection hubConnection, NavigationManager navigationManager)
        {
            hubConnection ??= new HubConnectionBuilder()
                                  .WithUrl(navigationManager.ToAbsoluteUri(ApplicationConstants.SignalR.HubUrl))
                                  .Build();
            return hubConnection;
        }
    }
}