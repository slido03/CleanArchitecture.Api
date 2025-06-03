using CleanArchitecture.Application.Features.DocumentTypes.Commands.AddEdit;
using CleanArchitecture.Application.Features.DocumentTypes.Commands.Delete;
using CleanArchitecture.Application.Features.DocumentTypes.Queries.Export;
using CleanArchitecture.Application.Features.DocumentTypes.Queries.GetAll;
using CleanArchitecture.Application.Features.DocumentTypes.Queries.GetAllByExternalApplication;
using CleanArchitecture.Application.Features.DocumentTypes.Queries.GetById;
using CleanArchitecture.Application.Features.DocumentTypes.Queries.GetCount;
using CleanArchitecture.Application.Features.DocumentTypes.Queries.GetCountByExternalApplication;
using CleanArchitecture.Shared.Constants.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CleanArchitecture.Server.Controllers.v1.Sgcd
{
    [ApiController]
    public class DocumentTypesController : BaseApiController<DocumentTypesController>
    {
        #region ExportDocumentTypesDocumentation
        /// <summary>
        /// Rechercher les types de document et les exporter dans un fichier excel.
        /// </summary>
        /// <param name="searchString">Critère de recherche</param>
        /// <returns>Un fichier excel généré sous forme d'octets.</returns>
        /// <remarks>
        /// Exemples de requête :
        ///     <br/>
        ///     <para>
        ///         <b>
        ///             GET /api/v1/documentTypes/export 
        ///         </b> 
        ///         <br/><br/><i>Retourne un fichier excel contenant tous les types de document, sous forme d'octets.</i><br/>
        ///     </para> 
        ///     <para>
        ///         <b>
        ///             GET /api/v1/documentTypes/export?searchString=etaf
        ///         </b>
        ///         <br/><br/><i>Retourne un fichier excel contenant tout type de document contenant le critère de recherche "etaf".</i><br/>
        ///     </para> 
        /// </remarks>
        /// <response code="200">Retourne le fichier Excel généré sous forme d'octets.</response>
        #endregion ExportDocumentTypesDocumentation
        [Authorize(Policy = Permissions.DocumentTypes.Export)]
        [HttpGet("export")]
        public async Task<IActionResult> Export(string searchString = "")
        {
            return Ok(await _mediator.Send(new ExportDocumentTypesQuery(searchString)));
        }

        #region GetAllDocumentTypesDocumentation
        /// <summary>
        /// Obtenir tous les types de document sous forme de liste paginée.
        /// </summary>
        /// <param name="pageNumber">Numéro de page</param>
        /// <param name="pageSize">Taille de de la page</param>
        /// <param name="searchString">Critère de recherche</param>
        /// <param name="orderBy">Critère de tri (de la forme : attribut [ascending|descending],attribut [ascending|descending],...).</param>
        /// <returns>Une liste de types de document paginés.</returns>
        /// <remarks>
        /// Exemples de requête :
        ///     <br/>
        ///     <para>
        ///         <b>
        ///             GET /api/v1/documentTypes 
        ///         </b> 
        ///         <br/><br/><i>Retourne la liste complète des types de document.</i><br/>
        ///     </para> 
        ///     <para>
        ///         <b>
        ///             GET /api/v1/documentTypes?pageNumber=3&amp;pageSize=10
        ///         </b>
        ///         <br/><br/><i>Retourne les 10 types de document à la page 3.</i><br/>
        ///     </para>    
        ///     <para>
        ///         <b>
        ///             GET /api/v1/documentTypes?searchString=etaf&amp;orderBy=name%20descending
        ///         </b>
        ///         <br/><br/><i>Retourne tous les types de document contenant le critère de recherche "etaf" et triés par nom dans l'ordre décroissant.</i><br/>
        ///     </para>
        ///     <para>
        ///         <b>
        ///             GET /api/v1/documentTypes?orderBy=id,name%20ascending 
        ///         </b>
        ///         <br/><br/><i>Retourne tous les types de document triés par id et nom dans l'odre croissant.</i><br/>
        ///     </para>
        /// </remarks>
        /// <response code="200"> Retourne la liste de types de document paginés obtenue.</response>
        /// <response code="500"> Si le critère de tri spécifié n'est pas conforme au modèle de données.</response>
        #endregion GetAllDocumentTypesDocumentation
        [Authorize(Policy = Permissions.DocumentTypes.View)]
        [HttpGet]
        public async Task<IActionResult> GetAll(int pageNumber, int pageSize, string searchString = "", string orderBy = "")
        {
            var documentTypes = await _mediator.Send(new GetAllDocumentTypesQuery(pageNumber, pageSize, searchString, orderBy));
            return Ok(documentTypes);
        }

        #region GetAllDocumentTypesByExternalApplicationDocumentation
        /// <summary>
        /// Obtenir tous les types de document par application externe, sous forme de liste paginée.
        /// </summary>
        /// <param name="externalapplicationid">Identifiant de l'application</param>
        /// <param name="pageNumber">Numéro de page</param>
        /// <param name="pageSize">Taille de de la page</param>
        /// <param name="searchString">Critère de recherche</param>
        /// <param name="orderBy">Critère de tri (de la forme : attribut [ascending|descending],attribut [ascending|descending],...).</param>
        /// <returns>Une liste de types de document paginés.</returns>
        /// <remarks>
        /// Exemples de requête :
        ///     <br/>
        ///     <para>
        ///         <b>
        ///             GET /api/v1/documentTypes/by-application/1
        ///         </b> 
        ///         <br/><br/><i>Retourne la liste complète des types de document de l'application ayant l'id 1.</i><br/>
        ///     </para> 
        ///     <para>
        ///         <b>
        ///             GET /api/v1/documentTypes/by-application/1?pageNumber=3&amp;pageSize=10
        ///         </b>
        ///         <br/><br/><i>Retourne les 10 types de document de l'application 1 à la page 3.</i><br/>
        ///     </para>    
        ///     <para>
        ///         <b>
        ///             GET /api/v1/documentTypes/by-application/1?searchString=etaf&amp;orderBy=name%20descending
        ///         </b>
        ///         <br/><br/><i>Retourne tous les types de document de l'application 1, contenant le critère de recherche "etaf" et triées par nom dans l'ordre décroissant.</i><br/>
        ///     </para>
        ///     <para>
        ///         <b>
        ///             GET /api/v1/documentTypes/by-application/1?orderBy=id,name%20ascending
        ///         </b>
        ///         <br/><br/><i>retourne tous les types de document de l'application 1, triés par id et nom dans l'ordre croissant.</i><br/>
        ///     </para>
        /// </remarks>
        /// <response code="200"> Retourne la liste de types de document paginés obtenue pour l'application spécifiée.</response>
        /// <response code="500"> Si le critère de tri spécifié n'est pas conforme au modèle de données.</response>
        #endregion GetAllDocumentTypesByExternalApplicationDocumentation
        [Authorize(Policy = Permissions.DocumentTypes.View)]
        [HttpGet("by-application/{externalapplicationid}")]
        public async Task<IActionResult> GetAllByExternalApplication(int externalapplicationid, int pageNumber, int pageSize, string searchString = "", string orderBy = "")
        {
            var documentTypes = await _mediator.Send(new GetAllDocumentTypesByExternalApplicationQuery(externalapplicationid, pageNumber, pageSize, searchString, orderBy));
            return Ok(documentTypes);
        }

        #region GetDocumentTypeByIdDocumentation
        /// <summary>
        /// Obtenir un type de document à partir de son identifiant.
        /// </summary>
        /// <param name="id">Identifiant du type de document</param>
        /// <returns>Un type de document portant l'identifiant spécifié.</returns>
        /// <remarks>
        /// Exemple de requête :
        ///     <br/>
        ///     <para>
        ///         <b>
        ///             GET /api/v1/documentTypes/1
        ///         </b>
        ///         <br/><br/><i>Retourne un type de document portant l'identifiant 1.</i><br/>
        ///     </para>
        /// </remarks>
        /// <response code="200">Retourne le type de document ayant l'identifiant spécifié.</response>
        #endregion GetDocumentTypeByIdDocumentation
        [Authorize(Policy = Permissions.DocumentTypes.View)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var documentType = await _mediator.Send(new GetDocumentTypeByIdQuery(id));
            return Ok(documentType);
        }

        #region GetDocumentTypeCountDocumentation
        /// <summary>
        /// Compter le nombre de types de document enregistrés.
        /// </summary>
        /// <returns>Le nombre total de types de document enregistrés.</returns>
        /// <remarks>
        /// Exemple de requête :
        ///     <br/>
        ///     <para>
        ///         <b>
        ///             GET /api/v1/documentTypes/count
        ///         </b>
        ///         <br/><br/><i>Retourne le nombre total de types de document enregistrés.</i><br/>
        ///     </para>
        /// </remarks>
        /// <response code="200">Retourne le nombre total de types de document enregistrés.</response>
        #endregion GetDocumentTypeCountDocumentation
        [Authorize(Policy = Permissions.DocumentTypes.View)]
        [HttpGet("count")]
        public async Task<IActionResult> GetCount()
        {
            var documentTypeCount = await _mediator.Send(new GetDocumentTypeCountQuery());
            return Ok(documentTypeCount);
        }

        #region GetDocumentTypeCountByExternalApplicationDocumentation
        /// <summary>
        /// Compter le nombre de types de document enregistrés par application externe.
        /// </summary>
        /// <param name="externalapplicationid">Identifiant de l'application</param>
        /// <returns>Le nombre total de types de document enregistrés pour l'application externe spécifiée.</returns>
        /// <remarks>
        /// Exemple de requête :
        ///     <br/>
        ///     <para>
        ///         <b>
        ///             GET /api/v1/documentTypes/count/by-application/1
        ///         </b>
        ///         <br/><br/><i>Retourne le nombre total de types de document enregistrés pour l'application externe 1.</i><br/>
        ///     </para>
        /// </remarks>
        /// <response code="200">Retourne le nombre total de types de document enregistrés pour l'application spécifiée.</response>
        #endregion GetDocumentTypeCountByExternalApplicationDocumentation
        [Authorize(Policy = Permissions.DocumentTypes.View)]
        [HttpGet("count/by-application/{externalapplicationid}")]
        public async Task<IActionResult> GetCountByExternalApplication(int externalapplicationid)
        {
            var documentTypeCount = await _mediator.Send(new GetDocumentTypeCountByExternalApplicationQuery(externalapplicationid));
            return Ok(documentTypeCount);
        }

        #region AddEditDocumentTypeDocumentation
        /// <summary>
        /// Ajouter/Mettre à jour un type de document.
        /// </summary>
        /// <param name="command">Corps de la requête</param>
        /// <returns>L'id d'un nouveau type de document créé ou celui d'un type de document mis à jour.</returns>
        /// <remarks>
        /// Exemples de requête :
        ///     <br/>
        ///     <para>
        ///         <i>Création</i>
        ///     </para>
        ///     <br/>
        ///     <para>
        ///         <b>
        ///             POST /api/v1/documentTypes
        ///                  {
        ///                      "Name": "etafi",
        ///                      "Description": "Etat Finançier",
        ///                      "Format": "pdf",
        ///                      "ExternalApplication": "gudef"
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
        ///             POST /api/v1/documentTypes
        ///                 {
        ///                     "Id": 1,
        ///                     "Name": "etafi",
        ///                     "Description": "Etat Finançier"
        ///                     "Format": "pdf",
        ///                     "ExternalApplication": "gudef"
        ///                 }
        ///         </b>
        ///     </para>
        /// </remarks>
        /// <response code="200">Retourne l'identifiant du type de document créé ou mis à jour.</response>
        /// <response code="400">Si le corps de la requête est vide ou incomplet.</response>
        #endregion AddEditDocumentTypeDocumentation
        [Authorize(Policy = Permissions.DocumentTypes.Create)]
        [HttpPost]
        public async Task<IActionResult> Post(AddEditDocumentTypeCommand command)
        {
            return Ok(await _mediator.Send(command));
        }

        #region DeleteDocumentTypeDocumentation
        /// <summary>
        /// Supprimer un type de document.
        /// </summary>
        /// <param name="id">Identifiant du type de document.</param>
        /// <returns>L'identifiant du type de document supprimé.</returns>
        /// <remarks>
        /// Exemple de requête:
        ///     <br/>
        ///     <para>
        ///         <b>
        ///             DELETE /api/v1/documentTypes/1
        ///         </b>
        ///         <br/><br/><i>Retourne l'identifiant 1 après suppression.</i><br/>
        ///     </para>
        /// </remarks>
        /// <response code="200">Retourne l'identifiant du type de document supprimé.</response>
        #endregion DeleteDocumentTypeDocumentation
        [Authorize(Policy = Permissions.DocumentTypes.Delete)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await _mediator.Send(new DeleteDocumentTypeCommand { Id = id }));
        }
    }
}