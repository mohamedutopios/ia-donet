using Module5.ApiIA;
using Module5.ApiIA.Middleware;
using Module5.ApiIA.Services;

var builder = WebApplication.CreateBuilder(args);

// ── Vérification configuration ────────────────────────────────────────────────
var endpoint = builder.Configuration["AzureOpenAI:Endpoint"] ?? "";
var apiKey   = builder.Configuration["AzureOpenAI:ApiKey"]   ?? "";

if (endpoint.Contains("VOTRE") || apiKey.Contains("VOTRE"))
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.Error.WriteLine("❌ Configure appsettings.json avec ton Endpoint et ApiKey Azure OpenAI.");
    Console.ResetColor();
    Environment.Exit(1);
}

// ── Services ──────────────────────────────────────────────────────────────────
builder.Services.Configure<AzureOpenAIOptions>(
    builder.Configuration.GetSection("AzureOpenAI"));

builder.Services.AddScoped<IAiService, AiService>();

builder.Services.AddControllers();

// Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title       = "Module 5 — API IA avec ASP.NET Core",
        Version     = "v1",
        Description = "Démo Formation IA .NET — Utopios"
    });
    // Inclure les commentaires XML
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath)) c.IncludeXmlComments(xmlPath);
});

// ── Application ───────────────────────────────────────────────────────────────
var app = builder.Build();

// Middleware d'erreurs en premier
app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Module 5 API v1");
    c.RoutePrefix = string.Empty;
});

app.MapControllers();

Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("""

 ╔══════════════════════════════════════════════════╗
 ║   Module 5 — API IA avec ASP.NET Core            ║
 ║   Formation IA .NET — Utopios                    ║
 ╚══════════════════════════════════════════════════╝

 Endpoints disponibles :
   POST  /api/ai/chat       → Conversation avec l'IA
   POST  /api/ai/summarize  → Résumé de texte
   GET   /api/ai/health     → Vérification santé

 Swagger UI : http://localhost:5000

""");
Console.ResetColor();

app.Run();
