using CleanArchitecture.Application.Interfaces.Services;
using System;

namespace CleanArchitecture.Infrastructure.Shared.Services
{
    public class SystemDateTimeService : IDateTimeService
    {
        public DateTime NowUtc => DateTime.UtcNow;
    }
}