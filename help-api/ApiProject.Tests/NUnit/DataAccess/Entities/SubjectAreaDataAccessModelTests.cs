using ApiProject.DatabaseAccess.Entities;

namespace ApiProject.Tests.NUnit.DataAccess.Entities;

[TestFixture]
public class SubjectAreaDataAccessModelTests
{
    [Test]
    public void CanCreateSubjectAreaDataAccessModel()
    {
        var model = new SubjectAreaDataAccessModel { Title = "Test", Description = "Test" };
        Assert.That(model, Is.Not.Null);
    }

    [Test]
    public void CanSetTitle()
    {
        var title = "Test";
        var model = new SubjectAreaDataAccessModel { Title = title, Description = "Test" };
        Assert.That(model.Title, Is.EqualTo(title));
    }

    [Test]
    public void CanSetDescription()
    {
        var description = "Test";
        var model = new SubjectAreaDataAccessModel { Title = "Test", Description = description };
        Assert.That(model.Description, Is.EqualTo(description));
    }

    [Test]
    public void CanSetIsActive()
    {
        var isActive = true;
        var model = new SubjectAreaDataAccessModel { Title = "Test", Description = "Test", IsActive = isActive };
        Assert.That(model.IsActive, Is.EqualTo(isActive));
    }
}
