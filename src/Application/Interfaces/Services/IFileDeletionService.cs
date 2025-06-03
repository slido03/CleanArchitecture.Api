using CleanArchitecture.Application.Requests;

namespace CleanArchitecture.Application.Interfaces.Services
{
    public interface IFileDeletionService
    {
        void DeleteFile(FileDeletionRequest request);
    }
}
