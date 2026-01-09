using ApiProject.DatabaseAccess.Entities;

namespace ApiProject.Tests.NUnit.DataAccess.Entities;

[TestFixture]
public class UserToSubjectAreasTests
{
    [Test]
    public void CanCreateUserToSubjectAreasDataAccessModel()
    {
        var model = new UserToSubjectAreas();
        Assert.That(model, Is.Not.Null);
    }

    [Test]
    public void CanSetUserId()
    {
        var userId = Guid.NewGuid();
        var model = new UserToSubjectAreas { UserId = userId };
        Assert.That(model.UserId, Is.EqualTo(userId));
    }

    [Test]
    public void CanSetSubjectAreaId()
    {
        var subjectAreaId = Guid.NewGuid();
        var model = new UserToSubjectAreas { SubjectAreaId = subjectAreaId };
        Assert.That(model.SubjectAreaId, Is.EqualTo(subjectAreaId));
    }
}