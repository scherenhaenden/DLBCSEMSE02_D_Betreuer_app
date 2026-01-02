using ApiProject.DatabaseAccess.Entities;
using NUnit.Framework;

namespace ApiProject.Tests.NUnit.DataAccess.Entities;

[TestFixture]
public class ThesisDocumentDataAccessModelTests
{
    [Test]
    public void CanCreateThesisDocumentDataAccessModel()
    {
        var model = new ThesisDocumentDataAccessModel { FileName = "Test", ContentType = "Test", Content = new byte[] { 1 } };
        Assert.That(model, Is.Not.Null);
    }

    [Test]
    public void CanSetFileName()
    {
        var fileName = "Test";
        var model = new ThesisDocumentDataAccessModel { FileName = fileName, ContentType = "Test", Content = new byte[] { 1 } };
        Assert.That(model.FileName, Is.EqualTo(fileName));
    }

    [Test]
    public void CanSetContentType()
    {
        var contentType = "Test";
        var model = new ThesisDocumentDataAccessModel { FileName = "Test", ContentType = contentType, Content = new byte[] { 1 } };
        Assert.That(model.ContentType, Is.EqualTo(contentType));
    }

    [Test]
    public void CanSetContent()
    {
        var content = new byte[] { 1, 2, 3 };
        var model = new ThesisDocumentDataAccessModel { FileName = "Test", ContentType = "Test", Content = content };
        Assert.That(model.Content, Is.EqualTo(content));
    }

    [Test]
    public void CanSetThesisId()
    {
        var thesisId = Guid.NewGuid();
        var model = new ThesisDocumentDataAccessModel { FileName = "Test", ContentType = "Test", Content = new byte[] { 1 }, ThesisId = thesisId };
        Assert.That(model.ThesisId, Is.EqualTo(thesisId));
    }
}
