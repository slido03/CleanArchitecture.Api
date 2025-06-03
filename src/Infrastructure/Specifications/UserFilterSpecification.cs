using CleanArchitecture.Application.Specifications.Base;
using CleanArchitecture.Infrastructure.Models.Identity;

namespace CleanArchitecture.Infrastructure.Specifications
{
    public class UserFilterSpecification : HeroSpecification<User>
    {
        public UserFilterSpecification(string searchString)
        {
            if (!string.IsNullOrEmpty(searchString))
            {
                Criteria = p => p.FirstName.Contains(searchString) || p.LastName.Contains(searchString) || p.Email.Contains(searchString) || p.PhoneNumber.Contains(searchString) || p.UserName.Contains(searchString);
            }
            else
            {
                Criteria = p => true;
            }
        }
    }
}