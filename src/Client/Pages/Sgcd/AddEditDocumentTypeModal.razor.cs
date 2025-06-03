using Blazored.FluentValidation;
using CleanArchitecture.Application.Features.DocumentTypes.Commands.AddEdit;
using CleanArchitecture.Application.Features.ExternalApplications.Queries.GetAll;
using CleanArchitecture.Client.Extensions;
using CleanArchitecture.Client.Infrastructure.Managers.Sgcd.DocumentType;
using CleanArchitecture.Client.Infrastructure.Managers.Sgcd.ExternalApplication;
using CleanArchitecture.Shared.Constants.Application;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static CleanArchitecture.Shared.Constants.Application.ApplicationConstants.Formats;

namespace CleanArchitecture.Client.Pages.Sgcd
{
    public partial class AddEditDocumentTypeModal
    {
        [Inject] private IDocumentTypeManager DocumentTypeManager { get; set; }
        [Inject] private IExternalApplicationManager ExternalApplicationManager { get; set; }

        [Parameter] public AddEditDocumentTypeCommand AddEditDocumentTypeModel { get; set; } = new();
        [CascadingParameter] private MudDialogInstance MudDialog { get; set; }
        [CascadingParameter] private HubConnection HubConnection { get; set; }

        private FluentValidationValidator _fluentValidationValidator;
        private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });
        private List<GetAllExternalApplicationsResponse> _applications = new();
        private List<string> _documentFormats = new();

        public void Cancel()
        {
            MudDialog.Cancel();
        }

        private async Task SaveAsync()
        {
            var response = await DocumentTypeManager.SaveAsync(AddEditDocumentTypeModel);
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

        private async Task LoadDataAsync()
        {
            LoadDocumentFormats();
            await LoadExternalApplicationsAsync();
        }

        private async Task LoadExternalApplicationsAsync()
        {
            var data = await ExternalApplicationManager.GetAllAsync();
            if (data.Succeeded)
            {
                _applications = data.Data;
            }
        }

        private void LoadDocumentFormats()
        {
            _documentFormats = DocumentFormats.GetRegisteredFormats();
        }

        private async Task<IEnumerable<string>> SearchExternalApplications(string value)
        {
            // In real life use an asynchronous function for fetching data from an api.
            await Task.Delay(5);

            // if text is null or empty, show complete list
            if (string.IsNullOrEmpty(value))
                return _applications.Select(x => x.Name);

            return _applications.Where(x => x.Name.Contains(value, System.StringComparison.InvariantCultureIgnoreCase))
                .Select(x => x.Name);
        }

        private async Task<IEnumerable<string>> SearchDocumentFormats(string value)
        {
            // In real life use an asynchronous function for fetching data from an api.
            await Task.Delay(5);

            // if text is null or empty, show complete list
            if (string.IsNullOrEmpty(value))
                return _documentFormats;

            return _documentFormats.Where(x => x.Contains(value, System.StringComparison.InvariantCultureIgnoreCase))
                .Select(x => x);
        }
    }
}