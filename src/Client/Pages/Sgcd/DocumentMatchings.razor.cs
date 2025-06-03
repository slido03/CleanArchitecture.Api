using CleanArchitecture.Application.Features.DocumentMatchings.Queries.GetAll;
using CleanArchitecture.Client.Infrastructure.Managers.Sgcd.DocumentMatching;
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
    public partial class DocumentMatchings
    {
        [Inject] private IDocumentMatchingManager DocumentMatchingManager { get; set; }

        private IEnumerable<GetAllDocumentMatchingsResponse> _pagedData;
        private MudTable<GetAllDocumentMatchingsResponse> _table;
        private int _totalItems;
        private string _searchString = "";
        private bool _dense = false;
        private bool _striped = true;
        private bool _bordered = false;

        private ClaimsPrincipal _currentUser;
        private bool _canDeleteDocumentMatchings;
        private bool _canSearchDocumentMatchings;
        private bool _loaded;

        protected override async Task OnInitializedAsync()
        {
            _currentUser = await _authenticationManager.CurrentUser();
            _canDeleteDocumentMatchings = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.DocumentMatchings.Delete)).Succeeded;
            _canSearchDocumentMatchings = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.DocumentMatchings.Search)).Succeeded;
            _loaded = true;
        }

        private async Task<TableData<GetAllDocumentMatchingsResponse>> ServerReload(TableState state)
        {
            if (!string.IsNullOrWhiteSpace(_searchString))
            {
                state.Page = 0;
            }
            await LoadData(state.Page, state.PageSize, state);
            return new TableData<GetAllDocumentMatchingsResponse> { TotalItems = _totalItems, Items = _pagedData };
        }

        private async Task LoadData(int pageNumber, int pageSize, TableState state)
        {
            string ordering = string.Empty;
            if (!string.IsNullOrEmpty(state.SortLabel))
            {
                ordering = (state.SortDirection != SortDirection.None) ? $"{state.SortLabel} {state.SortDirection}" : $"{state.SortLabel}";
            }

            var request = new GetAllDocumentMatchingsQuery(pageNumber, pageSize, _searchString, ordering);
            var response = await DocumentMatchingManager.GetAllPagedAsync(request);
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
            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, DisableBackdropClick = true };
            var dialog = _dialogService.Show<Shared.Dialogs.DeleteConfirmation>(_localizer["Deletion"], parameters, options);
            var result = await dialog.Result;
            if (!result.Cancelled)
            {
                var response = await DocumentMatchingManager.DeleteAsync(id);
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
