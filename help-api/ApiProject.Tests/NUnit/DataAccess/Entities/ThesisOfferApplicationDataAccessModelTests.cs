using ApiProject.DatabaseAccess.Entities;

namespace ApiProject.Tests.NUnit.DataAccess.Entities;

[TestFixture]
public class ThesisOfferApplicationDataAccessModelTests
{
    [Test]
    public void CanCreateThesisOfferApplicationDataAccessModel()
    {
        var model = new ThesisOfferApplicationDataAccessModel { ThesisOfferId = Guid.NewGuid(), StudentId = Guid.NewGuid(), RequestStatusId = Guid.NewGuid() };
        Assert.That(model, Is.Not.Null);
    }

    [Test]
    public void CanSetThesisOfferId()
    {
        var thesisOfferId = Guid.NewGuid();
        var model = new ThesisOfferApplicationDataAccessModel { ThesisOfferId = thesisOfferId, StudentId = Guid.NewGuid(), RequestStatusId = Guid.NewGuid() };
        Assert.That(model.ThesisOfferId, Is.EqualTo(thesisOfferId));
    }

    [Test]
    public void CanSetStudentId()
    {
        var studentId = Guid.NewGuid();
        var model = new ThesisOfferApplicationDataAccessModel { ThesisOfferId = Guid.NewGuid(), StudentId = studentId, RequestStatusId = Guid.NewGuid() };
        Assert.That(model.StudentId, Is.EqualTo(studentId));
    }

    [Test]
    public void CanSetRequestStatusId()
    {
        var requestStatusId = Guid.NewGuid();
        var model = new ThesisOfferApplicationDataAccessModel { ThesisOfferId = Guid.NewGuid(), StudentId = Guid.NewGuid(), RequestStatusId = requestStatusId };
        Assert.That(model.RequestStatusId, Is.EqualTo(requestStatusId));
    }

    [Test]
    public void CanSetMessage()
    {
        var message = "Test";
        var model = new ThesisOfferApplicationDataAccessModel { ThesisOfferId = Guid.NewGuid(), StudentId = Guid.NewGuid(), RequestStatusId = Guid.NewGuid(), Message = message };
        Assert.That(model.Message, Is.EqualTo(message));
    }
}
