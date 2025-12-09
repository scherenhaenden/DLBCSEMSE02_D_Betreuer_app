using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using ApiProject.DatabaseAccess.Context;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ApiProject.Installation
{
    public class SeedService
    {
        private readonly ThesisDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _configuration;

        public SeedService(ThesisDbContext context, IWebHostEnvironment env, IConfiguration configuration)
        {
            _context = context;
            _env = env;
            _configuration = configuration;
        }

        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();

            // Check if the database is already seeded by looking for any users.
            if (await _context.Users.AnyAsync())
            {
                return; // Database already has data.
            }

            var seedJsonPath = _configuration["Database:SeedJsonPath"];
            if (string.IsNullOrEmpty(seedJsonPath))
            {
                return; // Seeding path not configured.
            }

            var fullPath = Path.Combine(_env.ContentRootPath, seedJsonPath);
            if (!File.Exists(fullPath))
            {
                return; // Seed file not found.
            }

            var json = await File.ReadAllTextAsync(fullPath);
            var seedData = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);

            if (seedData == null)
            {
                return;
            }

            foreach (var kvp in seedData)
            {
                var dbSetProperty = _context.GetType().GetProperty(kvp.Key);
                if (dbSetProperty == null) continue;

                var entityType = dbSetProperty.PropertyType.GetGenericArguments()[0];
                if (kvp.Value.ValueKind != JsonValueKind.Array) continue;

                foreach (var element in kvp.Value.EnumerateArray())
                {
                    try
                    {
                        var entity = JsonSerializer.Deserialize(element.GetRawText(), entityType, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        if (entity != null)
                        {
                            _context.Add(entity);
                        }
                    }
                    catch (JsonException)
                    {
                        // Log error or handle malformed JSON object
                    }
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
