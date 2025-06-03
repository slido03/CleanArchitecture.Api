using System;
using System.Linq;

namespace CleanArchitecture.Client.Infrastructure.Routes
{
    public static class DocumentVersionsEndpoints
    {
        public static string GetAll => "api/v1/documentVersions";

        public static string Export => $"{GetAll}/export";

        public static string ExportFiltered(string searchString)
        {
            return $"{Export}?searchString={searchString}";
        }

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

        public static string GetAllByDocument(Guid documentId)
        {
            return $"{GetAll}/by-document/{documentId}";
        }

        public static string GetAllPagedByDocument(Guid documentId, int pageNumber, int pageSize, string searchString, string[] orderBy)
        {
            var url = $"{GetAllByDocument(documentId)}?pageNumber={pageNumber}&pageSize={pageSize}&searchString={searchString}&orderBy=";
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

        public static string GetById(Guid id)
        {
            return $"{GetAll}/{id}";
        }

        public static string GetCount => $"{GetAll}/count";
        

        public static string GetCountByDocument(Guid documentId)
        {
            return $"{GetCount}/by-document/{documentId}";
        }

        public static string Save => $"{GetAll}";

        public static string Delete => $"{GetAll}";
    }
}
