using ApiProject.DatabaseAccess.Entities;

namespace ApiProject.Tests.NUnit.DataAccess.Entities;

[TestFixture]
public class RoleDataAccessModelTests
{
    [Test]
    public void CanCreateRoleDataAccessModel()
    {
        var model = new RoleDataAccessModel { Name = "Test" };
        Assert.That(model, Is.Not.Null);
    }

    [Test]
    public void CanSetName()
    {
        var name = "Test";
        var model = new RoleDataAccessModel { Name = name };
        Assert.That(model.Name, Is.EqualTo(name));
    }
}
