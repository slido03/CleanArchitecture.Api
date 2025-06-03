using CleanArchitecture.Application.Configurations;
using CleanArchitecture.Application.Interfaces.Serialization.Options;
using CleanArchitecture.Application.Interfaces.Serialization.Serializers;
using CleanArchitecture.Application.Interfaces.Serialization.Settings;
using CleanArchitecture.Application.Interfaces.Services;
using CleanArchitecture.Application.Interfaces.Services.Account;
using CleanArchitecture.Application.Interfaces.Services.Identity;
using CleanArchitecture.Application.Serialization.JsonConverters;
using CleanArchitecture.Application.Serialization.Options;
using CleanArchitecture.Application.Serialization.Serializers;
using CleanArchitecture.Application.Serialization.Settings;
using CleanArchitecture.Infrastructure;
using CleanArchitecture.Infrastructure.Contexts;
using CleanArchitecture.Infrastructure.Models.Identity;
using CleanArchitecture.Infrastructure.Services;
using CleanArchitecture.Infrastructure.Services.Identity;
using CleanArchitecture.Infrastructure.Shared.Services;
using CleanArchitecture.Server.Filters;
using CleanArchitecture.Server.Localization;
using CleanArchitecture.Server.Managers.Preferences;
using CleanArchitecture.Server.Services;
using CleanArchitecture.Server.Settings;
using CleanArchitecture.Shared.Constants.Application;
using CleanArchitecture.Shared.Constants.Localization;
using CleanArchitecture.Shared.Constants.Permission;
using CleanArchitecture.Shared.Constants.User;
using CleanArchitecture.Shared.Wrapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Server.Extensions
{
    internal static class ServiceCollectionExtensions
    {
        internal static async Task<IStringLocalizer> GetRegisteredServerLocalizerAsync<T>(this IServiceCollection services) where T : class
        {
            var serviceProvider = services.BuildServiceProvider();
            await SetCultureFromServerPreferenceAsync(serviceProvider);
            var localizer = serviceProvider.GetService<IStringLocalizer<T>>();
            await serviceProvider.DisposeAsync();
            return localizer;
        }

        internal static IServiceCollection AddForwarding(this IServiceCollection services, IConfiguration configuration)
        {
            var applicationSettingsConfiguration = configuration.GetSection(nameof(AppConfiguration));
            var config = applicationSettingsConfiguration.Get<AppConfiguration>();
            if (config.BehindSSLProxy)
            {
                services.Configure<ForwardedHeadersOptions>(options =>
                {
                    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                    if (!string.IsNullOrWhiteSpace(config.ProxyIP))
                    {
                        var ipCheck = config.ProxyIP;
                        if (IPAddress.TryParse(ipCheck, out var proxyIP))
                            options.KnownProxies.Add(proxyIP);
                        else
                            Log.Logger.Warning("Invalid Proxy IP of {IpCheck}, Not Loaded", ipCheck);
                    }
                });

                services.AddCors(options =>
                {
                    options.AddDefaultPolicy(
                        builder =>
                        {
                            builder
                                .AllowCredentials()
                                .AllowAnyHeader()
                                .AllowAnyMethod()
                                .WithOrigins(config.ApplicationUrl.TrimEnd('/'));
                        });
                });
            }
            return services;
        }

        private static async Task SetCultureFromServerPreferenceAsync(IServiceProvider serviceProvider)
        {
            var storageService = serviceProvider.GetService<ServerPreferenceManager>();
            if (storageService != null)
            {
                // TODO - should implement ServerStorageProvider to work correctly!
                CultureInfo culture;
                if (await storageService.GetPreference() is ServerPreference preference)
                    culture = new(preference.LanguageCode);
                else
                    culture = new(LocalizationConstants.SupportedLanguages.FirstOrDefault()?.Code ?? "en-US");
                CultureInfo.DefaultThreadCurrentCulture = culture;
                CultureInfo.DefaultThreadCurrentUICulture = culture;
                CultureInfo.CurrentCulture = culture;
                CultureInfo.CurrentUICulture = culture;
            }
        }

        internal static IServiceCollection AddServerLocalization(this IServiceCollection services)
        {
            services.TryAddTransient(typeof(IStringLocalizer<>), typeof(ServerLocalizer<>));
            return services;
        }

        internal static AppConfiguration GetApplicationSettings(
           this IServiceCollection services,
           IConfiguration configuration)
        {
            var applicationSettingsConfiguration = configuration.GetSection(nameof(AppConfiguration));
            services.Configure<AppConfiguration>(applicationSettingsConfiguration);
            return applicationSettingsConfiguration.Get<AppConfiguration>();
        }

        internal static void RegisterSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(async c =>
            {
                //Lowercase Swagger Documents
                //Refer - https://gist.github.com/rafalkasa/01d5e3b265e5aa075678e0adfd54e23f

                //c.DocumentFilter<LowercaseDocumentFilter>();

                // include all project's xml comments
                var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (!assembly.IsDynamic)
                    {
                        var xmlFile = $"{assembly.GetName().Name}.xml";
                        var xmlPath = Path.Combine(baseDirectory, xmlFile);
                        if (File.Exists(xmlPath))
                        {
                            c.IncludeXmlComments(xmlPath);
                        }
                    }
                }

                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "CleanArchitecture",
                    License = new OpenApiLicense
                    {
                        Name = "MIT License",
                        Url = new Uri("https://opensource.org/licenses/MIT")
                    }
                });

                var localizer = await GetRegisteredServerLocalizerAsync<ServerCommonResources>(services);

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    Description = localizer["Input your Bearer token in this format - Bearer {your token here} to access this API"],
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer",
                            },
                            Scheme = "Bearer",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        }, new List<string>()
                    },
                });
            });
        }

        internal static IServiceCollection AddSerialization(this IServiceCollection services)
        {
            services
                .AddScoped<IJsonSerializerOptions, SystemTextJsonOptions>()
                .Configure<SystemTextJsonOptions>(configureOptions =>
                {
                    if (!configureOptions.JsonSerializerOptions.Converters.Any(c => c.GetType() == typeof(TimespanJsonConverter)))
                        configureOptions.JsonSerializerOptions.Converters.Add(new TimespanJsonConverter());
                });
            services.AddScoped<IJsonSerializerSettings, NewtonsoftJsonSettings>();

            services.AddScoped<IJsonSerializer, SystemTextJsonSerializer>(); // you can change it
            return services;
        }

        internal static IServiceCollection AddDatabase(
            this IServiceCollection services,
            IConfiguration configuration)
            => services
                .AddDbContext<DatabaseContext>(options => options
                    .UseSqlServer(configuration.GetConnectionString("DefaultConnection")))
            .AddTransient<IDatabaseSeeder, DatabaseSeeder>();

        internal static IServiceCollection AddCurrentUserService(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            return services;
        }

        internal static IServiceCollection AddIdentity(this IServiceCollection services)
        {
            services
                .AddIdentity<User, Role>(options =>
                {
                    options.Password.RequiredLength = UserConstants.RequiredPasswordLength;
                    options.Password.RequireDigit = true;
                    options.Password.RequireUppercase = true;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireNonAlphanumeric = true;
                    options.User.RequireUniqueEmail = true;
                    options.Lockout.AllowedForNewUsers = true;
                    options.Lockout.MaxFailedAccessAttempts = UserConstants.MaxFailedAccessAttempts;
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(UserConstants.DefaultLockoutTimeSpan);
                })
                .AddEntityFrameworkStores<DatabaseContext>()
                .AddDefaultTokenProviders();

            return services;
        }

        internal static IServiceCollection AddSharedInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IDateTimeService, SystemDateTimeService>();
            services.Configure<MailConfiguration>(configuration.GetSection("MailConfiguration"));
           // services.Configure<SMSConfiguration>(configuration.GetSection("SMSConfiguration"));
            services.AddTransient<IMailService, SMTPMailService>();
           // services.AddTransient<ISMSService, SMSService>();
            return services;
        }

        internal static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddTransient<IRoleClaimService, RoleClaimService>();
            services.AddTransient<ITokenService, IdentityService>();
            services.AddTransient<IRoleService, RoleService>();
            services.AddTransient<IAccountService, AccountService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IChatService, ChatService>();
            services.AddTransient<IUploadService, UploadService>();
            services.AddTransient<IAuditService, AuditService>();
            services.AddScoped<IExcelService, ExcelService>();
            services.AddScoped<IFileDeletionService, FileDeletionService>();
            return services;
        }

        internal static IServiceCollection AddJwtAuthentication(
            this IServiceCollection services, AppConfiguration config)
        {
            var key = Encoding.UTF8.GetBytes(config.Secret);
            services
                .AddAuthentication(authentication =>
                {
                    authentication.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    authentication.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(async bearer =>
                {
                    bearer.RequireHttpsMetadata = false;
                    bearer.SaveToken = true;
                    bearer.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        RoleClaimType = ClaimTypes.Role,
                        ClockSkew = TimeSpan.Zero
                    };

                    var localizer = await GetRegisteredServerLocalizerAsync<ServerCommonResources>(services);

                    bearer.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];

                            // If the request is for our hub...
                            var path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrEmpty(accessToken) &&
                                (path.StartsWithSegments(ApplicationConstants.SignalR.HubUrl)))
                            {
                                // Read the token out of the query string
                                context.Token = accessToken;
                            }
                            return Task.CompletedTask;
                        },
                        OnAuthenticationFailed = c =>
                        {
                            if (c.Exception is SecurityTokenExpiredException)
                            {
                                c.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                                c.Response.ContentType = "application/json";
                                var result = JsonConvert.SerializeObject(Result.Fail(localizer["The Token is expired."]));
                                return c.Response.WriteAsync(result);
                            }
                            else
                            {
#if DEBUG
                                c.NoResult();
                                c.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                                c.Response.ContentType = "text/plain";
                                return c.Response.WriteAsync(c.Exception.ToString());
#else
                                c.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                                c.Response.ContentType = "application/json";
                                var result = JsonConvert.SerializeObject(Result.Fail(localizer["An unhandled error has occurred."]));
                                return c.Response.WriteAsync(result);
#endif
                            }
                        },
                        OnChallenge = context =>
                        {
                            context.HandleResponse();
                            if (!context.Response.HasStarted)
                            {
                                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                                context.Response.ContentType = "application/json";
                                var result = JsonConvert.SerializeObject(Result.Fail(localizer["You are not Authorized."]));
                                return context.Response.WriteAsync(result);
                            }

                            return Task.CompletedTask;
                        },
                        OnForbidden = context =>
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                            context.Response.ContentType = "application/json";
                            var result = JsonConvert.SerializeObject(Result.Fail(localizer["You are not authorized to access this resource."]));
                            return context.Response.WriteAsync(result);
                        },
                    };
                });
            services.AddAuthorization(options =>
            {
                // Here I stored necessary permissions/roles in a constant
                foreach (var prop in typeof(Permissions).GetNestedTypes().SelectMany(c => c.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)))
                {
                    var propertyValue = prop.GetValue(null);
                    if (propertyValue is not null)
                    {
                        options.AddPolicy(propertyValue.ToString(), policy => policy.RequireClaim(ApplicationClaimTypes.Permission, propertyValue.ToString()));
                    }
                }
            });
            return services;
        }
    }
}