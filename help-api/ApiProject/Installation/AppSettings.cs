namespace ApiProject.Installation;

public class AppSettings
{
    public string Urls { get; set; }
    public Logging Logging { get; set; }
    public string AllowedHosts { get; set; }
    public Jwt Jwt { get; set; }
    public Database Database { get; set; }
}

public class Logging
{
    public LogLevel LogLevel { get; set; }
}

public class LogLevel
{
    public string Default { get; set; }
    public string MicrosoftAspNetCore { get; set; }
}

public class Jwt
{
    public string Key { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
}

public class Database
{
    public string Type { get; set; }
    public string ConnectionString { get; set; }
    public string SeedJsonPath { get; set; }
}
