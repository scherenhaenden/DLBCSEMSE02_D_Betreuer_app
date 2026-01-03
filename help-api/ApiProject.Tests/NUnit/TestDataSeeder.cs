using ApiProject.DatabaseAccess.Context;
using ApiProject.Tests.CreateSeed;

namespace ApiProject.Tests.NUnit;

public class TestDataSeeder
{
    private readonly ThesisDbContext _context;

    public TestDataSeeder(ThesisDbContext context)
    {
        _context = context;
    }

    public void SeedRoles()
    {
        var seeder = new Seeder();
        var seedData = seeder.CreateSeed();
        Seeder.SeedDatabase(_context, seedData);
    }
}
