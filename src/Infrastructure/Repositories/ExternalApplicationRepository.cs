using CleanArchitecture.Application.Interfaces.Repositories;
using CleanArchitecture.Domain.Entities.Sgcd;

namespace CleanArchitecture.Infrastructure.Repositories
{
    public class ExternalApplicationRepository : IExternalApplicationRepository
    {
        private readonly IRepositoryAsync<ExternalApplication, int> _repository;

        public ExternalApplicationRepository(IRepositoryAsync<ExternalApplication, int> repository)
        {
            _repository = repository;
        }
    }
}
