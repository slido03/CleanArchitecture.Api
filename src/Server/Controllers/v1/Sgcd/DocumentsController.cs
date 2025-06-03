using CleanArchitecture.Application.Features.Documents.Commands.AddEdit;
using CleanArchitecture.Application.Features.Documents.Commands.Delete;
using CleanArchitecture.Application.Features.Documents.Commands.RestoreVersion;
using CleanArchitecture.Application.Features.Documents.Queries.GetAll;
using CleanArchitecture.Application.Features.Documents.Queries.GetAllByDocumentType;
using CleanArchitecture.Application.Features.Documents.Queries.GetByExternalId;
using CleanArchitecture.Application.Features.Documents.Queries.GetById;
using CleanArchitecture.Application.Features.Documents.Queries.GetCount;
using CleanArchitecture.Application.Features.Documents.Queries.GetCountByDocumentType;
using CleanArchitecture.Shared.Constants.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace CleanArchitecture.Server.Controllers.v1.Sgcd
{
    [ApiController]
    public class DocumentsController : BaseApiController<DocumentsController>
    {
        #region GetAllDocumentsDocumentation
        /// <summary>
        /// Obtenir tous les documents de l'utlisateur ou publics sous forme de liste paginée.
        /// </summary>
        /// <param name="pageNumber">Numéro de page</param>
        /// <param name="pageSize">Taille de de la page</param>
        /// <param name="searchString">Critère de recherche</param>
        /// <param name="orderBy">Critère de tri (de la forme : attribut [ascending|descending],attribut [ascending|descending],...).</param>
        /// <returns>Une liste des documents de l'utlisateur ou publics paginés.</returns>
        /// <remarks>
        /// Exemples de requête :
        ///     <br/>
        ///     <para>
        ///         <b>
        ///             GET /api/v1/documents 
        ///         </b> 
        ///         <br/><br/><i>Retourne la liste complète des documents de l'utlisateur ou publics.</i><br/>
        ///     </para> 
        ///     <para>
        ///         <b>
        ///             GET /api/v1/documents?pageNumber=3&amp;pageSize=10 
        ///         </b>
        ///         <br/><br/><i>Retourne les 10 documents de l'utlisateur ou publics à la page 3.</i><br/>
        ///     </para>    
        ///     <para>
        ///         <b>
        ///             GET /api/v1/documents?searchString=etaf&amp;orderBy=title%20descending 
        ///         </b>
        ///         <br/><br/><i>Retourne tous les documents de l'utlisateur ou publics contenant le critère de recherche "etaf" et triés par titre dans l'ordre décroissant.</i><br/>
        ///     </para>
        ///     <para>
        ///         <b>
        ///             GET /api/v1/documents?orderBy=id,title%20ascending
        ///         </b>
        ///         <br/><br/><i>Retourne tous les documents de l'utlisateur ou publics triés par id et titre dans l'odre croissant.</i><br/>
        ///     </para>
        /// </remarks>
        /// <response code="200"> Retourne la liste des documents de l'utlisateur ou publics paginés obtenue.</response>
        /// <response code="500"> Si le critère de tri spécifié n'est pas conforme au modèle de données.</response>
        #endregion GetAllDocumentsDocumentation
        [Authorize(Policy = Permissions.Documents.View)]
        [HttpGet]
        public async Task<IActionResult> GetAll(int pageNumber, int pageSize, string searchString = "", string orderBy = "")
        {
            var docs = await _mediator.Send(new GetAllDocumentsQuery(pageNumber, pageSize, searchString, orderBy));
            return Ok(docs);
        }

        #region GetAllDocumentsByDocumentTypeDocumentation
        /// <summary>
        /// Obtenir tous les documents de l'utlisateur ou publics par type de document, sous forme de liste paginée.
        /// </summary>
        /// <param name="documenttypeid">Identifiant du type de document</param>
        /// <param name="pageNumber">Numéro de page</param>
        /// <param name="pageSize">Taille de de la page</param>
        /// <param name="searchString">Critère de recherche</param>
        /// <param name="orderBy">Critère de tri (de la forme : attribut [ascending|descending],attribut [ascending|descending],...).</param>
        /// <returns>Une liste des documents de l'utlisateur ou publics paginés.</returns>
        /// <remarks>
        /// Exemples de requête :
        ///     <br/>
        ///     <para>
        ///         <b>
        ///             GET /api/v1/documents/by-documentType/1 
        ///         </b> 
        ///         <br/><br/><i>Retourne la liste complète des documents de l'utlisateur ou publics, du type de document ayant l'identifiant 1.</i><br/>
        ///     </para> 
        ///     <para>
        ///         <b>
        ///             GET /api/v1/documents/by-documentType/1?pageNumber=3&amp;pageSize=10 
        ///         </b>
        ///         <br/><br/><i>Retourne les 10 documents de l'utlisateur ou publics, du type de document 1 à la page 3.</i><br/>
        ///     </para>    
        ///     <para>
        ///         <b>
        ///             GET /api/v1/documents/by-documentType/1?searchString=etaf&amp;orderBy=title%20descending
        ///         </b>
        ///         <br/><br/><i>Retourne tous les documents de l'utlisateur ou publics, du type de document 1, contenant le critère de recherche "etaf" et triés par titre dans l'ordre décroissant.</i><br/>
        ///     </para>
        ///     <para>
        ///         <b>
        ///             GET /api/v1/documents/by-documentType/1?orderBy=id,title%20ascending
        ///         </b>
        ///         <br/><br/><i>Retourne tous les documents de l'utlisateur ou publics, du type de document 1, triés par id et titre dans l'odre croissant.</i><br/>
        ///     </para>
        /// </remarks>
        /// <response code="200"> Retourne la liste des documents de l'utlisateur ou publics paginés, obtenue pour le type de document spécifié.</response>
        /// <response code="500"> Si le critère de tri spécifié n'est pas conforme au modèle de données.</response>
        #endregion GetAllDocumentsByDocumentTypeDocumentation
        [Authorize(Policy = Permissions.Documents.View)]
        [HttpGet("by-documentType/{documenttypeid}")]
        public async Task<IActionResult> GetAllByDocumentType(int documenttypeid, int pageNumber, int pageSize, string searchString = "", string orderBy = "")
        {
            var docs = await _mediator.Send(new GetAllDocumentsByDocumentTypeQuery(documenttypeid, pageNumber, pageSize, searchString, orderBy));
            return Ok(docs);
        }

        #region GetDocumentByIdDocumentation
        /// <summary>
        /// Obtenir un document à partir de son identifiant.
        /// </summary>
        /// <param name="id">Identifiant du document</param>
        /// <returns>Un document portant l'id spécifié.</returns>
        /// <remarks>
        /// Exemple de requête :
        ///     <br/>
        ///     <para>
        ///         <b>
        ///             GET /api/v1/documents/7c9e6679-7425-40de-944b-e07fc1f90ae7
        ///         </b>
        ///         <br/><br/><i>Retourne un document portant l'identifiant 7c9e6679-7425-40de-944b-e07fc1f90ae7.</i><br/>
        ///     </para>
        /// </remarks>
        /// <response code="200">Retourne le document portant l'identifiant spécifié.</response>
        #endregion GetDocumentByIdDocumentation
        [Authorize(Policy = Permissions.Documents.View)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var document = await _mediator.Send(new GetDocumentByIdQuery(id));
            return Ok(document);
        }

        #region GetDocumentByExternalIdDocumentation
        /// <summary>
        /// Obtenir un document à partir de son type de document et de son identifiant dans une application externe. 
        /// </summary>
        /// <param name="documenttype">Type du document</param>
        /// <param name="externalid">Identifiant dans l'application externe</param>
        /// <returns>Un document enregistré avec les informations spécifiées.</returns>
        /// <remarks>
        /// Exemple de requête :
        ///     <br/>
        ///     <para>
        ///         <b>
        ///             GET /api/v1/documents/documentType/etafi/by-externalId/138
        ///         </b>
        ///         <br/><br/><i>Retourne un document de type "etafi" portant l'identifiant externe 138.</i><br/>
        ///     </para>
        /// </remarks>
        /// <response code="200">Retourne le document enregistré avec les informations spécifiées.</response>
         #endregion GetDocumentByExternalIdDocumentation
        [Authorize(Policy = Permissions.Documents.View)]
        [HttpGet("documentType/{documenttype}/by-externalId/{externalid}")]
        public async Task<IActionResult> GetByExternalId(string documenttype, string externalid)
        {
            var document = await _mediator.Send(new GetDocumentByExternalIdQuery(documenttype, externalid));
            return Ok(document);
        }

        #region GetDocumentCountDocumentation
        /// <summary>
        /// Compter le nombre de documents enregistrés.
        /// </summary>
        /// <returns>Le nombre total de documents enregistrés.</returns>
        /// <remarks>
        /// Exemple de requête :
        ///     <br/>
        ///     <para>
        ///         <b>
        ///             GET /api/v1/documents/count
        ///         </b>
        ///         <br/><br/><i>Retourne le nombre total de documents enregistrés.</i><br/>
        ///     </para>
        /// </remarks>
        /// <response code="200">Retourne le nombre total de documents enregistrés.</response>
        #endregion GetDocumentCountDocumentation
        [Authorize(Policy = Permissions.Documents.View)]
        [HttpGet("count")]
        public async Task<IActionResult> GetCount()
        {
            var documentCount = await _mediator.Send(new GetDocumentCountQuery());
            return Ok(documentCount);
        }

        #region GetDocumentCountByDocumentTypeDocumentation
        /// <summary>
        /// Compter le nombre de documents enregistrés par type de document.
        /// </summary>
        /// <param name="documenttypeid">Identifiant du type de document</param>
        /// <returns>Le nombre total de documents enregistrés pour le type de document spécifié.</returns>
        /// <remarks>
        /// Exemple de requête :
        ///     <br/>
        ///     <para>
        ///         <b>
        ///             GET /api/v1/documents/count/by-documentType/1
        ///         </b>
        ///         <br/><br/><i>Retourne le nombre total de documents enregistrés pour le type de document 1.</i><br/>
        ///     </para>
        /// </remarks>
        /// <response code="200">Retourne le nombre total de documents enregistrés pour le type de document spécifié..</response>
        #endregion GetDocumentCountByDocumentTypeDocumentation
        [Authorize(Policy = Permissions.Documents.View)]
        [HttpGet("count/by-documentType/{documenttypeid}")]
        public async Task<IActionResult> GetCountByDocumentTypeId(int documenttypeid)
        {
            var documentCount = await _mediator.Send(new GetDocumentCountByDocumentTypeQuery(documenttypeid));
            return Ok(documentCount);
        }

        #region AddEditDocumentDocumentation
        /// <summary>
        /// Ajouter/Mettre à jour un document.
        /// </summary>
        /// <param name="command">Corps de la requête</param>
        /// <returns>L'id d'un nouveau document créé ou celui d'un document mis à jour.</returns>
        /// <remarks>
        /// Exemple de requête :
        ///     <br/>
        ///     <para>
        ///         <i>Création</i>
        ///     </para>
        ///     <br/>
        ///     <para>
        ///         <b>
        ///             POST /api/v1/documents
        ///                  {
        ///                      "Title": "doc1",
        ///                      "Description": "état finançier du 01/08/2021",
        ///                      "URL": "C:\\documents\finances",
        ///                      "DocumentType": "etafi",
        ///                      "ExternalId": "128"
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
        ///             POST /api/v1/documents
        ///                 {
        ///                     "Id": "7c9e6679-7425-40de-944b-e07fc1f90ae7",
        ///                     "Title": "doc1",
        ///                     "Description": "état finançier du 01/08/2021",
        ///                     "URL": "Files\GUDEF\ETAFI\D-8c4e6679-7425-45de-944b-e07fc1f90ae7_v1",
        ///                     "DocumentType": "etafi",
        ///                     "ExternalId": "128"
        ///                 }
        ///         </b>
        ///     </para>
        ///     <br/>
        ///     <br/>
        ///     <para>
        ///         <b><i>Pour téléverser un document vous devez renseigner les champs suivants de l'UploadRequest : </i></b>
        ///     </para>
        ///     <br/>
        ///     <para>
        ///         <b>data :</b> Tableau d'octets contenant les données du documents.
        ///     </para>
        ///     <br/>
        ///     <para>
        ///         <b>uploadType :</b> énumération correspondant au type de fichier téléversé (<b>1</b> pour les documents).
        ///     </para>
        ///     <br/>
        ///     <para>
        ///         <b>extension :</b> extension du document téléversé (<b>.pdf</b>, <b>.docx</b>, <b>.xlsx</b>, <b>.pptx</b>, <b>.csv</b>).
        ///     </para>
        /// </remarks>
        /// <response code="200">Retourne l'identifiant du document créé ou mis à jour.</response>
        /// <response code="400">Si le corps de la requête est vide ou incomplet.</response>
        #endregion AddEditDocumentDocumentation
        [Authorize(Policy = Permissions.Documents.Create)]
        [HttpPost]
        public async Task<IActionResult> Post(AddEditDocumentCommand command)
        {
            return Ok(await _mediator.Send(command));
        }

        #region RestoreDocumentVersionDocumentation
        /// <summary>
        /// Restaurer une version de document.
        /// </summary>
        /// <param name="command">Corps de la requête</param>
        /// <returns>L'identifiant de la version de document restaurée.</returns>
        /// <remarks>
        /// Exemple de requête :
        ///     <br/>
        ///     <para>
        ///         <i>Restaurer la version portant l'id 9d9e4679-7685-40de-944b-a47fc1f90ae7, du document 7c9e6679-7425-40de-944b-e07fc1f90ae7</i>
        ///     </para>
        ///     <br/>
        ///     <para>
        ///         <b>
        ///             PUT /api/v1/documents/restoreVersion
        ///                  {
        ///                      "DocumentId": "7c9e6679-7425-40de-944b-e07fc1f90ae7",
        ///                      "DocumentVersionId": "9d9e4679-7685-40de-944b-a47fc1f90ae7"
        ///                  }
        ///         </b>
        ///     </para>
        /// </remarks>
        /// <response code="200">Retourne l'identifiant de la version de document restaurée.</response>
        /// <response code="400">Si le corps de la requête est vide ou incomplet.</response>
        #endregion RestoreDocumentVersionDocumentation
        [Authorize(Policy = Permissions.Documents.Edit)]
        [HttpPut("restoreVersion")]
        public async Task<IActionResult> RestoreVersion(RestoreDocumentVersionCommand command)
        {
            return Ok(await _mediator.Send(command));
        }

        #region DeleteDocumentDocumentation
        /// <summary>
        /// Supprimer un document.
        /// </summary>
        /// <param name="id">Identifiant du document.</param>
        /// <returns>L'identifiant du document supprimé.</returns>
        /// <remarks>
        /// Exemple de requête :
        ///     <br/>
        ///     <para>
        ///         <b>
        ///             DELETE /api/v1/documents/7c9e6679-7425-40de-944b-e07fc1f90ae7
        ///         </b>
        ///         <br/><br/><i>Retourne l'identifiant 7c9e6679-7425-40de-944b-e07fc1f90ae7 après suppression.</i><br/>
        ///     </para>
        /// </remarks>
        /// <response code="200">Retourne l'identifiant du document supprimé.</response>
         #endregion DeleteDocumentDocumentation
        [Authorize(Policy = Permissions.Documents.Delete)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            return Ok(await _mediator.Send(new DeleteDocumentCommand { Id = id }));
        }
    }
}