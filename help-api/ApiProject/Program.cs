using ApiProject.Logic;
using Microsoft.OpenApi;

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

// In-Memory Services
builder.Services.AddSingleton<IUserService, UserService>();
builder.Services.AddSingleton<IThesisService, ThesisService>();

var app = builder.Build();

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

app.MapControllers();

// Neuer Endpunkt für die Index-Seite
app.MapGet("/", () => Results.Content(@"
<!DOCTYPE html>
<html lang='de'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Thesis Management API</title>
    <style>
        body { font-family: Arial, sans-serif; text-align: center; padding: 50px; }
        h1 { color: #333; }
        a { display: inline-block; margin-top: 20px; padding: 10px 20px; background-color: #007bff; color: white; text-decoration: none; border-radius: 5px; }
        a:hover { background-color: #0056b3; }
    </style>
</head>
<body>
    <h1>Willkommen zur Thesis Management API</h1>
    <p>Die API ist bereit und funktioniert. Verwenden Sie die folgenden Optionen:</p>
    <a href='/swagger'>Swagger-Dokumentation öffnen</a>
</body>
</html>
", "text/html"));

app.Run();