using CleanArchitecture.Application.Features.DocumentMatchings.Commands.AddEdit;
using CleanArchitecture.Application.Features.DocumentMatchings.Queries.GetAll;
using CleanArchitecture.Application.Features.DocumentMatchings.Queries.GetByCentralizedDocument;
using CleanArchitecture.Application.Features.DocumentMatchings.Queries.GetById;
using CleanArchitecture.Shared.Wrapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CleanArchitecture.Client.Infrastructure.Managers.Sgcd.DocumentMatching
{
    public interface IDocumentMatchingManager :IManager
    {
        Task<IResult<List<GetAllDocumentMatchingsResponse>>> GetAllAsync();

        Task<PaginatedResult<GetAllDocumentMatchingsResponse>> GetAllPagedAsync(GetAllDocumentMatchingsQuery request);

        Task<IResult<GetDocumentMatchingByCentralizedDocumentResponse>> GetByCentralizedDocumentAsync(GetDocumentMatchingByCentralizedDocumentQuery request);

        Task<IResult<GetDocumentMatchingByIdResponse>> GetByIdAsync(GetDocumentMatchingByIdQuery request);

        Task<IResult<long>> GetCount();

        Task<IResult<Guid>> SaveAsync(AddEditDocumentMatchingCommand request);

        Task<IResult<Guid>> DeleteAsync(Guid id);
    }
}
