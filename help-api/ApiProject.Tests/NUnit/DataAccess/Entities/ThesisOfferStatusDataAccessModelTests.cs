using ApiProject.DatabaseAccess.Entities;

namespace ApiProject.Tests.NUnit.DataAccess.Entities;

[TestFixture]
public class ThesisOfferStatusDataAccessModelTests
{
    [Test]
    public void CanCreateThesisOfferStatusDataAccessModel()
    {
        var model = new ThesisOfferStatusDataAccessModel { Name = "Test" };
        Assert.That(model, Is.Not.Null);
    }

    [Test]
    public void CanSetName()
    {
        var name = "Test";
        var model = new ThesisOfferStatusDataAccessModel { Name = name };
        Assert.That(model.Name, Is.EqualTo(name));
    }
}
