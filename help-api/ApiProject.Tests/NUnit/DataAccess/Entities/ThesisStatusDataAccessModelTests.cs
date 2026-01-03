using ApiProject.DatabaseAccess.Entities;
using NUnit.Framework;

namespace ApiProject.Tests.NUnit.DataAccess.Entities;

[TestFixture]
public class ThesisStatusDataAccessModelTests
{
    [Test]
    public void CanCreateThesisStatusDataAccessModel()
    {
        var model = new ThesisStatusDataAccessModel { Name = "Test" };
        Assert.That(model, Is.Not.Null);
    }

    [Test]
    public void CanSetName()
    {
        var name = "Test";
        var model = new ThesisStatusDataAccessModel { Name = name };
        Assert.That(model.Name, Is.EqualTo(name));
    }
}
