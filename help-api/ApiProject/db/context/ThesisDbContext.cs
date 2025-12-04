using ApiProject.Db.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApiProject.Db.Context;

public class ThesisDbContext : DbContext
{
    public ThesisDbContext(DbContextOptions<ThesisDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Thesis> Theses { get; set; }
    public DbSet<Topic> Topics { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure relationships
        modelBuilder.Entity<UserRole>()
            .HasKey(ur => new { ur.UserId, ur.RoleId });

        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.User)
            .WithMany(u => u.UserRoles)
            .HasForeignKey(ur => ur.UserId);

        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.Role)
            .WithMany(r => r.UserRoles)
            .HasForeignKey(ur => ur.RoleId);

        // Seed roles
        modelBuilder.Entity<Role>().HasData(
            new Role { Id = Guid.Parse("00000000-0000-0000-0000-000000000001"), Name = "STUDENT" },
            new Role { Id = Guid.Parse("00000000-0000-0000-0000-000000000002"), Name = "TUTOR" },
            new Role { Id = Guid.Parse("00000000-0000-0000-0000-000000000003"), Name = "SECOND_SUPERVISOR" }
        );
    }
}
