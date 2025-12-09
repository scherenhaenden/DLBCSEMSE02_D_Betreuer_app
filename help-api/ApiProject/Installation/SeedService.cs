using System.Text.Json;
using ApiProject.DatabaseAccess.Context;
using Microsoft.Extensions.Options;

namespace ApiProject.Installation;

public class SeedService 
{
    private readonly ThesisDbContext _context;
    private readonly IWebHostEnvironment _env;
    private readonly IOptions<AppSettings> _settings;

    public SeedService(ThesisDbContext context, IWebHostEnvironment env, IOptions<AppSettings> settings)
    {
        _context = context;
        _env = env;
        _settings = settings;
    }

    public async Task SeedAsync()
    {
        if (!await IsDatabasePresentAsync())
        {
            await _context.Database.EnsureCreatedAsync();
            string relativePath = _settings.Value.Database.SeedJsonPath.Replace("wwwroot/", "");
            string jsonPath = Path.Combine(_env.WebRootPath, relativePath);
            if (File.Exists(jsonPath))
            {
                string json = await File.ReadAllTextAsync(jsonPath);
                var seedData = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);
                if (seedData != null)
                {
                    foreach (var kvp in seedData)
                    {
                        var dbSetProperty = _context.GetType().GetProperty(kvp.Key);
                        if (dbSetProperty != null)
                        {
                            var dbSet = dbSetProperty.GetValue(_context);
                            var addRangeMethod = dbSet.GetType().GetMethod("AddRangeAsync", new[] { typeof(IEnumerable<>).MakeGenericType(dbSetProperty.PropertyType.GetGenericArguments()[0]) });
                            if (addRangeMethod != null)
                            {
                                var entityType = dbSetProperty.PropertyType.GetGenericArguments()[0];
                                var listType = typeof(List<>).MakeGenericType(entityType);
                                var list = JsonSerializer.Deserialize(kvp.Value.GetRawText(), listType);
                                if (list != null)
                                {
                                    await (Task)addRangeMethod.Invoke(dbSet, new[] { list });
                                }
                            }
                        }
                    }
                    await _context.SaveChangesAsync();
                }
            }
        }
    }

    private async Task<bool> IsDatabasePresentAsync()
    {
        return await _context.Database.CanConnectAsync();
    }
}