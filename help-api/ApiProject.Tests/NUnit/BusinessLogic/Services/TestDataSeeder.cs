using ApiProject.DatabaseAccess.Context;
using ApiProject.DatabaseAccess.Entities;
using ApiProject.Constants;

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
                new RoleDataAccessModel { Id = Guid.NewGuid(), Name = Roles.Student },
                new RoleDataAccessModel { Id = Guid.NewGuid(), Name = Roles.Tutor },
                new RoleDataAccessModel { Id = Guid.NewGuid(), Name = Roles.Admin }
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

    public void SeedUserToSubjectArea(Guid userId, Guid subjectAreaId)
    {
        if (!_context.UserToSubjectAreas.Any(ut => ut.UserId == userId && ut.SubjectAreaId == subjectAreaId))
        {
            _context.UserToSubjectAreas.Add(new UserToSubjectAreas { UserId = userId, SubjectAreaId = subjectAreaId });
            _context.SaveChanges();
        }
    }

    public void SeedThesisOfferStatuses()
    {
        if (!_context.ThesisOfferStatuses.Any())
        {
            _context.ThesisOfferStatuses.AddRange(
                new ThesisOfferStatusDataAccessModel { Id = Guid.NewGuid(), Name = ThesisOfferStatuses.Open, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new ThesisOfferStatusDataAccessModel { Id = Guid.NewGuid(), Name = ThesisOfferStatuses.Closed, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new ThesisOfferStatusDataAccessModel { Id = Guid.NewGuid(), Name = ThesisOfferStatuses.Archived, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
            );
            _context.SaveChanges();
        }
    }

    public void SeedRequestStatuses()
    {
        if (!_context.RequestStatuses.Any())
        {
            _context.RequestStatuses.AddRange(
                new RequestStatusDataAccessModel { Id = Guid.NewGuid(), Name = RequestStatuses.Pending, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new RequestStatusDataAccessModel { Id = Guid.NewGuid(), Name = RequestStatuses.Accepted, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new RequestStatusDataAccessModel { Id = Guid.NewGuid(), Name = RequestStatuses.Rejected, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
            );
            _context.SaveChanges();
        }
    }

    public void SeedRequestTypes()
    {
        if (!_context.RequestTypes.Any())
        {
            _context.RequestTypes.AddRange(
                new RequestTypeDataAccessModel { Id = Guid.NewGuid(), Name = RequestTypes.Supervision, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new RequestTypeDataAccessModel { Id = Guid.NewGuid(), Name = RequestTypes.CoSupervision, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
            );
            _context.SaveChanges();
        }
    }

    public ThesisOfferDataAccessModel SeedThesisOffer(string title, string description, Guid subjectAreaId, Guid tutorId, int maxStudents, DateTime expiresAt)
    {
        var openStatus = _context.ThesisOfferStatuses.First(s => s.Name == ThesisOfferStatuses.Open);
        var offer = new ThesisOfferDataAccessModel
        {
            Id = Guid.NewGuid(),
            Title = title,
            Description = description,
            SubjectAreaId = subjectAreaId,
            TutorId = tutorId,
            ThesisOfferStatusId = openStatus.Id,
            MaxStudents = maxStudents,
            ExpiresAt = expiresAt,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.ThesisOffers.Add(offer);
        _context.SaveChanges();
        return offer;
    }

    public ThesisOfferDataAccessModel SeedThesisOfferWithStatus(string title, string description, Guid subjectAreaId, Guid tutorId, int maxStudents, DateTime expiresAt, Guid statusId)
    {
        var offer = new ThesisOfferDataAccessModel
        {
            Id = Guid.NewGuid(),
            Title = title,
            Description = description,
            SubjectAreaId = subjectAreaId,
            TutorId = tutorId,
            ThesisOfferStatusId = statusId,
            MaxStudents = maxStudents,
            ExpiresAt = expiresAt,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.ThesisOffers.Add(offer);
        _context.SaveChanges();
        return offer;
    }

    public ThesisOfferApplicationDataAccessModel SeedThesisOfferApplication(Guid offerId, Guid studentId, string message)
    {
        var pendingStatus = _context.RequestStatuses.First(s => s.Name == RequestStatuses.Pending);
        var application = new ThesisOfferApplicationDataAccessModel
        {
            Id = Guid.NewGuid(),
            ThesisOfferId = offerId,
            StudentId = studentId,
            RequestStatusId = pendingStatus.Id,
            Message = message,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.ThesisOfferApplications.Add(application);
        _context.SaveChanges();
        return application;
    }

    public ThesisRequestDataAccessModel SeedThesisRequest(Guid requesterId, Guid receiverId, Guid thesisId, string requestType, string status, string message)
    {
        var requestTypeEntity = _context.RequestTypes.First(rt => rt.Name == requestType);
        var statusEntity = _context.RequestStatuses.First(rs => rs.Name == status);
        var request = new ThesisRequestDataAccessModel
        {
            Id = Guid.NewGuid(),
            RequesterId = requesterId,
            ReceiverId = receiverId,
            ThesisId = thesisId,
            RequestTypeId = requestTypeEntity.Id,
            StatusId = statusEntity.Id,
            Message = message,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.ThesisRequests.Add(request);
        _context.SaveChanges();
        return request;
    }
}
