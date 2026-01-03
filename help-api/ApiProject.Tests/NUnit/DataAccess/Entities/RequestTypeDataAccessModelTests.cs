using ApiProject.DatabaseAccess.Entities;
using NUnit.Framework;

namespace ApiProject.Tests.NUnit.DataAccess.Entities;

[TestFixture]
public class RequestTypeDataAccessModelTests
{
    [Test]
    public void CanCreateRequestTypeDataAccessModel()
    {
        var model = new RequestTypeDataAccessModel { Name = "Test" };
        Assert.That(model, Is.Not.Null);
    }

    [Test]
    public void CanSetName()
    {
        var name = "Test";
        var model = new RequestTypeDataAccessModel { Name = name };
        Assert.That(model.Name, Is.EqualTo(name));
    }
}
