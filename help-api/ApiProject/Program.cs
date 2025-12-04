using ApiProject.Logic;
using Microsoft.OpenApi;
using Microsoft.EntityFrameworkCore;
using ApiProject.Db.Context;
using ApiProject.Logic.Services;

var builder = WebApplication.CreateBuilder(args);

// API-Endpunkte für Swagger aktivieren
builder.Services.AddEndpointsApiExplorer();

// Swagger hinzufügen mit Info-Dokumentation
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Thesis Management API",
        Version = "v1",
        Description = "API für die Verwaltung von Thesen, Benutzern und Rollen."
    });
    // XML-Kommentare für Dokumentation einbinden (falls XML-Datei vorhanden)
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// MVC-Controller
builder.Services.AddControllers();

// Database
builder.Services.AddDbContext<ThesisDbContext>(options =>
    options.UseSqlite("Data Source=thesis.db"));

// Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IThesisService, ThesisService>();

var app = builder.Build();

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ThesisDbContext>();
    db.Database.EnsureCreated();
}

// Swagger und SwaggerUI aktivieren (nur in Entwicklung)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Thesis Management API v1");
    });
}

app.UseHttpsRedirection();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapControllers();


app.Run();