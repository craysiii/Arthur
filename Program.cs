var exitCode = Microsoft.Playwright.Program.Main(["install", "--with-deps", "chromium"]);
if (exitCode != 0) Environment.Exit(exitCode);

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Arthur",
        Description = "Small API to generate PDFs from either a URL or base64-encoded HTML",
        Contact = new OpenApiContact
        {
            Name = "Charles Ray Shisler III",
            Url = new Uri("https://github.com/craysiii/Arthur"),
            Email = "charles@cray.io"
        },
        License = new OpenApiLicense
        {
            Name = "MIT License",
            Url = new Uri("https://github.com/craysiii/Arthur/blob/main/LICENSE.txt")
        }
    });
});

// Serialization of enums to strings instead of ints
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
builder.Services.Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

// Add our service to control the browser
builder.Services.AddSingleton<PlaywrightService>();
builder.Services.AddSingleton<SimpleAuthService>();

builder.WebHost.UseUrls("http://0.0.0.0:8080");

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/", () => "Welcome to Arthur.")
    .ExcludeFromDescription();

app.MapGet("/{documentId:guid}", (Guid documentId) =>
{
    var pdfDocumentPath = Path.Join(Path.GetTempPath(), $"{documentId}.html");
    return
        File.Exists(pdfDocumentPath) ?
            Results.Text(File.ReadAllText(pdfDocumentPath), "text/html", Encoding.UTF8, 200) :
            Results.NotFound();
}).ExcludeFromDescription();

app.MapPost("/from-base64",
    async (HttpContext context, HtmlRequest template, PlaywrightService playwright, SimpleAuthService auth) =>
{
    // Verify auth
    if (!auth.RequestAuthorized(context))
    {
        return Results.Unauthorized();
    }
    
    // Validate passed options
    var valid = MiniValidator.TryValidate(template, out var errors);
    if (!valid) return Results.ValidationProblem(errors);
    
    // Generate PDF
    var pdfPath = await playwright.GeneratePdfFromHtmlTemplate(context, template);
    
    // Return according to format
    switch (template.ResponseFormat)
    {
        case ResponseFormat.PDF:
            var fileBytes = File.ReadAllBytes(pdfPath);
            File.Delete(pdfPath);
            return Results.File(fileBytes, "application/pdf", template.FileName ?? Path.GetFileName(pdfPath));
        case ResponseFormat.BASE64:
            var fileBase64 = Base64.FromFileToBase64(pdfPath);
            File.Delete(pdfPath);
            return Results.Json(new { encodedFile = fileBase64 });
        default:
            return Results.BadRequest("You shouldn't be here.");
    }
}).WithName("fromBase64").WithOpenApi();

app.MapPost("/from-url",
    async (HttpContext context, UrlRequest template, PlaywrightService playwright, SimpleAuthService auth) =>
{
    // Verify auth
    if (!auth.RequestAuthorized(context))
    {
        return Results.Unauthorized();
    }
    
    // Validate passed options
    var valid = MiniValidator.TryValidate(template, out var errors);
    if (!valid) return Results.ValidationProblem(errors);
    
    // Generate PDF
    var pdfPath = await playwright.GeneratePdfFromUrl(template);
    
    // Return according to format
    switch (template.ResponseFormat)
    {
        case ResponseFormat.PDF:
            var fileBytes = File.ReadAllBytes(pdfPath);
            File.Delete(pdfPath);
            return Results.File(fileBytes, "application/pdf", template.FileName ?? Path.GetFileName(pdfPath));
        case ResponseFormat.BASE64:
            var fileBase64 = Base64.FromFileToBase64(pdfPath);
            File.Delete(pdfPath);
            return Results.Json(new { encodedFile = fileBase64 });
        default:
            return Results.BadRequest("You shouldn't be here.");
    }
}).WithName("fromUrl").WithOpenApi();

app.Run();
