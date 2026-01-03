using ApiProject.DatabaseAccess.Entities;
using NUnit.Framework;

namespace ApiProject.Tests.NUnit.DataAccess.Entities;

[TestFixture]
public class RequestStatusDataAccessModelTests
{
    [Test]
    public void CanCreateRequestStatusDataAccessModel()
    {
        var model = new RequestStatusDataAccessModel { Name = "Test" };
        Assert.That(model, Is.Not.Null);
    }

    [Test]
    public void CanSetName()
    {
        var name = "Test";
        var model = new RequestStatusDataAccessModel { Name = name };
        Assert.That(model.Name, Is.EqualTo(name));
    }
}
