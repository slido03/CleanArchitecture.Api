using CleanArchitecture.Application.Features.DocumentVersions.Commands.Delete;
using CleanArchitecture.Application.Features.DocumentVersions.Queries.GetAll;
using CleanArchitecture.Application.Features.DocumentVersions.Queries.GetAllByDocument;
using CleanArchitecture.Application.Features.DocumentVersions.Queries.GetById;
using CleanArchitecture.Application.Features.DocumentVersions.Queries.GetCount;
using CleanArchitecture.Application.Features.DocumentVersions.Queries.GetCountByDocument;
using CleanArchitecture.Shared.Constants.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace CleanArchitecture.Server.Controllers.v1.Sgcd
{
    [ApiController]
    public class DocumentVersionsController : BaseApiController<DocumentVersionsController>
    {
        #region GetAllDocumentVersionsDocumentation
        /// <summary>
        /// Obtenir toutes les versions de document sous forme de liste paginée.
        /// </summary>
        /// <param name="pageNumber">Numéro de page</param>
        /// <param name="pageSize">Taille de de la page</param>
        /// <param name="searchString">Critère de recherche</param>
        /// <param name="orderBy">Critère de tri (de la forme : attribut [ascending|descending],attribut [ascending|descending],...).</param>
        /// <returns>Une liste de versions de document paginées.</returns>
        /// <remarks>
        /// Exemples de requête :
        ///     <br/>
        ///     <para>
        ///         <b>
        ///              GET /api/v1/documentVersions
        ///         </b> 
        ///         <br/><br/><i>Retourne la liste complète des versions de document.</i><br/>
        ///     </para> 
        ///     <para>
        ///         <b>
        ///             GET /api/v1/documentVersions?pageNumber=3&amp;pageSize=10
        ///         </b>
        ///         <br/><br/><i>Retourne les 10  versions de document à la page 3.</i><br/>
        ///     </para>    
        ///     <para>
        ///         <b>
        ///             GET /api/v1/documentVersions?searchString=doc1&amp;orderBy=versionNumber
        ///         </b>
        ///         <br/><br/><i>Retourne toutes les versions de document contenant le critère de recherche "doc1" et triées par numéro de version dans l'ordre croissant.</i><br/>
        ///     </para>
        /// </remarks>
        /// <response code="200"> Retourne la liste de versions de document paginées obtenue.</response>
        /// <response code="500"> Si le critère de tri spécifié n'est pas conforme au modèle de données.</response>
        #endregion GetAllDocumentVersionsDocumentation
        [Authorize(Policy = Permissions.DocumentVersions.View)]
        [HttpGet]
        public async Task<IActionResult> GetAll(int pageNumber, int pageSize, string searchString = "", string orderBy = "")
        {
            var documentVersions = await _mediator.Send(new GetAllDocumentVersionsQuery(pageNumber, pageSize, searchString, orderBy));
            return Ok(documentVersions);
        }

        #region GetAllDocumentVersionsByDocumentDocumentation
        /// <summary>
        /// Obtenir toutes les versions de document par document, sous forme de liste paginée.
        /// </summary>
        /// <param name="documentid">Identifiant du document</param>
        /// <param name="pageNumber">Numéro de page</param>
        /// <param name="pageSize">Taille de de la page</param>
        /// <param name="searchString">Critère de recherche</param>
        /// <param name="orderBy">Critère de tri (de la forme : attribut [ascending|descending],attribut [ascending|descending],...).</param>
        /// <returns>Une liste de versions de document paginées.</returns>
        /// <remarks>
        /// Exemples de requête :
        ///     <br/>
        ///     <para>
        ///         <b>
        ///              GET /api/v1/documentVersions/by-document/7c9e6679-7425-40de-944b-e07fc1f90ae7
        ///         </b> 
        ///         <br/><br/><i>Retourne la liste complète des versions du document portant l'identifiant 7c9e6679-7425-40de-944b-e07fc1f90ae7.</i><br/>
        ///     </para> 
        ///     <para>
        ///         <b>
        ///              GET /api/v1/documentVersions/by-document/7c9e6679-7425-40de-944b-e07fc1f90ae7?pageNumber=3&amp;pageSize=10
        ///         </b>
        ///         <br/><br/><i>Retourne les 10 versions du document 7c9e6679-7425-40de-944b-e07fc1f90ae7, à la page 3.</i><br/>
        ///     </para>    
        ///     <para>
        ///         <b>
        ///             GET /api/v1/documentVersions/by-document/7c9e6679-7425-40de-944b-e07fc1f90ae7?searchString=doc1&amp;orderBy=versionNumber
        ///         </b>
        ///         <br/><br/><i>Retourne toutes les versions du document 7c9e6679-7425-40de-944b-e07fc1f90ae7, contenant le critère de recherche "doc1" et triées par numéro de version dans l'ordre croissant.</i><br/>
        ///     </para>
        /// </remarks>
        /// <response code="200"> Retourne la liste de versions de document paginées obtenue pour le document spécifié.</response>
        /// <response code="500"> Si le critère de tri spécifié n'est pas conforme au modèle de données.</response>
        #endregion GetAllDocumentVersionsByDocumentDocumentation
        [Authorize(Policy = Permissions.DocumentVersions.View)]
        [HttpGet("by-document/{documentid}")]
        public async Task<IActionResult> GetAllByDocument(Guid documentid, int pageNumber, int pageSize, string searchString = "", string orderBy = "")
        {
            var documentVersions = await _mediator.Send(new GetAllDocumentVersionsByDocumentQuery(documentid, pageNumber, pageSize, searchString, orderBy));
            return Ok(documentVersions);
        }

        #region GetDocumentVersionByIdDocumentation
        /// <summary>
        /// Obtenir une version de document à partir de son identifiant.
        /// </summary>
        /// <param name="id">Identifiant de la version de document</param>
        /// <returns>Une version de document portant l'identifiant spécifié.</returns>
        /// <remarks>
        /// Exemple de requête :
        ///     <br/>
        ///     <para>
        ///         <b>
        ///             GET /api/v1/documentVersions/7c9e6679-7425-40de-944b-e07fc1f90ae7
        ///         </b>
        ///         <br/><br/><i>Retourne une version de document portant l'identifiant 7c9e6679-7425-40de-944b-e07fc1f90ae7.</i><br/>
        ///     </para> 
        /// </remarks>
        ///  <response code="200">Retourne la version de document portant l'identifiant spécifié.</response>
        #endregion GetDocumentVersionByIdDocumentation
        [Authorize(Policy = Permissions.DocumentVersions.View)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var documentVersion = await _mediator.Send(new GetDocumentVersionByIdQuery(id));
            return Ok(documentVersion);
        }

        #region GetDocumentVersionCountDocumentation
        /// <summary>
        /// Compter le nombre de versions de document enregistrées.
        /// </summary>
        /// <returns>Le nombre total de versions de document enregistrées.</returns>
        /// <remarks>
        /// Exemple de requête:
        ///     <br/>
        ///     <para>
        ///         <b>
        ///             GET /api/v1/documentVersions/count
        ///         </b>
        ///         <br/><br/><i>Retourne le nombre total de versions de document enregistrées.</i><br/>
        ///     </para>
        /// </remarks>
        /// <response code="200">Retourne le nombre total de versions de document enregistrées.</response>
        #endregion GetDocumentVersionCountDocumentation
        [Authorize(Policy = Permissions.DocumentVersions.View)]
        [HttpGet("count")]
        public async Task<IActionResult> GetCount()
        {
            var documentVersionCount = await _mediator.Send(new GetDocumentVersionCountQuery());
            return Ok(documentVersionCount);
        }

        #region GetDocumentVersionCountByDocumentDocumentation
        /// <summary>
        /// Compter le nombre de versions de document enregistrées par document.
        /// </summary>
        /// <param name="documentid">Identifiant du document</param>
        /// <returns>Le nombre total de versions de document enregistrées pour le document spécifié.</returns>
        /// <remarks>
        /// Exemple de requête:
        ///     <br/>
        ///     <para>
        ///         <b>
        ///             GET /api/v1/documentVersions/count/by-document/4d9e6679-7425-70de-942b-e07fc1f90ae7
        ///         </b>
        ///         <br/><br/><i>Retourne le nombre total de versions de document enregistrées pour le document 4d9e6679-7425-70de-942b-e07fc1f90ae7.</i><br/>
        ///     </para>
        /// </remarks>
        /// <response code="200">Retourne le nombre total de versions de document enregistrées pour le document spécifié.</response>
        #endregion GetDocumentVersionCountByDocumentDocumentation
        [Authorize(Policy = Permissions.DocumentVersions.View)]
        [HttpGet("count/by-document/{documentid}")]
        public async Task<IActionResult> GetCountByDocument(Guid documentid)
        {
            var documentVersionCount = await _mediator.Send(new GetDocumentVersionCountByDocumentQuery(documentid));
            return Ok(documentVersionCount);
        }

        #region DeleteDocumentVersionDocumentation
        /// <summary>
        /// Supprimer une version de document.
        /// </summary>
        /// <param name="id">Identifiant de la version de document.</param>
        /// <returns>L'identifiant de la version de document supprimée.</returns>
        /// <remarks>
        /// Exemple de requête:
        ///     <br/>
        ///     <para>
        ///         <b>
        ///             DELETE /api/v1/documentVersions/7c9e6679-7425-40de-944b-e07fc1f90ae7
        ///         </b>
        ///         <br/><br/><i>Retourne l'identifiant 7c9e6679-7425-40de-944b-e07fc1f90ae7 après suppression.</i><br/>
        ///     </para>
        /// </remarks>
        /// <response code="200">Retourne l'identifiant de la version de document supprimée.</response>
        #endregion DeleteDocumentVersionDocumentation
        [Authorize(Policy = Permissions.DocumentVersions.Delete)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            return Ok(await _mediator.Send(new DeleteDocumentVersionCommand { Id = id }));
        }
    }
}
