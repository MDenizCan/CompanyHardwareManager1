using CHM.ENTITIES.Common;
using CHM.ENTITIES.Entities;
using Microsoft.EntityFrameworkCore;

namespace CHM.INFRASTRUCTURE
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<UserRole> UserRoles => Set<UserRole>();
        public DbSet<Asset> Assets => Set<Asset>();
        public DbSet<Assignment> Assignments => Set<Assignment>();
        public DbSet<Request> Requests => Set<Request>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Global soft delete filter for all BaseEntity-derived entities
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
                {
                    modelBuilder.Entity(entityType.ClrType)
                        .HasQueryFilter(CreateIsDeletedRestriction(entityType.ClrType));
                }
            }

            modelBuilder.Entity<User>(b =>
            {
                b.HasIndex(x => x.Username).IsUnique();
                b.HasIndex(x => x.Email).IsUnique();
            });

            modelBuilder.Entity<Role>(b =>
            {
                b.HasIndex(x => x.Name).IsUnique();
            });

            modelBuilder.Entity<UserRole>(b =>
            {
                b.HasKey(x => new { x.UserId, x.RoleId });
                b.HasOne(x => x.User).WithMany(x => x.UserRoles).HasForeignKey(x => x.UserId);
                b.HasOne(x => x.Role).WithMany(x => x.UserRoles).HasForeignKey(x => x.RoleId);
                b.HasQueryFilter(x => !x.User.IsDeleted && !x.Role.IsDeleted);
            });

            modelBuilder.Entity<Asset>(b =>
            {
                b.HasIndex(x => x.SerialNumber).IsUnique();
            });

            modelBuilder.Entity<Assignment>(b =>
            {
                b.HasOne(x => x.Asset).WithMany(x => x.Assignments).HasForeignKey(x => x.AssetId);
                b.HasOne(x => x.User).WithMany(x => x.Assignments).HasForeignKey(x => x.UserId);

                b.HasOne(x => x.AssignedByUser)
                    .WithMany()
                    .HasForeignKey(x => x.AssignedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Enforce: same asset can have only one active assignment at a time
                // (SQL Server filtered unique index)
                b.HasIndex(x => x.AssetId)
                    .IsUnique()
                    .HasFilter("[ReturnedAt] IS NULL");
            });

            modelBuilder.Entity<Request>(b =>
            {
                b.HasOne(x => x.User).WithMany(x => x.Requests).HasForeignKey(x => x.UserId);
                b.HasOne(x => x.Asset).WithMany(x => x.Requests).HasForeignKey(x => x.AssetId);
            });

            modelBuilder.Entity<RefreshToken>(b =>
            {
                b.HasIndex(x => x.Token).IsUnique();
                b.HasOne(x => x.User).WithMany(x => x.RefreshTokens).HasForeignKey(x => x.UserId);
            });
        }

        private static System.Linq.Expressions.LambdaExpression CreateIsDeletedRestriction(Type entityClrType)
        {
            var parameter = System.Linq.Expressions.Expression.Parameter(entityClrType, "e");
            var property = System.Linq.Expressions.Expression.Property(parameter, nameof(BaseEntity.IsDeleted));
            var falseConstant = System.Linq.Expressions.Expression.Constant(false);
            var body = System.Linq.Expressions.Expression.Equal(property, falseConstant);
            return System.Linq.Expressions.Expression.Lambda(body, parameter);
        }
    }
}
