using CleanArchitecture.Application.Interfaces.Services;
using CleanArchitecture.Application.Models.Chat;
using CleanArchitecture.Domain.Contracts;
using CleanArchitecture.Domain.Entities.ExtendedAttributes;
using CleanArchitecture.Domain.Entities.Sgcd;
using CleanArchitecture.Infrastructure.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Infrastructure.Contexts
{
    public class DatabaseContext : AuditableContext
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IDateTimeService _dateTimeService;

        public DatabaseContext(DbContextOptions<DatabaseContext> options, ICurrentUserService currentUserService, IDateTimeService dateTimeService)
            : base(options)
        {
            _currentUserService = currentUserService;
            _dateTimeService = dateTimeService;
        }

        #region EntitiesDbSets

        #region Sgcd DbSets

        public DbSet<ExternalApplication> ExternalApplications { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<DocumentType> DocumentTypes { get; set; }
        public DbSet<DocumentVersion> DocumentVersions { get; set; }
        public DbSet<DocumentMatching> DocumentMatchings { get; set; }
        public DbSet<DocumentExtendedAttribute> DocumentExtendedAttributes { get; set; }

        #endregion Sgcd DbSets

        public DbSet<ChatHistory<User>> ChatHistories { get; set; }

        #endregion EntitiesDbSets

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
        {
            foreach (var entry in ChangeTracker.Entries<IAuditableEntity>().ToList())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedOn = _dateTimeService.NowUtc;
                        entry.Entity.CreatedBy = _currentUserService.UserId;
                        break;

                    case EntityState.Modified:
                        entry.Entity.LastModifiedOn = _dateTimeService.NowUtc;
                        entry.Entity.LastModifiedBy = _currentUserService.UserId;
                        break;
                }
            }
            if (_currentUserService.UserId == null)
            {
                return await base.SaveChangesAsync(cancellationToken);
            }
            else
            {
                return await base.SaveChangesAsync(_currentUserService.UserId, cancellationToken);
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            foreach (var property in builder.Model.GetEntityTypes()
            .SelectMany(t => t.GetProperties())
            .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
            {
                property.SetColumnType("decimal(18,2)");
            }

            foreach (var property in builder.Model.GetEntityTypes()
                .SelectMany(t => t.GetProperties())
                .Where(p => p.Name is "LastModifiedBy" or "CreatedBy"))
            {
                property.SetColumnType("nvarchar(128)");
            }

            base.OnModelCreating(builder);
            builder.Entity<ChatHistory<User>>(entity =>
            {
                entity.ToTable("ChatHistory");

                entity.HasOne(d => d.FromUser)
                    .WithMany(p => p.ChatHistoryFromUsers)
                    .HasForeignKey(d => d.FromUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.ToUser)
                    .WithMany(p => p.ChatHistoryToUsers)
                    .HasForeignKey(d => d.ToUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });
            builder.Entity<User>(entity =>
            {
                entity.ToTable(name: "Users", "Identity");
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            builder.Entity<Role>(entity =>
            {
                entity.ToTable(name: "Roles", "Identity");
            });
            builder.Entity<IdentityUserRole<string>>(entity =>
            {
                entity.ToTable("UserRoles", "Identity");
            });

            builder.Entity<IdentityUserClaim<string>>(entity =>
            {
                entity.ToTable("UserClaims", "Identity");
            });

            builder.Entity<IdentityUserLogin<string>>(entity =>
            {
                entity.ToTable("UserLogins", "Identity");
            });

            builder.Entity<RoleClaim>(entity =>
            {
                entity.ToTable(name: "RoleClaims", "Identity");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.RoleClaims)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<IdentityUserToken<string>>(entity =>
            {
                entity.ToTable("UserTokens", "Identity");
            });

            //Sgcd model constraints
            builder.Entity<ExternalApplication>()
                .HasIndex(b => b.Name)
                .IsUnique();

            builder.Entity<DocumentType>()
                .HasIndex(b => b.Name)
                .IsUnique();

            builder.Entity<DocumentMatching>()
                .HasIndex(b => new { b.CentralizedDocumentId, b.ExternalId })
                .IsUnique();
        }

        //configure lazyloading for nested entities
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();
        }
    }
}
