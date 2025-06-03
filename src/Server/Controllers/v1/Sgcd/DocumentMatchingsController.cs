using CleanArchitecture.Application.Features.DocumentMatchings.Commands.AddEdit;
using CleanArchitecture.Application.Features.DocumentMatchings.Commands.Delete;
using CleanArchitecture.Application.Features.DocumentMatchings.Queries.GetAll;
using CleanArchitecture.Application.Features.DocumentMatchings.Queries.GetByCentralizedDocument;
using CleanArchitecture.Application.Features.DocumentMatchings.Queries.GetById;
using CleanArchitecture.Application.Features.DocumentMatchings.Queries.GetCount;
using CleanArchitecture.Shared.Constants.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace CleanArchitecture.Server.Controllers.v1.Sgcd
{
    [ApiController]
    public class DocumentMatchingsController : BaseApiController<DocumentMatchingsController>
    {
        #region GetAllDocumentMatchingsDocumentation
        /// <summary>
        /// Obtenir toutes les correspondances de documents sous forme de liste paginée.
        /// </summary>
        /// <param name="pageNumber">Numéro de page</param>
        /// <param name="pageSize">Taille de de la page</param>
        /// <param name="searchString">Critère de recherche</param>
        /// <param name="orderBy">Critère de tri (de la forme : attribut [ascending|descending],attribut [ascending|descending],...).</param>
        /// <returns>Une liste de correspondances de documents paginées.</returns>
        /// <remarks>
        /// Exemples de requête :
        ///     <br/>
        ///     <para>
        ///         <b>
        ///             GET /api/v1/documentMatchings
        ///         </b> 
        ///         <br/><br/><i>Retourne la liste complète des correspondances de documents.</i><br/>
        ///     </para> 
        ///     <para>
        ///         <b>
        ///             GET /api/v1/documentMatchings?pageNumber=3&amp;pageSize=10 
        ///         </b>
        ///         <br/><br/><i>Retourne les 10 correspondances de documents à la page 3.</i><br/>
        ///     </para>    
        ///     <para>
        ///         <b>
        ///             GET /api/v1/documentMatchings?searchString=doc1&amp;orderBy=externalId
        ///         </b>
        ///         <br/><br/><i>Retourne toutes les correspondances de documents contenant le critère de recherche "doc1" et triées par id externe dans l'ordre croissant.</i><br/>
        ///     </para>
        /// </remarks>
        /// <response code="200"> Retourne la liste des correspondances de documents paginées obtenue.</response>
        /// <response code="500"> Si le critère de tri spécifié n'est pas conforme au modèle de données.</response>
        #endregion GetAllDocumentMatchingsDocumentation
        [Authorize(Policy = Permissions.DocumentMatchings.View)]
        [HttpGet]
        public async Task<IActionResult> GetAll(int pageNumber, int pageSize, string searchString = "", string orderBy = "")
        {
            var documentMatchings = await _mediator.Send(new GetAllDocumentMatchingsQuery(pageNumber, pageSize, searchString, orderBy));
            return Ok(documentMatchings);
        }

        #region GetDocumentMatchingByIdDocumentation
        /// <summary>
        /// Obtenir une correspondance de documents à partir de son identifiant.
        /// </summary>
        /// <param name="id">Identifiant de la correspondance de documents</param>
        /// <returns>Une correspondance de documents portant l'identifiant spécifié.</returns>
        /// <remarks>
        ///  Exemple de requête :
        ///     <br/>
        ///     <para>
        ///         <b>
        ///             GET /api/v1/documentMatchings/7c9e6679-7425-40de-944b-e07fc1f90ae7
        ///         </b>
        ///         <br/><br/><i>Retourne une correspondance de documents portant l'identifiant 7c9e6679-7425-40de-944b-e07fc1f90ae7.</i><br/>
        ///     </para>
        /// </remarks>
        /// <response code="200">Retourne la correspondance de documents portant l'identifiant spécifié.</response>
        #endregion GetDocumentMatchingByIdDocumentation
        [Authorize(Policy = Permissions.DocumentMatchings.View)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var documentMatching = await _mediator.Send(new GetDocumentMatchingByIdQuery(id));
            return Ok(documentMatching);
        }

        #region GetDocumentMatchingByCentralizedDocumentDocumentation
        /// <summary>
        /// Obtenir une correspondance de documents à partir d'un document centralisé.
        /// </summary>
        /// <param name="centralizeddocumentid">Identifiant du  document centralisé</param>
        /// <returns>Une correspondance de documents correspondant au document centralisé spécifié.</returns>
        /// <remarks>
        /// Exemple de requête :
        ///     <br/>
        ///     <para>
        ///         <b>
        ///              GET /api/v1/documentMatchings/by-centralizedDocument/7c9e6679-7425-40de-944b-e07fc1f90ae7
        ///         </b>
        ///         <br/><br/><i>Retourne une correspondance de documents pour le document centralisé portant l'identifiant 7c9e6679-7425-40de-944b-e07fc1f90ae7.</i><br/>
        ///     </para>
        /// </remarks>
        /// <response code="200">Retourne la correspondance de documents correspondant au document centralisé spécifié.</response>
        #endregion GetDocumentMatchingByCentralizedDocumentDocumentation
        [Authorize(Policy = Permissions.DocumentMatchings.View)]
        [HttpGet("by-centralizedDocument/{centralizeddocumentid}")]
        public async Task<IActionResult> GetByCentralizedDocument(Guid centralizeddocumentid)
        {
            var documentMatching = await _mediator.Send(new GetDocumentMatchingByCentralizedDocumentQuery(centralizeddocumentid));
            return Ok(documentMatching);
        }

        #region GetDocumentMatchingCountDocumentation
        /// <summary>
        /// Compter le nombre de correspondances de documents enregistrées.
        /// </summary>
        /// <returns>Le nombre total de correspondances de documents enregistrées.</returns>
        /// <remarks>
        /// Exemple de requête :
        ///     <br/>
        ///     <para>
        ///         <b>
        ///             GET /api/v1/documentMatchings/count
        ///         </b>
        ///         <br/><br/><i>Retourne le nombre total de correspondances de documents enregistrées.</i><br/>
        ///     </para>
        /// </remarks>
        /// <response code="200">Retourne le nombre total de correspondances de documents enregistrées.</response>
        #endregion GetDocumentMatchingCountDocumentation
        [Authorize(Policy = Permissions.DocumentMatchings.View)]
        [HttpGet("count")]
        public async Task<IActionResult> GetCount()
        {
            var documentMatchingCount = await _mediator.Send(new GetDocumentMatchingCountQuery());
            return Ok(documentMatchingCount);
        }

        #region AddEditDocumentMatchingDocumentation
        /// <summary>
        /// Ajouter/Mettre à jour une correspondance de documents.
        /// </summary>
        /// <param name="command">Corps de la requête</param>
        /// <returns>L'id de la nouvelle correspondance de documents créée ou celui d'une correspondance de documents mise à jour.</returns>
        /// <remarks>
        /// Exemple de requête:
        ///     <br/>
        ///     <para>
        ///         <i>Création</i>
        ///     </para>
        ///     <br/>
        ///     <para>
        ///         <b>
        ///             POST /api/v1/documentMatchings
        ///                  {
        ///                      "ExternalId": "156",
        ///                      "CentralizedDocumentId": "7c9e6679-7425-40de-944b-e07fc1f90ae7"
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
        ///             POST /api/v1/documentMatchings
        ///                 {
        ///                     "Id": "7c4d6679-7425-38de-944b-e07fc1f40ae7"
        ///                     "ExternalId": "156",
        ///                     "CentralizedDocumentId": "7c9e6679-7425-40de-944b-e07fc1f90ae7"
        ///                 }
        ///         </b>
        ///     </para>
        /// </remarks>
        /// <response code="200">Retourne l'id de la correspondance de documents créée ou mise à jour.</response>
        /// <response code="400">Si le corps de la requête est vide ou incomplet.</response>
        #endregion AddEditDocumentMatchingDocumentation
        [Authorize(Policy = Permissions.DocumentMatchings.Create)]
        [HttpPost]
        public async Task<IActionResult> Post(AddEditDocumentMatchingCommand command)
        {
            return Ok(await _mediator.Send(command));
        }

        #region DeleteDocumentMatchingDocumentation
        /// <summary>
        /// Supprimer une correspondance de documents.
        /// </summary>
        /// <param name="id">Identifiant de la correspondance de documents.</param>
        /// <returns>L'identifiant de la correspondance de documents supprimée.</returns>
        /// <remarks>
        /// Exemple de requête:
        ///     <br/>
        ///     <para>
        ///         <b>
        ///             DELETE /api/v1/documentMatchings/7c9e6679-7425-40de-944b-e07fc1f90ae7
        ///         </b>
        ///         <br/><br/><i>Retourne l'identifiant 7c9e6679-7425-40de-944b-e07fc1f90ae7 après suppression.</i><br/>
        ///     </para>
        /// </remarks>
        /// <response code="200">L'identifiant de la correspondance de documents supprimée.</response>
        #endregion DeleteDocumentMatchingDocumentation
        [Authorize(Policy = Permissions.DocumentMatchings.Delete)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            return Ok(await _mediator.Send(new DeleteDocumentMatchingCommand { Id = id }));
        }
    }
}
