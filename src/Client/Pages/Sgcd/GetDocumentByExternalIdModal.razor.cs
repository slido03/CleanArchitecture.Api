using CleanArchitecture.Application.Features.Documents.Commands.AddEdit;
using CleanArchitecture.Application.Features.Documents.Queries.GetByExternalId;
using CleanArchitecture.Application.Features.DocumentTypes.Queries.GetAll;
using CleanArchitecture.Application.Features.ExternalApplications.Queries.GetAll;
using CleanArchitecture.Client.Extensions;
using CleanArchitecture.Client.Infrastructure.Managers.Sgcd.Document;
using CleanArchitecture.Client.Infrastructure.Managers.Sgcd.DocumentType;
using CleanArchitecture.Client.Infrastructure.Managers.Sgcd.ExternalApplication;
using CleanArchitecture.Shared.Constants.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CleanArchitecture.Client.Pages.Sgcd
{
    public partial class GetDocumentByExternalIdModal
    {
        [Inject] private IDocumentManager DocumentManager { get; set; }
        [Inject] private IDocumentTypeManager DocumentTypeManager { get; set; }
        [Inject] private IExternalApplicationManager ExternalApplicationManager { get; set; }

        [CascadingParameter] private MudDialogInstance MudDialog { get; set; }

        private ClaimsPrincipal _currentUser;
        private bool _canEditDocuments;
        private bool _canDeleteDocuments;
        private bool _canViewDocumentVersions;
        private bool _loaded;
        private string _currentUserId;

        private List<GetDocumentByExternalIdResponse> _documents = new List<GetDocumentByExternalIdResponse>();
        private MudTable<GetDocumentByExternalIdResponse> _table;
        private List<GetAllExternalApplicationsResponse> _applications = new();
        private List<GetAllDocumentTypesResponse> _documentTypes = new();
        private string _selectedApplication = string.Empty;
        private string _selectedType = string.Empty;
        private string _externalId = string.Empty;

        public void Close()
        {
            MudDialog.Close();
        }

        protected override async Task OnInitializedAsync()
        {
            _currentUser = await _authenticationManager.CurrentUser();
            _canEditDocuments = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Documents.Edit)).Succeeded;
            _canDeleteDocuments = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Documents.Delete)).Succeeded;
            _canViewDocumentVersions = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.DocumentVersions.View)).Succeeded;

            var state = await _stateProvider.GetAuthenticationStateAsync();
            var user = state.User;
            if (user == null) return;
            if (user.Identity?.IsAuthenticated == true)
            {
                _currentUserId = user.GetUserId();
            }
            await LoadDataAsync();
            _loaded = true;
        }

        private async Task LoadDataAsync()
        {
            await LoadExternalApplicationsAsync();
            await LoadDocumentTypesAsync();



        }

        private async Task OnSearch(string documentType, string externalId)
        {
            if (string.IsNullOrWhiteSpace(documentType) || string.IsNullOrWhiteSpace(externalId))
                _documents.Clear();

            _selectedType = documentType;
            _externalId = externalId;
            var request = new GetDocumentByExternalIdQuery(_selectedType, _externalId);
            var response = await DocumentManager.GetByExternalIdAsync(request);
            if (response.Succeeded)
            {
                if ((response.Data.CreatedBy == _currentUserId) || response.Data.IsPublic == true)
                {
                    _documents.Clear();
                    _documents.Add(response.Data);
                }
                else
                {
                    _documents.Clear();
                }
            }
            else
            {
                _documents.Clear();
                await _table.ReloadServerData();
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
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

        private async Task InvokeAddEditModal(Guid id)
        {
            var parameters = new DialogParameters();
            if (id != Guid.Empty)
            {
                var doc = _documents.FirstOrDefault(c => c.Id == id);
                if (doc != null)
                {
                    parameters.Add(nameof(AddEditDocumentModal.AddEditDocumentModel), new AddEditDocumentCommand
                    {
                        Id = doc.Id,
                        Title = doc.Title,
                        Description = doc.Description,
                        URL = doc.URL,
                        IsPublic = doc.IsPublic,
                        DocumentType = doc.DocumentType,
                        ExternalId = doc.ExternalId,
                    });
                }
            }
            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Medium, FullWidth = true, DisableBackdropClick = true };
            var dialog = _dialogService.Show<AddEditDocumentModal>(id == Guid.Empty ? _localizer["Create"] : _localizer["Edit"], parameters, options);
            var result = await dialog.Result;
            if (!result.Cancelled)
            {
                await _table.ReloadServerData();
            }
        }

        private async Task InvokeDocVersionsModal(Guid id)
        {
            var parameters = new DialogParameters();
            if (id != Guid.Empty)
            {
                parameters.Add(nameof(DocumentVersionsModal.DocumentId), id);
            }
            var options = new DialogOptions
            {
                CloseButton = true,
                MaxWidth = MaxWidth.Large,
                FullWidth = true,
                DisableBackdropClick = true
            };
            var dialog = _dialogService.Show<DocumentVersionsModal>(_localizer["Document Versions"], parameters, options);
            var result = await dialog.Result;
            if (!result.Cancelled)
            {
                await _table.ReloadServerData();
            }
        }

        private async Task Delete(Guid id)
        {
            string deleteContent = _localizer["Delete {0} Content"];
            var parameters = new DialogParameters
            {
                {nameof(Shared.Dialogs.DeleteConfirmation.ContentText), string.Format(deleteContent, id)}
            };
            var options = new DialogOptions
            {
                CloseButton = true,
                MaxWidth = MaxWidth.Small,
                FullWidth = true,
                DisableBackdropClick = true
            };
            var dialog = _dialogService.Show<Shared.Dialogs.DeleteConfirmation>(_localizer["Deletion"], parameters, options);
            var result = await dialog.Result;
            if (!result.Cancelled)
            {
                var response = await DocumentManager.DeleteAsync(id);
                if (response.Succeeded)
                {
                    await _table.ReloadServerData();
                    _snackBar.Add(response.Messages[0], Severity.Success);
                }
                else
                {
                    await _table.ReloadServerData();
                    foreach (var message in response.Messages)
                    {
                        _snackBar.Add(message, Severity.Error);
                    }
                }
            }
        }
    }
}



