using CleanArchitecture.Application.Enums;
using CleanArchitecture.Application.Extensions;
using CleanArchitecture.Application.Interfaces.Services;
using CleanArchitecture.Application.Requests;
using System.IO;

namespace CleanArchitecture.Infrastructure.Services
{
    public class UploadService : IUploadService
    {
        public string Upload(UploadRequest request)
        {
            if (request.Data == null) return string.Empty;

            if (request.UploadType == UploadType.Document)
            {
                return UploadDocument(request);
            }
            else
            {
                var streamData = new MemoryStream(request.Data);
                if (streamData.Length > 0)
                {
                    var folder = request.UploadType.ToDescriptionString();
                    var folderName = Path.Combine("Files", folder);
                    var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                    bool exists = System.IO.Directory.Exists(pathToSave);
                    if (!exists)
                        System.IO.Directory.CreateDirectory(pathToSave);
                    var fileName = request.FileName.Trim('"');
                    var fullPath = Path.Combine(pathToSave, fileName);
                    var dbPath = Path.Combine(folderName, fileName);
                    if (File.Exists(dbPath))
                    {
                        dbPath = NextAvailableFilename(dbPath);
                        fullPath = NextAvailableFilename(fullPath);
                    }
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        streamData.CopyTo(stream);
                    }
                    return dbPath;
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        private static string UploadDocument(UploadRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.ExternalApplication)) return string.Empty;
            if (string.IsNullOrWhiteSpace(request.DocumentType)) return string.Empty;

            var streamData = new MemoryStream(request.Data);
            if (streamData.Length > 0)
            {
                var folderName = GetFolderName(request);
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                bool exists = System.IO.Directory.Exists(pathToSave);
                if (!exists)
                    System.IO.Directory.CreateDirectory(pathToSave);

                var fullFileName = GetFullFileName(request);
                var fullPath = Path.Combine(pathToSave, fullFileName);
                var dbPath = Path.Combine(folderName, fullFileName);
                if (File.Exists(dbPath))
                {
                    dbPath = NextAvailableFilename(dbPath);
                    fullPath = NextAvailableFilename(fullPath);
                }
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    streamData.CopyTo(stream);
                }
                return dbPath;
            }
            else
            {
                return string.Empty;
            }
        }

        private static string GetFolderName(UploadRequest request)
        {
            var rootFolder = request.UploadType.ToDescriptionString();
            var rootFolderName = Path.Combine("Files", rootFolder);

            var applicationFolder = request.ExternalApplication;
            var applicationFolderName = Path.Combine(rootFolderName, applicationFolder);

            var documentTypeFolder = request.DocumentType;
            var documentTypeFolderName = Path.Combine(applicationFolderName, documentTypeFolder);
            return documentTypeFolderName;
        }

        private static string GetFullFileName(UploadRequest request)
        {
            var fileName = request.FileName.Trim('"');
            var versionNumber = request.VersionNumber;
            fileName = fileName.Replace($"_v{versionNumber - 1}", string.Empty);
            var fullFileName = fileName + $"_v{versionNumber}" + request.Extension;
            return fullFileName;
        }

        private static readonly string numberPattern = " ({0})";

        private static string NextAvailableFilename(string path)
        {
            // Short-cut if already available
            if (!File.Exists(path))
                return path;

            // If path has extension then insert the number pattern just before the extension and return next filename
            if (Path.HasExtension(path))
                return GetNextFilename(path.Insert(path.LastIndexOf(Path.GetExtension(path)), numberPattern));
            // Otherwise just append the pattern to the path and return next filename
            return GetNextFilename(path + numberPattern);
        }

        private static string GetNextFilename(string pattern)
        {
            string tmp = string.Format(pattern, 1);
            if (!File.Exists(tmp))
                return tmp; // short-circuit if no matches

            int min = 1, max = 2; // min is inclusive, max is exclusive/untested
            while (File.Exists(string.Format(pattern, max)))
            {
                min = max;
                max *= 2;
            }

            while (max != min + 1)
            {
                int pivot = (max + min) / 2;
                if (File.Exists(string.Format(pattern, pivot)))
                    min = pivot;
                else
                    max = pivot;
            }

            return string.Format(pattern, max);
        }
    }
}