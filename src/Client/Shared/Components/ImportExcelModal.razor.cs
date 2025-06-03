using Blazored.FluentValidation;
using CleanArchitecture.Application.Requests;
using CleanArchitecture.Shared.Wrapper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using System;
using System.IO;
using System.Threading.Tasks;

namespace CleanArchitecture.Client.Shared.Components
{
    public partial class ImportExcelModal
    {
        private IBrowserFile _file;
        [CascadingParameter] private MudDialogInstance MudDialog { get; set; }
        [Parameter] public UploadRequest UploadRequest { get; set; } = new();
        [Parameter] public string ModelName { get; set; }
        [Parameter] public Func<UploadRequest, Task<IResult<int>>> OnSaved { get; set; }

        private FluentValidationValidator _fluentValidationValidator;
        private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });
        private bool _uploading = false;
        public void Cancel()
        {
            MudDialog.Cancel();
        }

        private async Task SaveAsync()
        {
            if (OnSaved != null)
            {
                _uploading = true;
                var result = await OnSaved.Invoke(UploadRequest);
                if (result.Succeeded)
                {
                    _snackBar.Add(result.Messages[0], Severity.Success);
                    MudDialog.Close();
                }
                else
                {
                    foreach (var message in result.Messages)
                    {
                        _snackBar.Add(message, Severity.Error);
                    }
                }
                _uploading = false;
            }
            else
            {
                MudDialog.Close();
            }

            await Task.CompletedTask;
        }

        private async Task UploadFiles(InputFileChangeEventArgs e)
        {
            _file = e.File;
            if (_file != null)
            {
                var buffer = new byte[_file.Size];
                var extension = Path.GetExtension(_file.Name);
                await _file.OpenReadStream(_file.Size).ReadAsync(buffer);
                UploadRequest = new UploadRequest
                {
                    Data = buffer,
                    FileName = _file.Name,
                    UploadType = Application.Enums.UploadType.Document,
                    Extension = extension
                };
            }
        }

        protected override async Task OnInitializedAsync()
        {
            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            await Task.CompletedTask;
        }
    }
}