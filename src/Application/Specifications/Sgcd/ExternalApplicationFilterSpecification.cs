using CleanArchitecture.Application.Specifications.Base;
using CleanArchitecture.Domain.Entities.Sgcd;

namespace CleanArchitecture.Application.Specifications.Sgcd
{
    public class ExternalApplicationFilterSpecification : HeroSpecification<ExternalApplication>
    {
        public ExternalApplicationFilterSpecification(string searchString)
        {
            if (!string.IsNullOrEmpty(searchString))
            {
                Criteria = a => a.Name.Contains(searchString) || a.Description.Contains(searchString);
            }
            else
            {
                Criteria = a => true;
            }
        }
    }
}
