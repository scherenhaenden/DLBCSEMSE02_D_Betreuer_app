using ApiProject.DatabaseAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApiProject.DatabaseAccess.Context;

public class ThesisDbContext : DbContext
{
    public ThesisDbContext(DbContextOptions<ThesisDbContext> options) : base(options) { }

    public DbSet<UserDataAccessModel> Users { get; set; }
    public DbSet<RoleDataAccessModel> Roles { get; set; }
    public DbSet<UserRoleDataAccessModel> UserRoles { get; set; }
    public DbSet<ThesisDataAccessModel> Theses { get; set; }
    public DbSet<SubjectAreaDataAccessModel> Topics { get; set; }
    public DbSet<UserToSubjectAreas> UserTopics { get; set; }
    public DbSet<ThesisStatusDataAccessModel> ThesisStatuses { get; set; }
    public DbSet<BillingStatusDataAccessModel> BillingStatuses { get; set; }
    public DbSet<ThesisDocumentDataAccessModel> ThesisDocuments { get; set; }
    public DbSet<ThesisRequestDataAccessModel> ThesisRequests { get; set; }
    public DbSet<RequestTypeDataAccessModel> RequestTypes { get; set; }
    public DbSet<RequestStatusDataAccessModel> RequestStatuses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // --- Composite Keys ---
        modelBuilder.Entity<UserRoleDataAccessModel>().HasKey(ur => new { ur.UserId, ur.RoleId });
        modelBuilder.Entity<UserToSubjectAreas>().HasKey(ut => new { ut.UserId, TopicId = ut.UserToSubjectAreaId });

        // --- Relationships ---
        modelBuilder.Entity<UserRoleDataAccessModel>()
            .HasOne(ur => ur.User)
            .WithMany(u => u.UserRoles)
            .HasForeignKey(ur => ur.UserId);

        modelBuilder.Entity<UserRoleDataAccessModel>()
            .HasOne(ur => ur.Role)
            .WithMany(r => r.UserRoles)
            .HasForeignKey(ur => ur.RoleId);

        modelBuilder.Entity<UserToSubjectAreas>()
            .HasOne(ut => ut.User)
            .WithMany(u => u.UserToSubjectAreas)
            .HasForeignKey(ut => ut.UserId);

        modelBuilder.Entity<UserToSubjectAreas>()
            .HasOne(ut => ut.SubjectArea)
            .WithMany(t => t.UserToSubjectAreas)
            .HasForeignKey(ut => ut.UserToSubjectAreaId);

        modelBuilder.Entity<ThesisDataAccessModel>()
            .HasOne(t => t.Owner)
            .WithMany()
            .HasForeignKey(t => t.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ThesisDataAccessModel>()
            .HasOne(t => t.Tutor)
            .WithMany()
            .HasForeignKey(t => t.TutorId)
            .OnDelete(DeleteBehavior.Restrict);
            
        modelBuilder.Entity<ThesisDataAccessModel>()
            .HasOne(t => t.Document)
            .WithOne(d => d.Thesis)
            .HasForeignKey<ThesisDocumentDataAccessModel>(d => d.ThesisId);
            
        modelBuilder.Entity<ThesisRequestDataAccessModel>()
            .HasOne(tr => tr.Requester)
            .WithMany()
            .HasForeignKey(tr => tr.RequesterId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ThesisRequestDataAccessModel>()
            .HasOne(tr => tr.Receiver)
            .WithMany()
            .HasForeignKey(tr => tr.ReceiverId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ThesisRequestDataAccessModel>()
            .HasOne(tr => tr.Thesis)
            .WithMany()
            .HasForeignKey(tr => tr.ThesisId)
            .OnDelete(DeleteBehavior.Cascade);

        // --- Seed Data ---
        modelBuilder.Entity<RoleDataAccessModel>().HasData(
            new RoleDataAccessModel { Id = Guid.NewGuid(), Name = "STUDENT", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new RoleDataAccessModel { Id = Guid.NewGuid(), Name = "TUTOR", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new RoleDataAccessModel { Id = Guid.NewGuid(), Name = "ADMIN", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        );

        modelBuilder.Entity<ThesisStatusDataAccessModel>().HasData(
            new ThesisStatusDataAccessModel { Id = Guid.NewGuid(), Name = "PendingApproval", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new ThesisStatusDataAccessModel { Id = Guid.NewGuid(), Name = "Registered", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new ThesisStatusDataAccessModel { Id = Guid.NewGuid(), Name = "Submitted", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new ThesisStatusDataAccessModel { Id = Guid.NewGuid(), Name = "Colloquium", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        );

        modelBuilder.Entity<BillingStatusDataAccessModel>().HasData(
            new BillingStatusDataAccessModel { Id = Guid.NewGuid(), Name = "None", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new BillingStatusDataAccessModel { Id = Guid.NewGuid(), Name = "Invoiced", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new BillingStatusDataAccessModel { Id = Guid.NewGuid(), Name = "Paid", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        );
        
        modelBuilder.Entity<RequestTypeDataAccessModel>().HasData(
            new RequestTypeDataAccessModel { Id = Guid.NewGuid(), Name = "SUPERVISION", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new RequestTypeDataAccessModel { Id = Guid.NewGuid(), Name = "CO_SUPERVISION", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        );

        modelBuilder.Entity<RequestStatusDataAccessModel>().HasData(
            new RequestStatusDataAccessModel { Id = Guid.NewGuid(), Name = "PENDING", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new RequestStatusDataAccessModel { Id = Guid.NewGuid(), Name = "ACCEPTED", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new RequestStatusDataAccessModel { Id = Guid.NewGuid(), Name = "REJECTED", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        );
    }
}
