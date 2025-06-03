using CleanArchitecture.Application.Requests;

namespace CleanArchitecture.Application.Interfaces.Services
{
    public interface IUploadService
    {
        string Upload(UploadRequest request);
    }
}