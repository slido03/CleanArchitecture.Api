using CleanArchitecture.Application.Features.Documents.Commands.RestoreVersion;
using CleanArchitecture.Application.Features.DocumentVersions.Queries.GetAllByDocument;
using CleanArchitecture.Client.Infrastructure.Managers.Sgcd.Document;
using CleanArchitecture.Client.Infrastructure.Managers.Sgcd.DocumentVersion;
using CleanArchitecture.Shared.Constants.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CleanArchitecture.Client.Pages.Sgcd
{
    public partial class DocumentVersionsModal
    {
        [Inject] private IDocumentVersionManager DocumentVersionManager { get; set; }
        [Inject] private IDocumentManager DocumentManager { get; set; }
        [Parameter] public Guid DocumentId { get; set; }
        [CascadingParameter] private MudDialogInstance MudDialog { get; set; }

        private IEnumerable<GetAllDocumentVersionsByDocumentResponse> _pagedData;
        private MudTable<GetAllDocumentVersionsByDocumentResponse> _table;
        private int _totalItems;
        private string _searchString = "";
        private bool _dense = false;
        private bool _striped = true;
        private bool _bordered = false;

        private ClaimsPrincipal _currentUser;
        private bool _canEditDocuments;
        private bool _canSearchDocumentVersions;
        private bool _canDeleteDocumentVersions;
        private bool _loaded;

        public void Close()
        {
            MudDialog.Close();
        }

        protected override async Task OnInitializedAsync()
        {
            _currentUser = await _authenticationManager.CurrentUser();
            _canEditDocuments = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Documents.Edit)).Succeeded;
            _canDeleteDocumentVersions = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.DocumentVersions.Delete)).Succeeded;
            _canSearchDocumentVersions = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.DocumentVersions.Search)).Succeeded;
            _loaded = true;
        }

        private async Task<TableData<GetAllDocumentVersionsByDocumentResponse>> ServerReload(TableState state)
        {
            if (!string.IsNullOrWhiteSpace(_searchString))
            {
                state.Page = 0;
            }
            await LoadDataAsync(state.Page, state.PageSize, state);
            return new TableData<GetAllDocumentVersionsByDocumentResponse> { TotalItems = _totalItems, Items = _pagedData };
        }

        private async Task LoadDataAsync(int pageNumber, int pageSize, TableState state)
        {
            string ordering = string.Empty;
            if (!string.IsNullOrEmpty(state.SortLabel))
            {
                ordering = (state.SortDirection != SortDirection.None) ? $"{state.SortLabel} {state.SortDirection}" : $"{state.SortLabel}";
            }

            var request = new GetAllDocumentVersionsByDocumentQuery(DocumentId, pageNumber, pageSize, _searchString, ordering);
            var response = await DocumentVersionManager.GetAllPagedByDocumentAsync(request);
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
                var response = await DocumentVersionManager.DeleteAsync(id);
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

        private async Task Restore(Guid id)
        {
            string restoreContent = _localizer["Restore Document Version {0}"];
            var parameters = new DialogParameters
            {
                {nameof(Shared.Dialogs.RestoreConfirmation.ContentText), string.Format(restoreContent, id)}
            };
            var options = new DialogOptions
            {
                CloseButton = true,
                MaxWidth = MaxWidth.Small,
                FullWidth = true,
                DisableBackdropClick = true
            };
            var dialog = _dialogService.Show<Shared.Dialogs.RestoreConfirmation>(_localizer["Restoration"], parameters, options);
            var result = await dialog.Result;
            if (!result.Cancelled)
            {
                var command = new RestoreDocumentVersionCommand() { DocumentId = DocumentId, DocumentVersionId = id };
                var response = await DocumentManager.RestoreVersionAsync(command);
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
    }
}
