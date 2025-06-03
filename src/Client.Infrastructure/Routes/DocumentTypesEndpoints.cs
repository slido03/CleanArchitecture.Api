using System.Linq;

namespace CleanArchitecture.Client.Infrastructure.Routes
{
    public static class DocumentTypesEndpoints
    {
        public static string GetAll => "api/v1/documentTypes";

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

        public static string GetAllByExternalApplication(int externalApplicationId)
        {
            return $"{GetAll}/by-application/{externalApplicationId}";
        }

        public static string GetAllPagedByExternalApplication(int externalApplicationId, int pageNumber, int pageSize, string searchString, string[] orderBy)
        {
            var url = $"{GetAllByExternalApplication(externalApplicationId)}?pageNumber={pageNumber}&pageSize={pageSize}&searchString={searchString}&orderBy=";
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

        public static string GetById(int id)
        {
            return $"{GetAll}/{id}";
        }

        public static string GetCount => $"{GetAll}/count";

        public static string GetCountByExternalApplication(int externalApplicationId)
        {
            return $"{GetCount}/by-application/{externalApplicationId}";
        }

        public static string Save => $"{GetAll}";

        public static string Import => $"{GetAll}/import";

        public static string Delete => $"{GetAll}";
    }
}