using CleanArchitecture.Domain.Contracts;
using System.ComponentModel.DataAnnotations.Schema;

namespace CleanArchitecture.Domain.Entities.Sgcd
{
    public class DocumentType : AuditableEntity<int>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        [Column(TypeName = "varchar(4)")]
        public string Format { get; set; }
        public int ExternalApplicationId { get; set; }
        public virtual ExternalApplication ExternalApplication { get; set; }
    }
}