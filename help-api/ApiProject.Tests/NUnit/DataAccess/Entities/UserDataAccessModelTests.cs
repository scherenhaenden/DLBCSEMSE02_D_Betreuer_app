using ApiProject.DatabaseAccess.Entities;

namespace ApiProject.Tests.NUnit.DataAccess.Entities;

[TestFixture]
public class UserDataAccessModelTests
{
    [Test]
    public void CanCreateUserDataAccessModel()
    {
        var model = new UserDataAccessModel { FirstName = "Test", LastName = "Test", Email = "test@test.com", PasswordHash = "hash" };
        Assert.That(model, Is.Not.Null);
    }

    [Test]
    public void CanSetFirstName()
    {
        var firstName = "Test";
        var model = new UserDataAccessModel { FirstName = firstName, LastName = "Test", Email = "test@test.com", PasswordHash = "hash" };
        Assert.That(model.FirstName, Is.EqualTo(firstName));
    }

    [Test]
    public void CanSetLastName()
    {
        var lastName = "Test";
        var model = new UserDataAccessModel { FirstName = "Test", LastName = lastName, Email = "test@test.com", PasswordHash = "hash" };
        Assert.That(model.LastName, Is.EqualTo(lastName));
    }

    [Test]
    public void CanSetEmail()
    {
        var email = "test@test.com";
        var model = new UserDataAccessModel { FirstName = "Test", LastName = "Test", Email = email, PasswordHash = "hash" };
        Assert.That(model.Email, Is.EqualTo(email));
    }

    [Test]
    public void CanSetPasswordHash()
    {
        var passwordHash = "hash";
        var model = new UserDataAccessModel { FirstName = "Test", LastName = "Test", Email = "test@test.com", PasswordHash = passwordHash };
        Assert.That(model.PasswordHash, Is.EqualTo(passwordHash));
    }
}
