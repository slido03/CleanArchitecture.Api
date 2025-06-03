using System.Linq;

namespace CleanArchitecture.Client.Infrastructure.Routes
{
    public class ExternalApplicationsEndpoints
    {
        public static string GetAll => "api/v1/externalApplications";

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

        public static string GetById(int id)
        {
            return $"{GetAll}/{id}";
        }

        public static string GetCount => $"{GetAll}/count";

        public static string Save => $"{GetAll}";

        public static string Import => $"{GetAll}/import";

        public static string Delete => $"{GetAll}";
    }
}
