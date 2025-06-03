using CleanArchitecture.Application.Interfaces.Repositories;
using CleanArchitecture.Application.Interfaces.Serialization.Serializers;
using CleanArchitecture.Application.Interfaces.Services.Storage;
using CleanArchitecture.Application.Interfaces.Services.Storage.Provider;
using CleanArchitecture.Application.Serialization.JsonConverters;
using CleanArchitecture.Application.Serialization.Options;
using CleanArchitecture.Application.Serialization.Serializers;
using CleanArchitecture.Infrastructure.Repositories;
using CleanArchitecture.Infrastructure.Services.Storage;
using CleanArchitecture.Infrastructure.Services.Storage.Provider;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;

namespace CleanArchitecture.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddInfrastructureMappings(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
        }

        public static void AddInfrastructureLocalization(this IServiceCollection services)
        {
            services.AddLocalization(options =>
            {
                options.ResourcesPath = "Resources";
            });
        }

        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            return services
                .AddTransient(typeof(IRepositoryAsync<,>), typeof(RepositoryAsync<,>))
                .AddTransient<IExternalApplicationRepository, ExternalApplicationRepository>()
                .AddTransient<IDocumentRepository, DocumentRepository>()
                .AddTransient<IDocumentTypeRepository, DocumentTypeRepository>()
                .AddTransient<IDocumentVersionRepository, DocumentVersionRepository>()
                .AddTransient<IDocumentMatchingRepository, DocumentMatchingRepository>()
                .AddTransient(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));
        }

        public static IServiceCollection AddExtendedAttributesUnitOfWork(this IServiceCollection services)
        {
            return services
                .AddTransient(typeof(IExtendedAttributeUnitOfWork<,,>), typeof(ExtendedAttributeUnitOfWork<,,>));
        }

        public static IServiceCollection AddServerStorage(this IServiceCollection services)
            => AddServerStorage(services, null);

        public static IServiceCollection AddServerStorage(this IServiceCollection services, Action<SystemTextJsonOptions> configure)
        {
            return services
                .AddScoped<IJsonSerializer, SystemTextJsonSerializer>()
                .AddScoped<IStorageProvider, ServerStorageProvider>()
                .AddScoped<IServerStorageService, ServerStorageService>()
                .AddScoped<ISyncServerStorageService, ServerStorageService>()
                .Configure<SystemTextJsonOptions>(configureOptions =>
                {
                    configure?.Invoke(configureOptions);
                    if (!configureOptions.JsonSerializerOptions.Converters.Any(c => c.GetType() == typeof(TimespanJsonConverter)))
                        configureOptions.JsonSerializerOptions.Converters.Add(new TimespanJsonConverter());
                });
        }
    }
}