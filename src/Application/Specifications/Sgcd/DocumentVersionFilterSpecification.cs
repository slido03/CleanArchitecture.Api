using CleanArchitecture.Application.Specifications.Base;
using CleanArchitecture.Domain.Entities.Sgcd;

namespace CleanArchitecture.Application.Specifications.Sgcd
{
    public class DocumentVersionFilterSpecification : HeroSpecification<DocumentVersion>
    {
        public DocumentVersionFilterSpecification(string searchString)
        {
            if (!string.IsNullOrEmpty(searchString))
            {
                Criteria = v => v.Document.Title.Contains(searchString) || v.Description.Contains(searchString) || v.Document.Description.Contains(searchString)
                || v.Document.DocumentType.Name.Contains(searchString) || v.Document.DocumentType.ExternalApplication.Name.Contains(searchString);
            }
            else
            {
                Criteria = v => true;
            }
        }
    }
}
