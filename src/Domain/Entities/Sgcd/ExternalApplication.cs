using CleanArchitecture.Domain.Contracts;

namespace CleanArchitecture.Domain.Entities.Sgcd
{
    public class ExternalApplication : AuditableEntity<int>
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
