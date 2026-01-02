using ApiProject.DatabaseAccess.Entities;
using NUnit.Framework;

namespace ApiProject.Tests.NUnit.DataAccess.Entities;

[TestFixture]
public class UserToSubjectAreasTests
{
    [Test]
    public void CanCreateUserTopicDataAccessModel()
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
    public void CanSetTopicId()
    {
        var topicId = Guid.NewGuid();
        var model = new UserToSubjectAreas { SubjectAreaId = topicId };
        Assert.That(model.SubjectAreaId, Is.EqualTo(topicId));
    }
}