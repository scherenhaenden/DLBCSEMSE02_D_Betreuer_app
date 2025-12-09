
using ApiProject.Extensions;
using ApiProject.Installation;

var builder = WebApplication.CreateBuilder(args);

// Configure Kestrel to use URLs from appsettings.json
builder.WebHost.UseUrls(builder.Configuration["Urls"]);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.ConfigureSwagger();
builder.Services.AddControllers();
builder.Services.Configure<AppSettings>(builder.Configuration);
builder.Services.ConfigureJwtAuthentication(builder.Configuration);
builder.Services.ConfigureDatabase(builder.Configuration);
builder.Services.ConfigureBusinessLogicServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.EnsureDatabaseCreated();

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
