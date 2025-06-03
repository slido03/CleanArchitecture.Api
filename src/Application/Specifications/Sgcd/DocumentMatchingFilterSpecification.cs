using CleanArchitecture.Application.Specifications.Base;
using CleanArchitecture.Domain.Entities.Sgcd;

namespace CleanArchitecture.Application.Specifications.Sgcd
{
    public class DocumentMatchingFilterSpecification : HeroSpecification<DocumentMatching>
    {
        public DocumentMatchingFilterSpecification(string searchString)
        {
            if (!string.IsNullOrEmpty(searchString))
            {
                Criteria = m => m.CentralizedDocument.Title.Contains(searchString) || m.CentralizedDocument.DocumentType.Name.Contains(searchString)
                || m.CentralizedDocument.DocumentType.ExternalApplication.Name.Contains(searchString);
            }
            else
            {
                Criteria = m => true;
            }
        }
    }
}
