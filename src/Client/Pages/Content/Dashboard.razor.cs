using CleanArchitecture.Client.Extensions;
using CleanArchitecture.Client.Infrastructure.Managers.Dashboard;
using CleanArchitecture.Shared.Constants.Application;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace CleanArchitecture.Client.Pages.Content
{
    public partial class Dashboard
    {
        [Inject] private IDashboardManager DashboardManager { get; set; }

        [CascadingParameter] private HubConnection HubConnection { get; set; }
        [Parameter] public int ExternalApplicationCount { get; set; }
        [Parameter] public int DocumentTypeCount { get; set; }
        [Parameter] public long DocumentCount { get; set; }
        [Parameter] public long DocumentMatchingCount { get; set; }
        [Parameter] public long DocumentVersionCount { get; set; }
        [Parameter] public int UserCount { get; set; }
        [Parameter] public int RoleCount { get; set; }

        private readonly string[] _dataEnterBarChartXAxisLabels = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
        private readonly List<ChartSeries> _dataEnterBarChartSeries = new();
        private string[] _pieChartLabels;
        private double[] _pieChartData;
        private bool _loaded;

        protected override async Task OnInitializedAsync()
        {
            await LoadDataAsync();
            _loaded = true;
            HubConnection = HubConnection.TryInitialize(_navigationManager, _localStorage);
            HubConnection.On(ApplicationConstants.SignalR.ReceiveUpdateDashboard, async () =>
            {
                await LoadDataAsync();
                StateHasChanged();
            });
            if (HubConnection.State == HubConnectionState.Disconnected)
            {
                await HubConnection.StartAsync();
            }
        }

        private async Task LoadDataAsync()
        {
            var response = await DashboardManager.GetDataAsync();
            if (response.Succeeded)
            {
                ExternalApplicationCount = response.Data.ExternalApplicationCount;
                DocumentTypeCount = response.Data.DocumentTypeCount;
                DocumentCount = response.Data.DocumentCount;
                DocumentVersionCount = response.Data.DocumentVersionCount;
                DocumentMatchingCount = response.Data.DocumentMatchingCount;
                UserCount = response.Data.UserCount;
                RoleCount = response.Data.RoleCount;
                foreach (var item in response.Data.DataEnterBarChart)
                {
                    _dataEnterBarChartSeries
                        .RemoveAll(x => x.Name.Equals(item.Name, StringComparison.OrdinalIgnoreCase));
                    _dataEnterBarChartSeries.Add(new ChartSeries { Name = item.Name, Data = item.Data });
                }

                var documentsByDocumentTypePieChart = response.Data.DocumentsByDocumentTypePieChart;
                _pieChartLabels = new string[documentsByDocumentTypePieChart.Count];
                _pieChartData = new double[documentsByDocumentTypePieChart.Count];
                documentsByDocumentTypePieChart.Keys.CopyTo(_pieChartLabels, 0);
                for (int i = 0; i < _pieChartLabels.Length; i++)
                {
                    _pieChartData[i] = documentsByDocumentTypePieChart[_pieChartLabels[i]];
                }
            }
            else
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
        }
    }
}