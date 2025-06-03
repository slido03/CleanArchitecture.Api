using CleanArchitecture.Application.Features.ExternalApplications.Commands.AddEdit;
using CleanArchitecture.Application.Features.ExternalApplications.Commands.Delete;
using CleanArchitecture.Application.Features.ExternalApplications.Queries.Export;
using CleanArchitecture.Application.Features.ExternalApplications.Queries.GetAll;
using CleanArchitecture.Application.Features.ExternalApplications.Queries.GetById;
using CleanArchitecture.Application.Features.ExternalApplications.Queries.GetCount;
using CleanArchitecture.Shared.Constants.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CleanArchitecture.Server.Controllers.v1.Sgcd
{
    [ApiController]
    public class ExternalApplicationsController : BaseApiController<ExternalApplicationsController>
    {
        #region ExportExternalApplicationsDocumentation
        /// <summary>
        /// Rechercher les applications externes et les exporter dans un fichier excel.
        /// </summary>
        /// <param name="searchString">Critère de recherche</param>
        /// <returns>Un fichier excel généré sous forme d'octets.</returns>
        /// <remarks>
        /// Exemples de requête :
        ///     <br/>
        ///     <para>
        ///         <b>
        ///             GET /api/v1/externalApplications/export
        ///         </b> 
        ///         <br/><br/><i>Retourne un fichier excel contenant toutes les applications externes, sous forme d'octets.</i><br/>
        ///     </para> 
        ///     <para>
        ///         <b>
        ///              GET /api/v1/externalApplications/export?searchString=gud
        ///         </b>
        ///         <br/><br/><i>Retourne un fichier excel contenant toute application contenant le critère de recherche "gud".</i><br/>
        ///     </para> 
        /// </remarks>
        /// <response code="200">Retourne le fichier Excel généré sous forme d'octets.</response>
        #endregion ExportExternalApplicationsDocumentation
        [Authorize(Policy = Permissions.ExternalApplications.Export)]
        [HttpGet("export")]
        public async Task<IActionResult> Export(string searchString = "")
        {
            return Ok(await _mediator.Send(new ExportExternalApplicationsQuery(searchString)));
        }

        #region GetAllExternalApplicationsDocumentation
        /// <summary>
        /// Obtenir toutes les applications externes sous forme de liste paginée.
        /// </summary>
        /// <param name="pageNumber">Numéro de page</param>
        /// <param name="pageSize">Taille de de la page</param>
        /// <param name="searchString">Critère de recherche</param>
        /// <param name="orderBy">Critère de tri (de la forme : attribut [ascending|descending],attribut [ascending|descending],...).</param>
        /// <returns>Une liste d'applications paginées.</returns>
        /// <remarks>
        /// Exemples de requête :
        ///     <br/>
        ///     <para>
        ///         <b>
        ///             GET /api/v1/externalApplications
        ///         </b> 
        ///         <br/><br/><i>Retourne la liste complète des applications.</i><br/>
        ///     </para> 
        ///     <para>
        ///         <b>
        ///             GET /api/v1/externalApplications?pageNumber=3&amp;pageSize=10
        ///         </b>
        ///         <br/><br/><i>Retourne les 10 applications à la page 3.</i><br/>
        ///     </para>    
        ///     <para>
        ///         <b>
        ///             GET /api/v1/externalApplications?searchString=gud&amp;orderBy=name%20descending
        ///         </b>
        ///         <br/><br/><i>Retourne toutes les applications contenant le critère de recherche "gud" et triées par nom dans l'ordre décroissant.</i><br/>
        ///     </para>
        ///     <para>
        ///         <b>
        ///             GET /api/v1/externalApplications?orderBy=id,name%20ascending
        ///         </b>
        ///         <br/><br/><i>Retourne toutes les applications triées par id et nom dans l'odre croissant.</i><br/>
        ///     </para>
        /// </remarks>
        /// <response code="200"> Retourne la liste d'applications paginées obtenue.</response>
        /// <response code="500"> Si le critère de tri spécifié n'est pas conforme au modèle de données.</response>
        #endregion GetAllExternalApplicationsDocumentation
        [Authorize(Policy = Permissions.ExternalApplications.View)]
        [HttpGet]
        public async Task<IActionResult> GetAll(int pageNumber, int pageSize, string searchString = "", string orderBy = "")
        {
            var externalApplications = await _mediator.Send(new GetAllExternalApplicationsQuery(pageNumber, pageSize, searchString, orderBy));
            return Ok(externalApplications);
        }

        #region GetExternalApplicationByIdDocumentation
        /// <summary>
        ///Obtenir une application à partir de son identifiant.
        /// </summary>
        /// <param name="id">Identifiant de l'application</param>
        /// <returns>Une Application portant l'identifiant spécifié.</returns>
        /// <remarks>
        /// Exemple de requête :
        ///     <br/>
        ///     <para>
        ///         <b>
        ///             GET /api/v1/externalApplications/1
        ///         </b>
        ///         <br/><br/><i>Retourne une application externe portant l'identifiant 1.</i><br/>
        ///     </para> 
        /// </remarks>
        /// <response code="200">Retourne l'application ayant l'identifiant spécifié.</response>
        #endregion GetExternalApplicationByIdDocumentation
        [Authorize(Policy = Permissions.ExternalApplications.View)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var externalApplication = await _mediator.Send(new GetExternalApplicationByIdQuery(id));
            return Ok(externalApplication);
        }

        #region GetExternalApplicationCountDocumentation
        /// <summary>
        /// Compter le nombre d'applications externes enregistrées.
        /// </summary>
        /// <returns>Le nombre total d'applications enregistrées.</returns>
        /// <remarks>
        /// Exemple de requête :
        ///     <br/>
        ///     <para>
        ///         <b>
        ///             GET /api/v1/externalApplications/count
        ///         </b>
        ///         <br/><br/><i>Retourne le nombre total d'applications enregistrées.</i><br/>
        ///     </para>
        /// </remarks>
        /// <response code="200">Retourne le nombre total d'applications.</response>
        #endregion GetExternalApplicationCountDocumentation
        [Authorize(Policy = Permissions.ExternalApplications.View)]
        [HttpGet("count")]
        public async Task<IActionResult> GetCount()
        {
            var externalApplicationCount = await _mediator.Send(new GetExternalApplicationCountQuery());
            return Ok(externalApplicationCount);
        }

        #region AddEditExternalApplicationDocumentation
        /// <summary>
        /// Enregistrer/Mettre à jour une application externe.
        /// </summary>
        /// <param name="command">Corps de la requête</param>
        /// <returns>L'id d'une nouvelle application créée ou celui d'une application mise à jour.</returns>
        /// <remarks>
        /// Exemples de requête :
        ///     <br/>
        ///     <para>
        ///         <i>Création</i>
        ///     </para>
        ///     <br/>
        ///     <para>
        ///         <b>
        ///             POST /api/v1/externalApplications
        ///                  {
        ///                      "Name": "gudef",
        ///                      "Description": "Guichet Unique des Etats Finançiers"
        ///                  }
        ///         </b>
        ///     </para>
        ///     <br/> 
        ///     <para>
        ///         <i>Mise à jour</i>
        ///     </para>
        ///     <br/>
        ///     <para>
        ///         <b>
        ///             POST /api/v1/externalApplications
        ///                 {
        ///                     "Id": 1,
        ///                     "Name": "gudef",
        ///                     "Description": "Guichet Unique des Etats Finançiers"
        ///                 }
        ///         </b>
        ///     </para>
        /// </remarks>
        /// <response code="200">Retourne l'identifiant de l'application créée ou mise à jour.</response>
        /// <response code="400">Si le corps de la requête est vide ou incomplet.</response>
        #endregion AddEditExternalApplicationDocumentation
        [Authorize(Policy = Permissions.ExternalApplications.Create)]
        [HttpPost]
        public async Task<IActionResult> Post(AddEditExternalApplicationCommand command)
        {
            return Ok(await _mediator.Send(command));
        }

        #region DeleteExternalApplicationDocumentation
        /// <summary>
        /// Supprimer une application externe.
        /// </summary>
        /// <param name="id">Identifiant de l'application.</param>
        /// <returns>L'identifiant de l'application supprimée.</returns>
        /// <remarks>
        /// Exemple de requête:
        ///     <br/>
        ///     <para>
        ///         <b>
        ///             DELETE /api/v1/externalApplications/1
        ///         </b>
        ///         <br/><br/><i>Retourne l'identifiant 1 après suppression.</i><br/>
        ///     </para>
        /// </remarks>
        /// <response code="200">Retourne l'identifiant de l'application supprimée.</response>
        #endregion DeleteExternalApplicationDocumentation
        [Authorize(Policy = Permissions.ExternalApplications.Delete)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await _mediator.Send(new DeleteExternalApplicationCommand { Id = id }));
        }
    }
}
