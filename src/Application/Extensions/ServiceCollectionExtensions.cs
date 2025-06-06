﻿using CleanArchitecture.Application.Features.ExtendedAttributes.Commands.AddEdit;
using CleanArchitecture.Application.Features.ExtendedAttributes.Commands.Delete;
using CleanArchitecture.Application.Features.ExtendedAttributes.Queries.Export;
using CleanArchitecture.Application.Features.ExtendedAttributes.Queries.GetAll;
using CleanArchitecture.Application.Features.ExtendedAttributes.Queries.GetAllByEntityId;
using CleanArchitecture.Application.Features.ExtendedAttributes.Queries.GetById;
using CleanArchitecture.Domain.Contracts;
using CleanArchitecture.Shared.Wrapper;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CleanArchitecture.Application.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddApplicationLayer(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            services.AddLocalization(options =>
            {
                options.ResourcesPath = "Resources";
            });
        }

        public static void AddExtendedAttributesHandlers(this IServiceCollection services)
        {
            var extendedAttributeTypes = typeof(IEntity)
                .Assembly
                .GetExportedTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.BaseType?.IsGenericType == true)
                .Select(t => new
                {
                    BaseGenericType = t.BaseType,
                    CurrentType = t
                })
                .Where(t => t.BaseGenericType?.GetGenericTypeDefinition() == typeof(AuditableEntityExtendedAttribute<,,>))
                .ToList();

            foreach (var extendedAttributeType in extendedAttributeTypes)
            {
                var extendedAttributeTypeGenericArguments = extendedAttributeType.BaseGenericType.GetGenericArguments().ToList();
                extendedAttributeTypeGenericArguments.Add(extendedAttributeType.CurrentType);

                #region AddEditExtendedAttributeCommandHandler

                var tRequest = typeof(AddEditExtendedAttributeCommand<,,,>).MakeGenericType(extendedAttributeTypeGenericArguments.ToArray());
                var tResponse = typeof(Result<>).MakeGenericType(extendedAttributeTypeGenericArguments.First());
                var serviceType = typeof(IRequestHandler<,>).MakeGenericType(tRequest, tResponse);
                var implementationType = typeof(AddEditExtendedAttributeCommandHandler<,,,>).MakeGenericType(extendedAttributeTypeGenericArguments.ToArray());
                services.AddScoped(serviceType, implementationType);

                #endregion AddEditExtendedAttributeCommandHandler

                #region DeleteExtendedAttributeCommandHandler

                tRequest = typeof(DeleteExtendedAttributeCommand<,,,>).MakeGenericType(extendedAttributeTypeGenericArguments.ToArray());
                tResponse = typeof(Result<>).MakeGenericType(extendedAttributeTypeGenericArguments.First());
                serviceType = typeof(IRequestHandler<,>).MakeGenericType(tRequest, tResponse);
                implementationType = typeof(DeleteExtendedAttributeCommandHandler<,,,>).MakeGenericType(extendedAttributeTypeGenericArguments.ToArray());
                services.AddScoped(serviceType, implementationType);

                #endregion DeleteExtendedAttributeCommandHandler

                #region GetAllExtendedAttributesByEntityIdQueryHandler

                tRequest = typeof(GetAllExtendedAttributesByEntityIdQuery<,,,>).MakeGenericType(extendedAttributeTypeGenericArguments.ToArray());
                tResponse = typeof(Result<>).MakeGenericType(typeof(List<>).MakeGenericType(
                    typeof(GetAllExtendedAttributesByEntityIdResponse<,>).MakeGenericType(
                        extendedAttributeTypeGenericArguments[0], extendedAttributeTypeGenericArguments[1])));
                serviceType = typeof(IRequestHandler<,>).MakeGenericType(tRequest, tResponse);
                implementationType = typeof(GetAllExtendedAttributesByEntityIdQueryHandler<,,,>).MakeGenericType(extendedAttributeTypeGenericArguments.ToArray());
                services.AddScoped(serviceType, implementationType);

                #endregion GetAllExtendedAttributesByEntityIdQueryHandler

                #region GetExtendedAttributeByIdQueryHandler

                tRequest = typeof(GetExtendedAttributeByIdQuery<,,,>).MakeGenericType(extendedAttributeTypeGenericArguments.ToArray());
                tResponse = typeof(Result<>).MakeGenericType(
                    typeof(GetExtendedAttributeByIdResponse<,>).MakeGenericType(
                        extendedAttributeTypeGenericArguments[0], extendedAttributeTypeGenericArguments[1]));
                serviceType = typeof(IRequestHandler<,>).MakeGenericType(tRequest, tResponse);
                implementationType = typeof(GetExtendedAttributeByIdQueryHandler<,,,>).MakeGenericType(extendedAttributeTypeGenericArguments.ToArray());
                services.AddScoped(serviceType, implementationType);

                #endregion GetExtendedAttributeByIdQueryHandler

                #region GetAllExtendedAttributesQueryHandler

                tRequest = typeof(GetAllExtendedAttributesQuery<,,,>).MakeGenericType(extendedAttributeTypeGenericArguments.ToArray());
                tResponse = typeof(Result<>).MakeGenericType(typeof(List<>).MakeGenericType(
                    typeof(GetAllExtendedAttributesResponse<,>).MakeGenericType(
                        extendedAttributeTypeGenericArguments[0], extendedAttributeTypeGenericArguments[1])));
                serviceType = typeof(IRequestHandler<,>).MakeGenericType(tRequest, tResponse);
                implementationType = typeof(GetAllExtendedAttributesQueryHandler<,,,>).MakeGenericType(extendedAttributeTypeGenericArguments.ToArray());
                services.AddScoped(serviceType, implementationType);

                #endregion GetAllExtendedAttributesQueryHandler

                #region ExportExtendedAttributesQueryHandler

                tRequest = typeof(ExportExtendedAttributesQuery<,,,>).MakeGenericType(extendedAttributeTypeGenericArguments.ToArray());
                tResponse = typeof(Result<>).MakeGenericType(typeof(string));
                serviceType = typeof(IRequestHandler<,>).MakeGenericType(tRequest, tResponse);
                implementationType = typeof(ExportExtendedAttributesQueryHandler<,,,>).MakeGenericType(extendedAttributeTypeGenericArguments.ToArray());
                services.AddScoped(serviceType, implementationType);

                #endregion ExportExtendedAttributesQueryHandler
            }
        }
    }
}