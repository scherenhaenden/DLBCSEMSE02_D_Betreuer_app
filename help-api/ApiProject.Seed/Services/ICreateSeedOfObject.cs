using System.Linq.Expressions;

namespace ApiProject.Seed.Services;

public interface ICreateSeedOfObject
{
    string GenerateJson<T>(int count, Expression<Func<Bogus.Faker<T>, Bogus.Faker<T>>> configure) where T : class;
}