using System;
using System.Linq;

namespace CleanArchitecture.Client.Infrastructure.Routes
{
    public class DocumentMatchingsEndpoints
    {
        public static string GetAll => "api/v1/documentMatchings";

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

        public static string GetById(Guid documentMatchingId)
        {
            return $"{GetAll}/{documentMatchingId}";
        }

        public static string GetByCentralizedDocument(Guid centralizedDocumentId)
        {
            return $"{GetAll}/by-centralizedDocument/{centralizedDocumentId}";
        }

        public static string GetCount => $"{GetAll}/count";

        public static string Save => $"{GetAll}";

        public static string Delete => $"{GetAll}";
    }
}
