using CleanArchitecture.Application.Features.ExternalApplications.Commands.AddEdit;
using CleanArchitecture.Application.Features.ExternalApplications.Commands.Import;
using CleanArchitecture.Application.Features.ExternalApplications.Queries.GetAll;
using CleanArchitecture.Application.Requests;
using CleanArchitecture.Client.Infrastructure.Managers.Sgcd.ExternalApplication;
using CleanArchitecture.Client.Shared.Components;
using CleanArchitecture.Shared.Constants.Application;
using CleanArchitecture.Shared.Constants.Permission;
using CleanArchitecture.Shared.Wrapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CleanArchitecture.Client.Pages.Sgcd
{
    public partial class ExternalApplications
    {
        [Inject] private IExternalApplicationManager ExternalApplicationManager { get; set; }

        private IEnumerable<GetAllExternalApplicationsResponse> _pagedData;
        private MudTable<GetAllExternalApplicationsResponse> _table;
        private int _totalItems;
        private string _searchString = "";
        private bool _dense = false;
        private bool _striped = true;
        private bool _bordered = false;

        private ClaimsPrincipal _currentUser;
        private bool _canCreateExternalApplications;
        private bool _canEditExternalApplications;
        private bool _canDeleteExternalApplications;
        private bool _canSearchExternalApplications;
        private bool _canExportExternalApplications;
        private bool _canImportExternalApplications;
        private bool _loaded;


        protected override async Task OnInitializedAsync()
        {
            _currentUser = await _authenticationManager.CurrentUser();
            _canCreateExternalApplications = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.ExternalApplications.Create)).Succeeded;
            _canEditExternalApplications = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.ExternalApplications.Edit)).Succeeded;
            _canDeleteExternalApplications = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.ExternalApplications.Delete)).Succeeded;
            _canSearchExternalApplications = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.ExternalApplications.Search)).Succeeded;
            _canExportExternalApplications = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.ExternalApplications.Export)).Succeeded;
            _canImportExternalApplications = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.ExternalApplications.Import)).Succeeded;
            _loaded = true;
        }

        private async Task<TableData<GetAllExternalApplicationsResponse>> ServerReload(TableState state)
        {
            if (!string.IsNullOrWhiteSpace(_searchString))
            {
                state.Page = 0;
            }
            await LoadData(state.Page, state.PageSize, state);
            return new TableData<GetAllExternalApplicationsResponse> { TotalItems = _totalItems, Items = _pagedData };
        }

        private async Task LoadData(int pageNumber, int pageSize, TableState state)
        {
            string ordering = string.Empty;
            if (!string.IsNullOrEmpty(state.SortLabel))
            {
                ordering = (state.SortDirection != SortDirection.None) ? $"{state.SortLabel} {state.SortDirection}" : $"{state.SortLabel}";
            }

            var request = new GetAllExternalApplicationsQuery(pageNumber, pageSize, _searchString, ordering);
            var response = await ExternalApplicationManager.GetAllPagedAsync(request);
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
                var app = _pagedData.FirstOrDefault(c => c.Id == id);
                if (app != null)
                {
                    parameters.Add(nameof(AddEditExternalApplicationModal.AddEditExternalApplicationModel), new AddEditExternalApplicationCommand
                    {
                        Id = app.Id,
                        Name = app.Name,
                        Description = app.Description,
                    });
                }
            }
            var options = new DialogOptions
            {
                CloseButton = true,
                MaxWidth = MaxWidth.Medium,
                FullWidth = true,
                DisableBackdropClick = true
            };
            var dialog = _dialogService.Show<AddEditExternalApplicationModal>(id == 0 ? _localizer["Create"] : _localizer["Edit"], parameters, options);
            var result = await dialog.Result;
            if (!result.Cancelled)
            {
                OnSearch("");
            }
        }

        private async Task Delete(int id)
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
                var response = await ExternalApplicationManager.DeleteAsync(id);
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
            var response = await ExternalApplicationManager.ExportToExcelAsync(_searchString);
            if (response.Succeeded)
            {
                await _jsRuntime.InvokeVoidAsync("Download", new
                {
                    ByteArray = response.Data,
                    FileName = $"{nameof(ExternalApplications).ToLower()}_{DateTime.Now:ddMMyyyyHHmmss}.xlsx",
                    MimeType = ApplicationConstants.MimeTypes.OpenXml
                });
                _snackBar.Add(string.IsNullOrWhiteSpace(_searchString)
                    ? _localizer["External Applications exported"]
                    : _localizer["Filtered External Applications exported"], Severity.Success);
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
            var request = new ImportExternalApplicationsCommand { UploadRequest = uploadFile };
            var result = await ExternalApplicationManager.ImportAsync(request);
            return result;
        }

        private async Task InvokeImportModal()
        {
            var parameters = new DialogParameters
            {
                { nameof(ImportExcelModal.ModelName), _localizer["External Applications"].ToString() }
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
            var dialog = _dialogService.Show<ImportExcelModal>(_localizer["Import External Applications"], parameters, options);
            var result = await dialog.Result;
            if (!result.Cancelled)
            {
                OnSearch("");
            }
        }
    }
}
