using ApiProject.DatabaseAccess.Entities;
using NUnit.Framework;

namespace ApiProject.Tests.NUnit.DataAccess.Entities;

[TestFixture]
public class ThesisRequestDataAccessModelTests
{
    [Test]
    public void CanCreateThesisRequestDataAccessModel()
    {
        var model = new ThesisRequestDataAccessModel { RequesterId = Guid.NewGuid(), ReceiverId = Guid.NewGuid(), ThesisId = Guid.NewGuid(), RequestTypeId = Guid.NewGuid(), StatusId = Guid.NewGuid() };
        Assert.That(model, Is.Not.Null);
    }

    [Test]
    public void CanSetRequesterId()
    {
        var requesterId = Guid.NewGuid();
        var model = new ThesisRequestDataAccessModel { RequesterId = requesterId, ReceiverId = Guid.NewGuid(), ThesisId = Guid.NewGuid(), RequestTypeId = Guid.NewGuid(), StatusId = Guid.NewGuid() };
        Assert.That(model.RequesterId, Is.EqualTo(requesterId));
    }

    [Test]
    public void CanSetReceiverId()
    {
        var receiverId = Guid.NewGuid();
        var model = new ThesisRequestDataAccessModel { RequesterId = Guid.NewGuid(), ReceiverId = receiverId, ThesisId = Guid.NewGuid(), RequestTypeId = Guid.NewGuid(), StatusId = Guid.NewGuid() };
        Assert.That(model.ReceiverId, Is.EqualTo(receiverId));
    }

    [Test]
    public void CanSetThesisId()
    {
        var thesisId = Guid.NewGuid();
        var model = new ThesisRequestDataAccessModel { RequesterId = Guid.NewGuid(), ReceiverId = Guid.NewGuid(), ThesisId = thesisId, RequestTypeId = Guid.NewGuid(), StatusId = Guid.NewGuid() };
        Assert.That(model.ThesisId, Is.EqualTo(thesisId));
    }

    [Test]
    public void CanSetRequestTypeId()
    {
        var requestTypeId = Guid.NewGuid();
        var model = new ThesisRequestDataAccessModel { RequesterId = Guid.NewGuid(), ReceiverId = Guid.NewGuid(), ThesisId = Guid.NewGuid(), RequestTypeId = requestTypeId, StatusId = Guid.NewGuid() };
        Assert.That(model.RequestTypeId, Is.EqualTo(requestTypeId));
    }

    [Test]
    public void CanSetStatusId()
    {
        var statusId = Guid.NewGuid();
        var model = new ThesisRequestDataAccessModel { RequesterId = Guid.NewGuid(), ReceiverId = Guid.NewGuid(), ThesisId = Guid.NewGuid(), RequestTypeId = Guid.NewGuid(), StatusId = statusId };
        Assert.That(model.StatusId, Is.EqualTo(statusId));
    }

    [Test]
    public void CanSetMessage()
    {
        var message = "Test";
        var model = new ThesisRequestDataAccessModel { RequesterId = Guid.NewGuid(), ReceiverId = Guid.NewGuid(), ThesisId = Guid.NewGuid(), RequestTypeId = Guid.NewGuid(), StatusId = Guid.NewGuid(), Message = message };
        Assert.That(model.Message, Is.EqualTo(message));
    }
}
