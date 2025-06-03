using CleanArchitecture.Application.Features.Dashboards.Queries.GetData;
using CleanArchitecture.Shared.Constants.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CleanArchitecture.Server.Controllers.v1
{
    [ApiController]
    public class DashboardController : BaseApiController<DashboardController>
    {
        #region GetDashboardDataDocumentation
        /// <summary>
        /// Obtenir toutes les données du tableau de bord.
        /// </summary>
        /// <returns>Toutes les données du tableau de bord.</returns>
        #endregion GetDashboardDataDocumentation
        [Authorize(Policy = Permissions.Dashboards.View)]
        [HttpGet]
        public async Task<IActionResult> GetDataAsync()
        {
            var result = await _mediator.Send(new GetDashboardDataQuery());
            return Ok(result);
        }
    }
}