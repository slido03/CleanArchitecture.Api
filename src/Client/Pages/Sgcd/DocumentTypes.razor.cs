using CleanArchitecture.Application.Features.DocumentTypes.Commands.AddEdit;
using CleanArchitecture.Application.Features.DocumentTypes.Commands.Import;
using CleanArchitecture.Application.Features.DocumentTypes.Queries.GetAll;
using CleanArchitecture.Application.Requests;
using CleanArchitecture.Client.Infrastructure.Managers.Sgcd.DocumentType;
using CleanArchitecture.Client.Shared.Components;
using CleanArchitecture.Shared.Constants.Application;
using CleanArchitecture.Shared.Constants.Permission;
using CleanArchitecture.Shared.Wrapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CleanArchitecture.Client.Pages.Sgcd
{
    public partial class DocumentTypes
    {
        [Inject] private IDocumentTypeManager DocumentTypeManager { get; set; }

        [CascadingParameter] private HubConnection HubConnection { get; set; }

        private IEnumerable<GetAllDocumentTypesResponse> _pagedData;
        private MudTable<GetAllDocumentTypesResponse> _table;
        private int _totalItems;
        private string _searchString = "";
        private bool _dense = false;
        private bool _striped = true;
        private bool _bordered = false;

        private ClaimsPrincipal _currentUser;
        private bool _canCreateDocumentTypes;
        private bool _canEditDocumentTypes;
        private bool _canDeleteDocumentTypes;
        private bool _canExportDocumentTypes;
        private bool _canImportDocumentTypes;
        private bool _canSearchDocumentTypes;
        private bool _loaded;

        protected override async Task OnInitializedAsync()
        {
            _currentUser = await _authenticationManager.CurrentUser();
            _canCreateDocumentTypes = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.DocumentTypes.Create)).Succeeded;
            _canEditDocumentTypes = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.DocumentTypes.Edit)).Succeeded;
            _canDeleteDocumentTypes = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.DocumentTypes.Delete)).Succeeded;
            _canExportDocumentTypes = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.DocumentTypes.Export)).Succeeded;
            _canImportDocumentTypes = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.DocumentTypes.Import)).Succeeded;
            _canSearchDocumentTypes = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.DocumentTypes.Search)).Succeeded;
            _loaded = true;
        }

        private async Task<TableData<GetAllDocumentTypesResponse>> ServerReload(TableState state)
        {
            if (!string.IsNullOrWhiteSpace(_searchString))
            {
                state.Page = 0;
            }
            await LoadData(state.Page, state.PageSize, state);
            return new TableData<GetAllDocumentTypesResponse> { TotalItems = _totalItems, Items = _pagedData };
        }

        private async Task LoadData(int pageNumber, int pageSize, TableState state)
        {
            string ordering = string.Empty;
            if (!string.IsNullOrEmpty(state.SortLabel))
            {
                ordering = (state.SortDirection != SortDirection.None) ? $"{state.SortLabel} {state.SortDirection}" : $"{state.SortLabel}";
            }

            var request = new GetAllDocumentTypesQuery(pageNumber, pageSize, _searchString, ordering);
            var response = await DocumentTypeManager.GetAllPagedAsync(request);
            if (response.Succeeded)
            {
                _totalItems = response.TotalCount;
                _pagedData = response.Data;
            }
            else
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
        }

        private void OnSearch(string text)
        {
            _searchString = text;
            _table.ReloadServerData();
        }

        private async Task InvokeAddEditModal(int id)
        {
            var parameters = new DialogParameters();
            if (id != 0)
            {
                var docType = _pagedData.FirstOrDefault(c => c.Id == id);
                if (docType != null)
                {
                    parameters.Add(nameof(AddEditDocumentTypeModal.AddEditDocumentTypeModel), new AddEditDocumentTypeCommand
                    {
                        Id = docType.Id,
                        Name = docType.Name,
                        Description = docType.Description,
                        Format = docType.Format,
                        ExternalApplication = docType.ExternalApplication,
                    });
                }
            }
            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Medium, FullWidth = true, DisableBackdropClick = true };
            var dialog = _dialogService.Show<AddEditDocumentTypeModal>(id == 0 ? _localizer["Create"] : _localizer["Edit"], parameters, options);
            var result = await dialog.Result;
            if (!result.Cancelled)
            {
                OnSearch("");
            }
        }

        private async Task Delete(int id)
        {
            string deleteContent = _localizer["Delete Content"];
            var parameters = new DialogParameters
            {
                {nameof(Shared.Dialogs.DeleteConfirmation.ContentText), string.Format(deleteContent, id)}
            };
            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, DisableBackdropClick = true };
            var dialog = _dialogService.Show<Shared.Dialogs.DeleteConfirmation>(_localizer["Deletion"], parameters, options);
            var result = await dialog.Result;
            if (!result.Cancelled)
            {
                var response = await DocumentTypeManager.DeleteAsync(id);
                if (response.Succeeded)
                {
                    OnSearch("");
                    _snackBar.Add(response.Messages[0], Severity.Success);
                }
                else
                {
                    OnSearch("");
                    foreach (var message in response.Messages)
                    {
                        _snackBar.Add(message, Severity.Error);
                    }
                }
            }
        }

        private async Task ExportToExcel()
        {
            var response = await DocumentTypeManager.ExportToExcelAsync(_searchString);
            if (response.Succeeded)
            {
                await _jsRuntime.InvokeVoidAsync("Download", new
                {
                    ByteArray = response.Data,
                    FileName = $"{nameof(DocumentTypes).ToLower()}_{DateTime.Now:ddMMyyyyHHmmss}.xlsx",
                    MimeType = ApplicationConstants.MimeTypes.OpenXml
                });
                _snackBar.Add(string.IsNullOrWhiteSpace(_searchString)
                    ? _localizer["Document Types exported"]
                    : _localizer["Filtered Document Types exported"], Severity.Success);
            }
            else
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
        }

        private async Task<IResult<int>> ImportExcel(UploadRequest uploadFile)
        {
            var request = new ImportDocumentTypesCommand { UploadRequest = uploadFile };
            var result = await DocumentTypeManager.ImportAsync(request);
            return result;
        }

        private async Task InvokeImportModal()
        {
            var parameters = new DialogParameters
            {
                { nameof(ImportExcelModal.ModelName), _localizer["Document Types"].ToString() }
            };
            Func<UploadRequest, Task<IResult<int>>> importExcel = ImportExcel;
            parameters.Add(nameof(ImportExcelModal.OnSaved), importExcel);
            var options = new DialogOptions
            {
                CloseButton = true,
                MaxWidth = MaxWidth.Small,
                FullWidth = true,
                DisableBackdropClick = true
            };
            var dialog = _dialogService.Show<ImportExcelModal>(_localizer["Import Document Types"], parameters, options);
            var result = await dialog.Result;
            if (!result.Cancelled)
            {
                OnSearch("");
            }
        }
    }
}
