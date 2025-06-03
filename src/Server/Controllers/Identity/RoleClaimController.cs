using CleanArchitecture.Application.Interfaces.Services.Identity;
using CleanArchitecture.Application.Requests.Identity;
using CleanArchitecture.Shared.Constants.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CleanArchitecture.Server.Controllers
{
    [Route("api/identity/roleClaim")]
    [ApiController]
    public class RoleClaimController : ControllerBase
    {
        private readonly IRoleClaimService _roleClaimService;

        public RoleClaimController(IRoleClaimService roleClaimService)
        {
            _roleClaimService = roleClaimService;
        }

        /// <summary>
        /// Obtenir toutes les revendications de rôle (par exemple, Document Create Permission)
        /// </summary>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.RoleClaims.View)]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var roleClaims = await _roleClaimService.GetAllAsync();
            return Ok(roleClaims);
        }

        /// <summary>
        /// Obtenir toutes les revendications du rôle par son identifiant
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.RoleClaims.View)]
        [HttpGet("{roleId}")]
        public async Task<IActionResult> GetAllByRoleId([FromRoute] string roleId)
        {
            var response = await _roleClaimService.GetAllByRoleIdAsync(roleId);
            return Ok(response);
        }

        /// <summary>
        /// Ajouter une revendication de rôle
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Status 200 OK </returns>
        [Authorize(Policy = Permissions.RoleClaims.Create)]
        [HttpPost]
        public async Task<IActionResult> Post(RoleClaimRequest request)
        {
            var response = await _roleClaimService.SaveAsync(request);
            return Ok(response);
        }

        /// <summary>
        /// Supprimer une revendication de rôle par son identifiant
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.RoleClaims.Delete)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _roleClaimService.DeleteAsync(id);
            return Ok(response);
        }
    }
}