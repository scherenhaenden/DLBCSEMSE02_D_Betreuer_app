using ApiProject.DatabaseAccess.Context;
using ApiProject.DatabaseAccess.Entities;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace ApiProject.Tests.NUnit.BusinessLogic.Services;

public class TestDataSeeder
{
    private readonly ThesisDbContext _context;

    public TestDataSeeder(ThesisDbContext context)
    {
        _context = context;
    }

    public void SeedRoles()
    {
        if (!_context.Roles.Any())
        {
            _context.Roles.AddRange(
                new RoleDataAccessModel { Id = Guid.NewGuid(), Name = "STUDENT" },
                new RoleDataAccessModel { Id = Guid.NewGuid(), Name = "TUTOR" },
                new RoleDataAccessModel { Id = Guid.NewGuid(), Name = "ADMIN" }
            );
            _context.SaveChanges();
        }
    }

    public void SeedBillingStatuses()
    {
        if (!_context.BillingStatuses.Any())
        {
            _context.BillingStatuses.AddRange(
                new BillingStatusDataAccessModel { Id = Guid.NewGuid(), Name = "NONE" },
                new BillingStatusDataAccessModel { Id = Guid.NewGuid(), Name = "ISSUED" },
                new BillingStatusDataAccessModel { Id = Guid.NewGuid(), Name = "PAID" }
            );
            _context.SaveChanges();
        }
    }

    public void SeedThesisStatuses()
    {
        if (!_context.ThesisStatuses.Any())
        {
            _context.ThesisStatuses.AddRange(
                new ThesisStatusDataAccessModel { Id = Guid.NewGuid(), Name = "IN_DISCUSSION" },
                new ThesisStatusDataAccessModel { Id = Guid.NewGuid(), Name = "REGISTERED" },
                new ThesisStatusDataAccessModel { Id = Guid.NewGuid(), Name = "SUBMITTED" },
                new ThesisStatusDataAccessModel { Id = Guid.NewGuid(), Name = "DEFENDED" }
            );
            _context.SaveChanges();
        }
    }

    public void SeedSubjectAreas()
    {
        if (!_context.SubjectAreas.Any())
        {
            _context.SubjectAreas.AddRange(
                new SubjectAreaDataAccessModel { Id = Guid.NewGuid(), Title = "Computer Science", Description = "Study of computers" },
                new SubjectAreaDataAccessModel { Id = Guid.NewGuid(), Title = "Mathematics", Description = "Study of numbers" }
            );
            _context.SaveChanges();
        }
    }

    public UserDataAccessModel SeedUser(string firstName, string lastName, string email, string password, string roleName)
    {
        var role = _context.Roles.First(r => r.Name == roleName);
        var user = new UserDataAccessModel
        {
            Id = Guid.NewGuid(),
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password, workFactor: 4),
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };
        _context.Users.Add(user);
        _context.UserRoles.Add(new UserRoleDataAccessModel { UserId = user.Id, RoleId = role.Id });
        _context.SaveChanges();
        return user;
    }

    public ThesisDataAccessModel SeedThesis(string title, Guid ownerId, Guid subjectAreaId, Guid statusId, Guid billingStatusId)
    {
        var thesis = new ThesisDataAccessModel
        {
            Id = Guid.NewGuid(),
            Title = title,
            OwnerId = ownerId,
            SubjectAreaId = subjectAreaId,
            StatusId = statusId,
            BillingStatusId = billingStatusId,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };
        _context.Theses.Add(thesis);
        _context.SaveChanges();
        return thesis;
    }
}
