using ApiProject.DatabaseAccess.Entities;

namespace ApiProject.Tests.NUnit.DataAccess.Entities;

[TestFixture]
public class UserRoleDataAccessModelTests
{
    [Test]
    public void CanCreateUserRoleDataAccessModel()
    {
        var model = new UserRoleDataAccessModel { UserId = Guid.NewGuid(), RoleId = Guid.NewGuid() };
        Assert.That(model, Is.Not.Null);
    }

    [Test]
    public void CanSetUserId()
    {
        var userId = Guid.NewGuid();
        var model = new UserRoleDataAccessModel { UserId = userId, RoleId = Guid.NewGuid() };
        Assert.That(model.UserId, Is.EqualTo(userId));
    }

    [Test]
    public void CanSetRoleId()
    {
        var roleId = Guid.NewGuid();
        var model = new UserRoleDataAccessModel { UserId = Guid.NewGuid(), RoleId = roleId };
        Assert.That(model.RoleId, Is.EqualTo(roleId));
    }
}
