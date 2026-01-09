using ApiProject.DatabaseAccess.Context;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using ApiProject.ApiLogic.Models;
using Microsoft.Extensions.Configuration;

namespace ApiProject.Tests.NUnit.ApiLogic.Controllers;

[TestFixture]
public class AuthControllerTests
{
    private WebApplicationFactory<Program> _factory;
    private HttpClient _client;
    private ThesisDbContext _context;

    /*
     * AuthControllerTests SetUp Method Analysis
     * 
     * Overview:
     * The [SetUp] method in AuthControllerTests is the cornerstone of the integration testing framework.
     * It establishes a fully isolated, production-like test environment for each test method.
     * This comment provides a detailed breakdown of each operation within the SetUp method.
     * 
     * Method Signature and NUnit Context:
     * [SetUp] public void SetUp()
     * - [SetUp] is an NUnit attribute that marks a method to be executed before each test method in the fixture.
     * - Ensures a clean, consistent state for every test, preventing state pollution between tests.
     * - Runs once per test method, guaranteeing test isolation.
     * 
     * Step-by-Step Breakdown:
     * 
     * 1. WebApplicationFactory Initialization
     * _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder => { ... });
     * Purpose: Creates a test server that hosts the ASP.NET Core application.
     * Why WebApplicationFactory<Program>?
     * - Program is the entry point class containing Main() and application configuration.
     * - The generic parameter ensures the test server uses the same startup logic as production.
     * - Provides a real HTTP server for integration testing, unlike unit tests that mock the web layer.
     * WithWebHostBuilder: Allows customization of the host before the application starts, enabling test-specific configurations.
     * 
     * 2. Application Configuration Override
     * builder.ConfigureAppConfiguration((context, config) => { config.AddInMemoryCollection(new Dictionary<string, string> { ["Database:SeedJsonPath"] = null }); });
     * Purpose: Prevents automatic database seeding from JSON files during tests.
     * Deep Analysis:
     * - Production applications often seed initial data from JSON files specified by Database:SeedJsonPath.
     * - Tests require manual control over data to ensure predictability and performance.
     * - Setting the path to null disables this seeding mechanism.
     * - AddInMemoryCollection has higher precedence than appsettings.json, effectively overriding production settings.
     * Why This Matters:
     * - Performance: Avoids loading large seed datasets for each test.
     * - Predictability: Tests seed only necessary data, avoiding interference from production seed data.
     * - Isolation: Eliminates dependencies on external seed files that might change.
     * 
     * 3. Service Container Modification
     * builder.ConfigureServices(services => { var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ThesisDbContext>)); if (descriptor != null) { services.Remove(descriptor); } services.AddDbContext<ThesisDbContext>(options => { options.UseSqlite("Data Source=AuthControllerTests.db"); }); });
     * Purpose: Replaces the production database configuration with a test-specific SQLite database.
     * Service Descriptor Removal:
     * - Production registers DbContextOptions<ThesisDbContext> for the actual database (e.g., SQL Server).
     * - SingleOrDefault finds the existing registration to avoid conflicts.
     * - services.Remove(descriptor) eliminates the production configuration.
     * SQLite Database Addition:
     * - AddDbContext<ThesisDbContext> registers EF Core with SQLite provider.
     * - "Data Source=AuthControllerTests.db" specifies a file-based SQLite database.
     * - Unique filename prevents interference between test classes.
     * Why SQLite?
     * - Isolation: File-based database ensures each test class has its own data store.
     * - Performance: Faster than network databases, no connection overhead.
     * - Compatibility: Same EF Core LINQ queries work with SQLite as with production databases.
     * - Cleanup: Easy to delete the file after tests.
     * 
     * 4. HTTP Client Creation
     * _client = _factory.CreateClient();
     * Purpose: Creates an HttpClient configured to communicate with the test server.
     * Technical Details:
     * - CreateClient() returns an HttpClient with BaseAddress set to the test server's URL.
     * - Handles cookies, redirects, and other HTTP features automatically.
     * - Simulates real HTTP traffic through the full ASP.NET Core pipeline.
     * Why Not Direct Controller Testing?
     * - Integration tests validate the complete request-response cycle.
     * - Includes middleware, routing, model binding, and serialization.
     * - Catches issues that unit tests miss, like configuration errors.
     * 
     * 5. Dependency Injection Scope Creation
     * var scope = _factory.Services.CreateScope(); _context = scope.ServiceProvider.GetRequiredService<ThesisDbContext>();
     * Purpose: Obtains a reference to the test database context for direct data manipulation.
     * Scope Creation:
     * - CreateScope() creates a new DI scope, ensuring proper service lifetimes.
     * - Services resolved from this scope have the same lifetime management as in production.
     * DbContext Retrieval:
     * - GetRequiredService<ThesisDbContext>() gets the SQLite-configured context.
     * - Allows direct database operations for seeding and verification.
     * Why Direct Access?
     * - Seeding Efficiency: Bulk data operations via EF Core are faster than HTTP requests.
     * - State Verification: Tests can inspect database state after operations.
     * - Test Setup Flexibility: Enables complex data scenarios without HTTP overhead.
     * 
     * 6. Database Schema Creation
     * _context.Database.EnsureCreated();
     * Purpose: Creates the database schema based on EF Core entity configurations.
     * Technical Details:
     * - Applies EF Core migrations or code-first configurations.
     * - Creates tables, indexes, foreign keys, and constraints.
     * - Idempotent operation - safe to call multiple times.
     * Why Not Migrations?
     * - EnsureCreated() is simpler for tests than running migrations.
     * - Creates schema from current model definitions.
     * - Faster than migration scripts for test scenarios.
     * 
     * 7. Test Data Seeding
     * var seeder = new TestDataSeeder(_context); seeder.SeedRoles();
     * Purpose: Populates the database with minimal data required for authentication tests.
     * TestDataSeeder Usage:
     * - TestDataSeeder is a custom class that inserts essential data.
     * - SeedRoles() creates user roles (Student, Tutor) required for authentication.
     * - Only seeds what's necessary, keeping tests fast.
     * Why Manual Seeding?
     * - Minimalism: Avoids unnecessary data that slows tests.
     * - Control: Tests can seed different data for different scenarios.
     * - Predictability: Ensures consistent test state.
     * - Performance: Faster than loading large seed files.
     * 
     * Execution Flow and Timing:
     * Pre-Test Execution:
     * 1. NUnit identifies [SetUp] method
     * 2. SetUp runs before each [Test] method
     * 3. Clean environment established per test
     * 
     * Resource Management:
     * - Each SetUp creates new database file
     * - Test runs with isolated data
     * - TearDown cleans up resources
     * 
     * Performance Implications:
     * - Initial factory creation has overhead
     * - Subsequent tests reuse the factory but get fresh databases
     * - SQLite provides fast operations
     * 
     * Error Scenarios and Debugging:
     * Common Issues:
     * - Service Registration Conflicts: Ensure all production services are properly removed/replaced.
     * - Database File Locks: SQLite files may remain locked if not disposed properly.
     * - Configuration Precedence: In-memory config must override production settings.
     * Debugging Tips:
     * - Database Inspection: Use _context to query data during debugging.
     * - HTTP Traffic: Inspect _client requests/responses.
     * - Service Resolution: Verify DI container has expected services.
     * 
     * Integration with Test Methods:
     * Data Availability:
     * - Roles are seeded, so authentication logic can validate user roles.
     * - Tests can assume basic data exists without manual setup.
     * HTTP Testing:
     * - _client is ready to send requests to /auth/* endpoints.
     * - Full middleware pipeline is active (authentication, validation, etc.).
     * Database Verification:
     * - _context allows post-test database state inspection.
     * - Useful for verifying side effects of operations.
     * 
     * Conclusion:
     * The SetUp method creates a hermetically sealed testing environment that balances fidelity to production with the isolation required for reliable tests. Each line serves multiple purposes: isolation, performance, and maintainability. The setup ensures that authentication tests run against a real ASP.NET Core application with a real database, catching integration issues that unit tests miss while maintaining the speed and predictability required for effective test suites.
     * 
     * This approach represents best practices for ASP.NET Core integration testing, providing confidence that the authentication system works correctly in production-like conditions.
     */
    [SetUp]
    public void SetUp()
    {
        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureAppConfiguration((context, config) =>
                {
                    config.AddInMemoryCollection(new Dictionary<string, string>
                    {
                        ["Database:SeedJsonPath"] = null
                    });
                });
                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ThesisDbContext>));
                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }
                    services.AddDbContext<ThesisDbContext>(options =>
                    {
                        options.UseSqlite("Data Source=AuthControllerTests.db");
                    });
                });
            });

        _client = _factory.CreateClient();
        var scope = _factory.Services.CreateScope();
        _context = scope.ServiceProvider.GetRequiredService<ThesisDbContext>();
        _context.Database.EnsureCreated();

        // Seed data using Seeder
        var seeder = new TestDataSeeder(_context);
        seeder.SeedRoles();
    }

    [TearDown]
    public void TearDown()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
        _client.Dispose();
        _factory.Dispose();
    }
    
    
    
    [Test]
    public async Task Login_ValidCredentials_ReturnsOk()
    {
        // Arrange
        var loginRequest = new LoginRequest
        {
            Email = "abraham.student@test.com",
            Password = "password123"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/auth/login", loginRequest);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task Login_MissingPassword_ReturnsBadRequest()
    {
        // Arrange
        var loginRequest = new LoginRequest
        {
            Email = "abraham.student@test.com",
            Password = ""
        };

        // Act
        var response = await _client.PostAsJsonAsync("/auth/login", loginRequest);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task Login_InvalidCredentials_ReturnsUnauthorized()
    {
        // Arrange
        var loginRequest = new LoginRequest
        {
            Email = "invalid@example.com",
            Password = "wrongpassword"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/auth/login", loginRequest);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }
}
