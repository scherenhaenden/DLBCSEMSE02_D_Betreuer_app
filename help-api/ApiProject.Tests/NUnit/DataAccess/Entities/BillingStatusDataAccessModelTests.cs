using ApiProject.DatabaseAccess.Entities;

namespace ApiProject.Tests.NUnit.DataAccess.Entities;

[TestFixture]
public class BillingStatusDataAccessModelTests
{
    [Test]
    public void CanCreateBillingStatusDataAccessModel()
    {
        var model = new BillingStatusDataAccessModel { Name = "Test" };
        Assert.That(model, Is.Not.Null);
    }

    [Test]
    public void CanSetName()
    {
        var name = "Test";
        var model = new BillingStatusDataAccessModel { Name = name };
        Assert.That(model.Name, Is.EqualTo(name));
    }
}
