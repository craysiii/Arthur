namespace Arthur.Services;

public class SimpleAuthService
{
    private string? ApiKey { get; set; }
    
    public SimpleAuthService()
    {
        ApiKey = Environment.GetEnvironmentVariable("API_KEY");
    }

    public bool RequestAuthorized(HttpContext context)
    {
        var apiKeyProvided = context.Request.Headers.ContainsKey("API_KEY");
        var apiKeyPassed = apiKeyProvided ? context.Request.Headers["API_KEY"].ToString() : null;

        if (string.IsNullOrWhiteSpace(ApiKey)) return true;
        return apiKeyProvided && string.Equals(ApiKey, apiKeyPassed);
    }
}