using ApiProject.DatabaseAccess.Entities;
using NUnit.Framework;

namespace ApiProject.Tests.NUnit.DataAccess.Entities;

[TestFixture]
public class UserTopicDataAccessModelTests
{
    [Test]
    public void CanCreateUserTopicDataAccessModel()
    {
        var model = new UserTopicDataAccessModel();
        Assert.That(model, Is.Not.Null);
    }

    [Test]
    public void CanSetUserId()
    {
        var userId = Guid.NewGuid();
        var model = new UserTopicDataAccessModel { UserId = userId };
        Assert.That(model.UserId, Is.EqualTo(userId));
    }

    [Test]
    public void CanSetTopicId()
    {
        var topicId = Guid.NewGuid();
        var model = new UserTopicDataAccessModel { TopicId = topicId };
        Assert.That(model.TopicId, Is.EqualTo(topicId));
    }
}