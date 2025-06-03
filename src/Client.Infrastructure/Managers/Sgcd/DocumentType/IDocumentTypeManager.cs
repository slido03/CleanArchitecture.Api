using CleanArchitecture.Application.Features.DocumentTypes.Commands.AddEdit;
using CleanArchitecture.Application.Features.DocumentTypes.Commands.Import;
using CleanArchitecture.Application.Features.DocumentTypes.Queries.GetAll;
using CleanArchitecture.Application.Features.DocumentTypes.Queries.GetAllByExternalApplication;
using CleanArchitecture.Application.Features.DocumentTypes.Queries.GetById;
using CleanArchitecture.Application.Features.DocumentTypes.Queries.GetCountByExternalApplication;
using CleanArchitecture.Shared.Wrapper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CleanArchitecture.Client.Infrastructure.Managers.Sgcd.DocumentType
{
    public interface IDocumentTypeManager : IManager
    {
        Task<IResult<string>> ExportToExcelAsync(string searchString = "");

        Task<IResult<List<GetAllDocumentTypesResponse>>> GetAllAsync();

        Task<IResult<List<GetAllDocumentTypesByExternalApplicationResponse>>> GetAllByExternalApplicationAsync(GetAllDocumentTypesByExternalApplicationQuery request);

        Task<PaginatedResult<GetAllDocumentTypesResponse>> GetAllPagedAsync(GetAllDocumentTypesQuery request);

        Task<PaginatedResult<GetAllDocumentTypesByExternalApplicationResponse>> GetAllPagedByExternalApplicationAsync(GetAllDocumentTypesByExternalApplicationQuery request);

        Task<IResult<GetDocumentTypeByIdResponse>> GetByIdAsync(GetDocumentTypeByIdQuery request);

        Task<IResult<int>> GetCount();

        Task<IResult<int>> GetCountByExternalApplicationAsync(GetDocumentTypeCountByExternalApplicationQuery request);

        Task<IResult<int>> SaveAsync(AddEditDocumentTypeCommand request);

        Task<IResult<int>> ImportAsync(ImportDocumentTypesCommand request);

        Task<IResult<int>> DeleteAsync(int id);
    }
}