using CleanArchitecture.Application.Interfaces.Services.Identity;
using CleanArchitecture.Application.Requests.Identity;
using CleanArchitecture.Shared.Constants.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CleanArchitecture.Server.Controllers
{
    [Route("api/identity/role")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        /// <summary>
        /// Obtenir tous les rôles enregistrés (basic, admin etc.)
        /// </summary>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Roles.View)]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var roles = await _roleService.GetAllAsync();
            return Ok(roles);
        }

        /// <summary>
        /// Ajouter un rôle utilisateur
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Roles.Create)]
        [HttpPost]
        public async Task<IActionResult> Post(RoleRequest request)
        {
            var response = await _roleService.SaveAsync(request);
            return Ok(response);
        }

        /// <summary>
        ///Supprimer un rôle utilisateur
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Roles.Delete)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var response = await _roleService.DeleteAsync(id);
            return Ok(response);
        }

        /// <summary>
        /// Obtenir toutes les permissions par rôle utilisateur
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns>Status 200 Ok</returns>
        [Authorize(Policy = Permissions.RoleClaims.View)]
        [HttpGet("permissions/{roleId}")]
        public async Task<IActionResult> GetPermissionsByRoleId([FromRoute] string roleId)
        {
            var response = await _roleService.GetAllPermissionsAsync(roleId);
            return Ok(response);
        }

        /// <summary>
        /// Mettre à jour les revendications du rôle utilisateur
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(Policy = Permissions.RoleClaims.Edit)]
        [HttpPut("permissions/update")]
        public async Task<IActionResult> Update(PermissionRequest model)
        {
            var response = await _roleService.UpdatePermissionsAsync(model);
            return Ok(response);
        }
    }
}