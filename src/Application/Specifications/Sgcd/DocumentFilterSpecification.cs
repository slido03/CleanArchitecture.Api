using CleanArchitecture.Application.Specifications.Base;
using CleanArchitecture.Domain.Entities.Sgcd;

namespace CleanArchitecture.Application.Specifications.Sgcd
{
    public class DocumentFilterSpecification : HeroSpecification<Document>
    {
        public DocumentFilterSpecification(string searchString, string userId)
        {
            if (!string.IsNullOrEmpty(searchString))
            {
                Criteria = d => (d.Title.Contains(searchString) || d.Description.Contains(searchString)
                || d.DocumentType.Name.Contains(searchString) || d.DocumentType.ExternalApplication.Name.Contains(searchString)) &&
                (d.IsPublic == true || (d.IsPublic == false && d.CreatedBy == userId));
            }
            else
            {
                Criteria = d => d.IsPublic == true || (d.IsPublic == false && d.CreatedBy == userId);
            }
        }
    }
}