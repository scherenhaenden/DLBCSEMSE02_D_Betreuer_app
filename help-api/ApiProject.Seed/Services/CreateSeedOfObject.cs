using System.Text.Json;
using Bogus;
using System.Linq.Expressions;

namespace ApiProject.Seed.Services;

public class CreateSeedOfObject : ICreateSeedOfObject
{
    public string GenerateJson<T>(int count, Expression<Func<Faker<T>, Faker<T>>> configure) where T : class
    {
        var faker = new Faker<T>();
        var configuredFaker = configure.Compile()(faker);
        var objects = configuredFaker.Generate(count);
        return JsonSerializer.Serialize(objects, new JsonSerializerOptions { WriteIndented = true });
    }   
    
    
}
