using ApiProject.DatabaseAccess.Entities;

namespace ApiProject.Tests.NUnit.DataAccess.Entities;

[TestFixture]
public class ThesisOfferDataAccessModelTests
{
    [Test]
    public void CanCreateThesisOfferDataAccessModel()
    {
        var model = new ThesisOfferDataAccessModel { Title = "Test", SubjectAreaId = Guid.NewGuid(), TutorId = Guid.NewGuid(), ThesisOfferStatusId = Guid.NewGuid() };
        Assert.That(model, Is.Not.Null);
    }

    [Test]
    public void CanSetTitle()
    {
        var title = "Test";
        var model = new ThesisOfferDataAccessModel { Title = title, SubjectAreaId = Guid.NewGuid(), TutorId = Guid.NewGuid(), ThesisOfferStatusId = Guid.NewGuid() };
        Assert.That(model.Title, Is.EqualTo(title));
    }

    [Test]
    public void CanSetDescription()
    {
        var description = "Test";
        var model = new ThesisOfferDataAccessModel { Title = "Test", Description = description, SubjectAreaId = Guid.NewGuid(), TutorId = Guid.NewGuid(), ThesisOfferStatusId = Guid.NewGuid() };
        Assert.That(model.Description, Is.EqualTo(description));
    }

    [Test]
    public void CanSetSubjectAreaId()
    {
        var subjectAreaId = Guid.NewGuid();
        var model = new ThesisOfferDataAccessModel { Title = "Test", SubjectAreaId = subjectAreaId, TutorId = Guid.NewGuid(), ThesisOfferStatusId = Guid.NewGuid() };
        Assert.That(model.SubjectAreaId, Is.EqualTo(subjectAreaId));
    }

    [Test]
    public void CanSetTutorId()
    {
        var tutorId = Guid.NewGuid();
        var model = new ThesisOfferDataAccessModel { Title = "Test", SubjectAreaId = Guid.NewGuid(), TutorId = tutorId, ThesisOfferStatusId = Guid.NewGuid() };
        Assert.That(model.TutorId, Is.EqualTo(tutorId));
    }

    [Test]
    public void CanSetThesisOfferStatusId()
    {
        var thesisOfferStatusId = Guid.NewGuid();
        var model = new ThesisOfferDataAccessModel { Title = "Test", SubjectAreaId = Guid.NewGuid(), TutorId = Guid.NewGuid(), ThesisOfferStatusId = thesisOfferStatusId };
        Assert.That(model.ThesisOfferStatusId, Is.EqualTo(thesisOfferStatusId));
    }

    [Test]
    public void CanSetMaxStudents()
    {
        var maxStudents = 5;
        var model = new ThesisOfferDataAccessModel { Title = "Test", SubjectAreaId = Guid.NewGuid(), TutorId = Guid.NewGuid(), ThesisOfferStatusId = Guid.NewGuid(), MaxStudents = maxStudents };
        Assert.That(model.MaxStudents, Is.EqualTo(maxStudents));
    }

    [Test]
    public void CanSetExpiresAt()
    {
        var expiresAt = DateTime.Now;
        var model = new ThesisOfferDataAccessModel { Title = "Test", SubjectAreaId = Guid.NewGuid(), TutorId = Guid.NewGuid(), ThesisOfferStatusId = Guid.NewGuid(), ExpiresAt = expiresAt };
        Assert.That(model.ExpiresAt, Is.EqualTo(expiresAt));
    }
}
