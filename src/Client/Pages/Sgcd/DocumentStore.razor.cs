using CleanArchitecture.Application.Features.Documents.Commands.AddEdit;
using CleanArchitecture.Application.Features.Documents.Queries.GetAll;
using CleanArchitecture.Client.Extensions;
using CleanArchitecture.Client.Infrastructure.Managers.Sgcd.Document;
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
    public partial class DocumentStore
    {
        [Inject] private IDocumentManager DocumentManager { get; set; }

        private IEnumerable<GetAllDocumentsResponse> _pagedData;
        private MudTable<GetAllDocumentsResponse> _table;
        private int _totalItems;
        private string _searchString = "";
        private bool _dense = false;
        private bool _striped = true;
        private bool _bordered = false;

        private ClaimsPrincipal _currentUser;
        private bool _canCreateDocuments;
        private bool _canEditDocuments;
        private bool _canDeleteDocuments;
        private bool _canSearchDocuments;
        private bool _canViewDocumentVersions;
        private bool _loaded;
        private string _currentUserId;

        protected override async Task OnInitializedAsync()
        {
            _currentUser = await _authenticationManager.CurrentUser();
            _canCreateDocuments = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Documents.Create)).Succeeded;
            _canEditDocuments = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Documents.Edit)).Succeeded;
            _canDeleteDocuments = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Documents.Delete)).Succeeded;
            _canSearchDocuments = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Documents.Search)).Succeeded;
            _canViewDocumentVersions = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.DocumentVersions.View)).Succeeded;
            _loaded = true;

            var state = await _stateProvider.GetAuthenticationStateAsync();
            var user = state.User;
            if (user == null) return;
            if (user.Identity?.IsAuthenticated == true)
            {
                _currentUserId = user.GetUserId();
            }
        }

        private async Task<TableData<GetAllDocumentsResponse>> ServerReload(TableState state)
        {
            if (!string.IsNullOrWhiteSpace(_searchString))
            {
                state.Page = 0;
            }
            await LoadData(state.Page, state.PageSize, state);
            return new TableData<GetAllDocumentsResponse> { TotalItems = _totalItems, Items = _pagedData };
        }

        private async Task LoadData(int pageNumber, int pageSize, TableState state)
        {
            string ordering = string.Empty;
            if (!string.IsNullOrEmpty(state.SortLabel))
            {
                ordering = (state.SortDirection != SortDirection.None) ? $"{state.SortLabel} {state.SortDirection}" : $"{state.SortLabel}";
            }

            var request = new GetAllDocumentsQuery(pageNumber, pageSize, _searchString, ordering);
            var response = await DocumentManager.GetAllPagedAsync(request);
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

        private string GetOwner(string userId)
        {
            if (_currentUserId == userId)
                return _localizer["You"];
            else
                return _localizer["Other"];
        }

        private async Task InvokeAddEditModal(Guid id)
        {
            var parameters = new DialogParameters();
            if (id != Guid.Empty)
            {
                var doc = _pagedData.FirstOrDefault(c => c.Id == id);
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
                OnSearch("");
            }
        }

        private async Task InvokeGetByExternalIdModal()
        {
            var options = new DialogOptions
            {
                CloseButton = true,
                MaxWidth = MaxWidth.Large,
                FullWidth = true,
                DisableBackdropClick = true
            };
            var dialog = _dialogService.Show<GetDocumentByExternalIdModal>(_localizer["Search for a specific document"], options);
            var result = await dialog.Result;
            if (!result.Cancelled)
            {
                OnSearch("");
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
                OnSearch("");
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