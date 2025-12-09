using ApiProject.DatabaseAccess.Context;
using ApiProject.DatabaseAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApiProject.Tests.NUnit.DataAccess.Context;

[TestFixture]
public class ThesisDbContextTests
{
    private ThesisDbContext _context;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<ThesisDbContext>()
            // Using SQLite with a physical file for more realistic testing.
            .UseSqlite("Data Source=TestDb.db")
            .Options;
        _context = new ThesisDbContext(options);
        _context.Database.EnsureCreated();
    }

    [TearDown]
    public void TearDown()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Test]
    public void DbContext_CanBeCreated()
    {
        Assert.That(_context, Is.Not.Null);
    }

    [Test]
    public void DbSets_AreNotNull()
    {
        Assert.That(_context.Users, Is.Not.Null);
        Assert.That(_context.Roles, Is.Not.Null);
        Assert.That(_context.UserRoles, Is.Not.Null);
        Assert.That(_context.Theses, Is.Not.Null);
        Assert.That(_context.Topics, Is.Not.Null);
        Assert.That(_context.UserTopics, Is.Not.Null);
        Assert.That(_context.ThesisStatuses, Is.Not.Null);
        Assert.That(_context.BillingStatuses, Is.Not.Null);
        Assert.That(_context.ThesisDocuments, Is.Not.Null);
    }

    [Test]
    public void CanAddUser()
    {
        var user = new UserDataAccessModel
        {
            Id = Guid.NewGuid(),
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            PasswordHash = "hashedpassword"
        };

        _context.Users.Add(user);
        _context.SaveChanges();

        var retrieved = _context.Users.Find(user.Id);
        Assert.That(retrieved, Is.Not.Null);
        Assert.That(retrieved.FirstName, Is.EqualTo("John"));
    }

    [Test]
    public void CanAddThesis()
    {
        var owner = new UserDataAccessModel { Id = Guid.NewGuid(), FirstName = "Owner", LastName = "User", Email = "owner@example.com", PasswordHash = "hash" };
        var tutor = new UserDataAccessModel { Id = Guid.NewGuid(), FirstName = "Tutor", LastName = "User", Email = "tutor@example.com", PasswordHash = "hash" };
        var status = new ThesisStatusDataAccessModel { Id = Guid.NewGuid(), Name = "PendingApproval" };
        var billingStatus = new BillingStatusDataAccessModel { Id = Guid.NewGuid(), Name = "None" };

        _context.Users.AddRange(owner, tutor);
        _context.ThesisStatuses.Add(status);
        _context.BillingStatuses.Add(billingStatus);

        var thesis = new ThesisDataAccessModel
        {
            Id = Guid.NewGuid(),
            Title = "Test Thesis",
            SubjectArea = "Computer Science",
            OwnerId = owner.Id,
            TutorId = tutor.Id,
            StatusId = status.Id,
            BillingStatusId = billingStatus.Id
        };

        _context.Theses.Add(thesis);
        _context.SaveChanges();

        var retrieved = _context.Theses.Find(thesis.Id);
        Assert.That(retrieved, Is.Not.Null);
        Assert.That(retrieved.Title, Is.EqualTo("Test Thesis"));
    }

    [Test]
    public void CanAddRole()
    {
        var role = new RoleDataAccessModel
        {
            Id = Guid.NewGuid(),
            Name = "STUDENT"
        };

        _context.Roles.Add(role);
        _context.SaveChanges();

        var retrieved = _context.Roles.Find(role.Id);
        Assert.That(retrieved, Is.Not.Null);
        Assert.That(retrieved.Name, Is.EqualTo("STUDENT"));
    }

    [Test]
    public void CanAddTopic()
    {
        var topic = new TopicDataAccessModel
        {
            Id = Guid.NewGuid(),
            Title = "Test Topic",
            Description = "Description",
            SubjectArea = "CS",
            IsActive = true
        };

        _context.Topics.Add(topic);
        _context.SaveChanges();

        var retrieved = _context.Topics.Find(topic.Id);
        Assert.That(retrieved, Is.Not.Null);
        Assert.That(retrieved.Title, Is.EqualTo("Test Topic"));
    }

    [Test]
    public void CanQueryUsers()
    {
        var user1 = new UserDataAccessModel { Id = Guid.NewGuid(), FirstName = "Alice", LastName = "Smith", Email = "alice@example.com", PasswordHash = "hash" };
        var user2 = new UserDataAccessModel { Id = Guid.NewGuid(), FirstName = "Bob", LastName = "Johnson", Email = "bob@example.com", PasswordHash = "hash" };

        _context.Users.AddRange(user1, user2);
        _context.SaveChanges();

        var users = _context.Users.ToList();
        Assert.That(users.Count, Is.EqualTo(2));
    }

    [Test]
    public void UserRoleRelationship_Works()
    {
        var user = new UserDataAccessModel { Id = Guid.NewGuid(), FirstName = "Test", LastName = "User", Email = "test@example.com", PasswordHash = "hash" };
        var role = new RoleDataAccessModel { Id = Guid.NewGuid(), Name = "TUTOR" };
        var userRole = new UserRoleDataAccessModel { UserId = user.Id, RoleId = role.Id };

        _context.Users.Add(user);
        _context.Roles.Add(role);
        _context.UserRoles.Add(userRole);
        _context.SaveChanges();

        var retrievedUser = _context.Users.Include(u => u.UserRoles).ThenInclude(ur => ur.Role).FirstOrDefault(u => u.Id == user.Id);
        Assert.That(retrievedUser, Is.Not.Null);
        Assert.That(retrievedUser.UserRoles.Count, Is.EqualTo(1));
        Assert.That(retrievedUser.UserRoles.First().Role.Name, Is.EqualTo("TUTOR"));
    }

    [Test]
    public void ThesisStatusRelationship_Works()
    {
        var owner = new UserDataAccessModel { Id = Guid.NewGuid(), FirstName = "Owner", LastName = "User", Email = "owner@example.com", PasswordHash = "hash" };
        var tutor = new UserDataAccessModel { Id = Guid.NewGuid(), FirstName = "Tutor", LastName = "User", Email = "tutor@example.com", PasswordHash = "hash" };
        var status = new ThesisStatusDataAccessModel { Id = Guid.NewGuid(), Name = "PendingApproval" };
        var billingStatus = new BillingStatusDataAccessModel { Id = Guid.NewGuid(), Name = "None" };

        _context.Users.AddRange(owner, tutor);
        _context.ThesisStatuses.Add(status);
        _context.BillingStatuses.Add(billingStatus);

        var thesis = new ThesisDataAccessModel
        {
            Id = Guid.NewGuid(),
            Title = "Thesis",
            SubjectArea = "CS",
            OwnerId = owner.Id,
            TutorId = tutor.Id,
            StatusId = status.Id,
            BillingStatusId = billingStatus.Id
        };

        _context.Theses.Add(thesis);
        _context.SaveChanges();

        var retrievedThesis = _context.Theses.Include(t => t.Status).FirstOrDefault(t => t.Id == thesis.Id);
        Assert.That(retrievedThesis, Is.Not.Null);
        Assert.That(retrievedThesis.Status.Name, Is.EqualTo("PendingApproval"));
    }
}