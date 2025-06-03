using Blazored.FluentValidation;
using CleanArchitecture.Application.Features.ExternalApplications.Commands.AddEdit;
using CleanArchitecture.Client.Extensions;
using CleanArchitecture.Client.Infrastructure.Managers.Sgcd.ExternalApplication;
using CleanArchitecture.Shared.Constants.Application;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using System.Threading.Tasks;

namespace CleanArchitecture.Client.Pages.Sgcd
{
    public partial class AddEditExternalApplicationModal
    {
        [Inject] private IExternalApplicationManager ExternalApplicationManager { get; set; }

        [Parameter] public AddEditExternalApplicationCommand AddEditExternalApplicationModel { get; set; } = new();
        [CascadingParameter] private MudDialogInstance MudDialog { get; set; }
        [CascadingParameter] private HubConnection HubConnection { get; set; }

        private FluentValidationValidator _fluentValidationValidator;
        private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });

        public void Cancel()
        {
            MudDialog.Cancel();
        }

        private async Task SaveAsync()
        {
            var response = await ExternalApplicationManager.SaveAsync(AddEditExternalApplicationModel);
            if (response.Succeeded)
            {
                _snackBar.Add(response.Messages[0], Severity.Success);
                MudDialog.Close();
            }
            else
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
            await HubConnection.SendAsync(ApplicationConstants.SignalR.SendUpdateDashboard);
        }

        protected override async Task OnInitializedAsync()
        {
            await LoadDataAsync();
            HubConnection = HubConnection.TryInitialize(_navigationManager, _localStorage);
            if (HubConnection.State == HubConnectionState.Disconnected)
            {
                await HubConnection.StartAsync();
            }
        }

        private static async Task LoadDataAsync()
        {
            await Task.CompletedTask;
        }
    }
}
