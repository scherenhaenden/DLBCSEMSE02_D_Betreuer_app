using ApiProject.DatabaseAccess.Entities;
using NUnit.Framework;

namespace ApiProject.Tests.NUnit.DataAccess.Entities;

[TestFixture]
public class ThesisDataAccessModelTests
{
    [Test]
    public void CanCreateThesisDataAccessModel()
    {
        var model = new ThesisDataAccessModel { Title = "Test" };
        Assert.That(model, Is.Not.Null);
    }

    [Test]
    public void CanSetTitle()
    {
        var title = "Test";
        var model = new ThesisDataAccessModel { Title = title };
        Assert.That(model.Title, Is.EqualTo(title));
    }

    [Test]
    public void CanSetStatusId()
    {
        var statusId = Guid.NewGuid();
        var model = new ThesisDataAccessModel { Title = "Test", StatusId = statusId };
        Assert.That(model.StatusId, Is.EqualTo(statusId));
    }

    [Test]
    public void CanSetBillingStatusId()
    {
        var billingStatusId = Guid.NewGuid();
        var model = new ThesisDataAccessModel { Title = "Test", BillingStatusId = billingStatusId };
        Assert.That(model.BillingStatusId, Is.EqualTo(billingStatusId));
    }

    [Test]
    public void CanSetOwnerId()
    {
        var ownerId = Guid.NewGuid();
        var model = new ThesisDataAccessModel { Title = "Test", OwnerId = ownerId };
        Assert.That(model.OwnerId, Is.EqualTo(ownerId));
    }

    [Test]
    public void CanSetTutorId()
    {
        var tutorId = Guid.NewGuid();
        var model = new ThesisDataAccessModel { Title = "Test", TutorId = tutorId };
        Assert.That(model.TutorId, Is.EqualTo(tutorId));
    }

    [Test]
    public void CanSetSecondSupervisorId()
    {
        var secondSupervisorId = Guid.NewGuid();
        var model = new ThesisDataAccessModel { Title = "Test", SecondSupervisorId = secondSupervisorId };
        Assert.That(model.SecondSupervisorId, Is.EqualTo(secondSupervisorId));
    }

    [Test]
    public void CanSetSubjectAreaId()
    {
        var subjectAreaId = Guid.NewGuid();
        var model = new ThesisDataAccessModel { Title = "Test", SubjectAreaId = subjectAreaId };
        Assert.That(model.SubjectAreaId, Is.EqualTo(subjectAreaId));
    }
}
