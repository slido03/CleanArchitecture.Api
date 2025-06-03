using System;
using System.Linq;

namespace CleanArchitecture.Client.Infrastructure.Routes
{
    public static class DocumentsEndpoints
    {
        public static string GetAll => "api/v1/documents";

        public static string GetAllPaged(int pageNumber, int pageSize, string searchString, string[] orderBy)
        {
            var url = $"{GetAll}?pageNumber={pageNumber}&pageSize={pageSize}&searchString={searchString}&orderBy=";
            if (orderBy?.Any() == true)
            {
                foreach (var orderByPart in orderBy)
                {
                    url += $"{orderByPart},";
                }
                url = url[..^1]; // delete training ,
            }
            return url;
        }

        public static string GetAllByDocumentType(int documentTypeId)
        {
            return $"{GetAll}/by-documentType/{documentTypeId}";
        }

        public static string GetAllPagedByDocumentType(int documentTypeId, int pageNumber, int pageSize, string searchString, string[] orderBy)
        {
            var url = $"{GetAllByDocumentType(documentTypeId)}?pageNumber={pageNumber}&pageSize={pageSize}&searchString={searchString}&orderBy=";
            if (orderBy?.Any() == true)
            {
                foreach (var orderByPart in orderBy)
                {
                    url += $"{orderByPart},";
                }
                url = url[..^1]; // delete training ,
            }
            return url;
        }

        public static string GetByExternalId(string documentType, string externalId)
        {
            return $"{GetAll}/documentType/{documentType}/by-externalId/{externalId}";
        }

        public static string GetById(Guid documentId)
        {
            return $"{GetAll}/{documentId}";
        }

        public static string GetCount => $"{GetAll}/count";

        public static string GetCountByDocumentType(int documentTypeId)
        {
            return $"{GetCount}/by-documentType/{documentTypeId}";
        }

        public static string RestoreVersion => $"{GetAll}/restoreVersion";

        public static string Save => $"{GetAll}";

        public static string Delete => $"{GetAll}";
    }
}