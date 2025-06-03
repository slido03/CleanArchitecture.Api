
namespace CleanArchitecture.Application.Features.DocumentTypes.Queries.GetAllByExternalApplication
{
    public class GetAllDocumentTypesByExternalApplicationResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Format { get; set; }
        public int ExternalApplicationId { get; set; }
        public string ExternalApplication { get; set; }
    }
}
