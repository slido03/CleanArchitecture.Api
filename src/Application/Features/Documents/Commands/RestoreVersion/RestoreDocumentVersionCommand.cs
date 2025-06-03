using CleanArchitecture.Application.Interfaces.Repositories;
using CleanArchitecture.Domain.Entities.Sgcd;
using CleanArchitecture.Shared.Wrapper;
using MediatR;
using Microsoft.Extensions.Localization;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Features.Documents.Commands.RestoreVersion
{
    public class RestoreDocumentVersionCommand : IRequest<Result<Guid>>
    {
        public Guid DocumentId { get; set; }
        public Guid DocumentVersionId { get; set; }
    }

    internal class RestoreDocumentVersionCommandHandler : IRequestHandler<RestoreDocumentVersionCommand, Result<Guid>>
    {
        private readonly IUnitOfWork<Guid> _unitOfWork;
        private readonly IStringLocalizer<RestoreDocumentVersionCommandHandler> _localizer;

        public RestoreDocumentVersionCommandHandler(IUnitOfWork<Guid> unitOfWork, IStringLocalizer<RestoreDocumentVersionCommandHandler> localizer)
        {
            _unitOfWork = unitOfWork;
            _localizer = localizer;
        }

        public async Task<Result<Guid>> Handle(RestoreDocumentVersionCommand command, CancellationToken cancellationToken)
        {
            var document = await _unitOfWork.Repository<Document>().GetByIdAsync(command.DocumentId);
            if (document != null)
            {
                var docVersions = document.DocumentVersions.AsQueryable();
                var version = docVersions.Where(v => v.Id == command.DocumentVersionId).FirstOrDefault();
                if (version != null)
                {
                    document.Title = version.Title;
                    document.Description = version.Description;
                    document.URL = version.FilePath;
                    await _unitOfWork.Repository<Document>().UpdateAsync(document);
                    await _unitOfWork.Commit(cancellationToken);
                    return await Result<Guid>.SuccessAsync(version.Id, _localizer["Document Version Restored"]);
                }
                else
                {
                    return await Result<Guid>.FailAsync(_localizer["The version specified were not found!"]);
                }
            }
            else
            {
                return await Result<Guid>.FailAsync(_localizer["Document Not Found!"]);
            }
        }
    }
}


