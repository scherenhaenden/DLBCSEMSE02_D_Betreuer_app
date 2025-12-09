using ApiProject.BusinessLogic.Services;
using Microsoft.EntityFrameworkCore;
using ApiProject.DatabaseAccess.Context;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// Configure Kestrel to use URLs from appsettings.json
builder.WebHost.UseUrls(builder.Configuration["Urls"]);

// Enable API endpoints for Swagger
builder.Services.AddEndpointsApiExplorer();

// Add Swagger with info documentation
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Thesis Management API",
        Version = "v1",
        Description = "API for managing theses, users, roles, and topics."
    });
    // Include XML comments for documentation (if XML file exists)
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// MVC Controllers
builder.Services.AddControllers();

// Add JWT Authentication
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "default-secret-key"))
        };
    });

// Database
if (builder.Environment.IsDevelopment() && !builder.Configuration.GetValue<bool>("UseSqlite"))
{
    builder.Services.AddDbContext<ThesisDbContext>(options =>
        options.UseInMemoryDatabase("TestDb"));
}
else
{
    builder.Services.AddDbContext<ThesisDbContext>(options =>
        options.UseSqlite("Data Source=thesis.db"));
}

// Services
builder.Services.AddScoped<IUserBusinessLogicService, UserBusinessLogicService>();
builder.Services.AddScoped<IThesisService, ThesisService>();
builder.Services.AddScoped<ITopicService, TopicService>();

var app = builder.Build();

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ThesisDbContext>();
    db.Database.EnsureCreated();
}

// Enable Swagger and SwaggerUI (only in development)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Thesis Management API v1");
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapControllers();

app.Run();
