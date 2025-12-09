using ApiProject.DatabaseAccess.Entities;
using ApiProject.Seed.Services;
using Bogus;
using System.Text.Json;
using Bogus;
using System.Linq.Expressions;

namespace ApiProject.Tests.CreateSeed;

[TestFixture]
public class Seeder
{
    [Test]
    public void CreateSeed()
    {
        ICreateSeedOfObject createSeedOfObject = new CreateSeedOfObject();
        
        
        var roleNames = new[] { "STUDENT", "TUTOR", "ADMIN" };
        var rolesToSeed = new List<RoleDataAccessModel>();

        foreach (var roleName in roleNames)
        {
            var role = new Faker<RoleDataAccessModel>()
                .RuleFor(r => r.Id, f => Guid.NewGuid())
                .RuleFor(r => r.Name, roleName) // Set the specific role name
                .RuleFor(r => r.CreatedAt, f => f.Date.Recent())
                .RuleFor(r => r.UpdatedAt, (f, r) => r.CreatedAt)
                .Generate();
    
            rolesToSeed.Add(role);
        }

        var userFaker = new Faker<UserDataAccessModel>()
            .RuleFor(u => u.FirstName, f => f.Name.FirstName())
            .RuleFor(u => u.LastName, f => f.Name.LastName())
            .RuleFor(u => u.Email, f => f.Internet.Email())
            .RuleFor(u => u.PasswordHash, f => f.Internet.Password())
            .RuleFor(u => u.Id, f => Guid.NewGuid())
            .RuleFor(u => u.CreatedAt, f => f.Date.Past())
            .RuleFor(u => u.UpdatedAt, f => f.Date.Recent());
        
        var users = userFaker.Generate(100);

        var userRolesToSeed = new List<UserRoleDataAccessModel>();
        var faker = new Faker();

        foreach (var user in users)
        {
            var randomRole = faker.PickRandom(rolesToSeed);
            var userRole = new UserRoleDataAccessModel
            {
                UserId = user.Id,
                RoleId = randomRole.Id
            };
            userRolesToSeed.Add(userRole);
        }
        
        var seedData = new { Users = users, Roles = rolesToSeed, UserRoles = userRolesToSeed };
        

        var json = JsonSerializer.Serialize(seedData, new JsonSerializerOptions { WriteIndented = true });

    } 
}
