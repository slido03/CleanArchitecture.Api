using CleanArchitecture.Application.Specifications.Base;
using CleanArchitecture.Domain.Entities.Sgcd;

namespace CleanArchitecture.Application.Specifications.Sgcd
{
    public class DocumentTypeFilterSpecification : HeroSpecification<DocumentType>
    {
        public DocumentTypeFilterSpecification(string searchString)
        {
            if (!string.IsNullOrEmpty(searchString))
            {
                Criteria = t => t.Name.Contains(searchString) || t.Format.Contains(searchString) || t.ExternalApplication.Name.Contains(searchString)
                || t.Description.Contains(searchString) || t.ExternalApplication.Description.Contains(searchString);
            }
            else
            {
                Criteria = t => true;
            }
        }
    }
}