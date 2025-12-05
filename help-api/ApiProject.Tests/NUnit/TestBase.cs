using ApiProject.Db.Context;
using Microsoft.EntityFrameworkCore;

namespace ApiProject.Tests.NUnit;

/// <summary>
/// Base class for tests that require a database context.
/// </summary>
public class TestBase
{
    protected ThesisDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<ThesisDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ThesisDbContext(options);
    }
}
