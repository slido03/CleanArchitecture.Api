using CleanArchitecture.Application.Interfaces.Services;
using CleanArchitecture.Application.Requests;
using System.IO;

namespace CleanArchitecture.Infrastructure.Services
{
    public class FileDeletionService : IFileDeletionService
    {
        public void DeleteFile(FileDeletionRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.FilePath)) return;

            var dbPath = request.FilePath;
            if (File.Exists(dbPath))
            {
                File.Delete(dbPath);
            }
        }
    }
}
