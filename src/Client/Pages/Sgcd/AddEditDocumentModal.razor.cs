using Blazored.FluentValidation;
using CleanArchitecture.Application.Features.Documents.Commands.AddEdit;
using CleanArchitecture.Application.Features.DocumentTypes.Queries.GetAll;
using CleanArchitecture.Application.Features.ExternalApplications.Queries.GetAll;
using CleanArchitecture.Application.Requests;
using CleanArchitecture.Client.Infrastructure.Managers.Sgcd.Document;
using CleanArchitecture.Client.Infrastructure.Managers.Sgcd.DocumentType;
using CleanArchitecture.Client.Infrastructure.Managers.Sgcd.ExternalApplication;
using CleanArchitecture.Shared.Constants.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;


namespace CleanArchitecture.Client.Pages.Sgcd
{
    public partial class AddEditDocumentModal
    {
        [Inject] private IDocumentManager DocumentManager { get; set; }
        [Inject] private IDocumentTypeManager DocumentTypeManager { get; set; }
        [Inject] private IExternalApplicationManager ExternalApplicationManager { get; set; }

        [Parameter] public AddEditDocumentCommand AddEditDocumentModel { get; set; } = new();
        [CascadingParameter] private MudDialogInstance MudDialog { get; set; }

        private FluentValidationValidator _fluentValidationValidator;
        private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });
        private List<GetAllExternalApplicationsResponse> _applications = new();
        private List<GetAllDocumentTypesResponse> _documentTypes = new();
        private string _selectedApplication = string.Empty;
        private ClaimsPrincipal _currentUser;
        private bool _canCreateDocumentMatchings;
        private bool _canEditDocumentMatchings;
        private bool _submitted = false;

        public void Cancel()
        {
            MudDialog.Cancel();
        }

        private async Task SaveAsync()
        {
            _submitted = true;
            var response = await DocumentManager.SaveAsync(AddEditDocumentModel);
            if (response.Succeeded)
            {
                _snackBar.Add(response.Messages[0], Severity.Success);
                MudDialog.Close();
            }
            else
            {
                _submitted = false;
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
        }

        protected override async Task OnInitializedAsync()
        {
            _currentUser = await _authenticationManager.CurrentUser();
            _canCreateDocumentMatchings = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.DocumentMatchings.Create)).Succeeded;
            _canEditDocumentMatchings = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.DocumentMatchings.Edit)).Succeeded;
            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            await LoadExternalApplicationsAsync();
            await LoadDocumentTypesAsync();
        }

        private async Task LoadExternalApplicationsAsync()
        {
            var data = await ExternalApplicationManager.GetAllAsync();
            if (data.Succeeded)
            {
                _applications = data.Data;
            }
        }

        private async Task LoadDocumentTypesAsync()
        {
            var data = await DocumentTypeManager.GetAllAsync();
            if (data.Succeeded)
            {
                _documentTypes = data.Data;
            }
        }

        private IBrowserFile _file;

        private async Task UploadFiles(InputFileChangeEventArgs e)
        {
            _file = e.File;
            if (_file != null)
            {
                var buffer = new byte[_file.Size];
                var extension = Path.GetExtension(_file.Name);
                var format = "application/octet-stream";
                await _file.OpenReadStream(_file.Size).ReadAsync(buffer);
                AddEditDocumentModel.URL = $"data:{format};base64,{Convert.ToBase64String(buffer)}";
                AddEditDocumentModel.UploadRequest = new UploadRequest { Data = buffer, UploadType = Application.Enums.UploadType.Document, Extension = extension };
            }
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

        private async Task<IEnumerable<string>> SearchDocumentTypes(string value)
        {
            // In real life use an asynchronous function for fetching data from an api.
            await Task.Delay(5);

            var documentTypesByApplication = _documentTypes
                                                .Where(x => x.ExternalApplication == _selectedApplication)
                                                .ToList();
            // if text is null or empty, show complete list
            if (string.IsNullOrEmpty(value))
                return documentTypesByApplication
                    .Select(x => x.Name);

            return documentTypesByApplication
                .Where(x => x.Name.Contains(value, StringComparison.InvariantCultureIgnoreCase))
                .Select(x => x.Name);
        }

        private static bool IsValidDbUrl(string url)
        {
            return url.StartsWith("Files");
        }
    }
}