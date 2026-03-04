using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Module6.SemanticKernel;
using Module6.SemanticKernel.Demos;
using Module6.SemanticKernel.Services;


// ── Configuration ──────────────────────────────────────────────────────────────
var config = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false)
    .Build();

var opts = config.GetSection("AzureOpenAI").Get<AzureOpenAIOptions>()
    ?? throw new InvalidOperationException("Section AzureOpenAI manquante.");

if (opts.Endpoint.Contains("VOTRE") || opts.ApiKey.Contains("VOTRE"))
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("❌ Configure appsettings.json avec ton Endpoint et ApiKey Azure OpenAI.");
    Console.ResetColor();
    return;
}

// ── Kernel partagé pour les démos 1-3 ─────────────────────────────────────────
var kernel = Kernel.CreateBuilder()
    .AddAzureOpenAIChatCompletion(
        deploymentName: opts.DeploymentChat,
        endpoint:       opts.Endpoint,
        apiKey:         opts.ApiKey)
    .Build();

// ── Service pour la démo 4 (interactive) ──────────────────────────────────────
using var loggerFactory = Microsoft.Extensions.Logging.LoggerFactory.Create(b => b.AddConsole().SetMinimumLevel(LogLevel.Warning));
var assistant     = new HrAssistantService(opts, loggerFactory);

// ── Menu ───────────────────────────────────────────────────────────────────────
Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("""
 ╔══════════════════════════════════════════════════╗
 ║   Module 6 — Semantic Kernel                     ║
 ║   Formation IA .NET — Utopios                    ║
 ╚══════════════════════════════════════════════════╝
 """);
Console.ResetColor();

Console.WriteLine(" Choisissez une démo :\n");
Console.WriteLine("  [1] Kernel Minimal     — InvokePromptAsync avec templates");
Console.WriteLine("  [2] Plugins            — [KernelFunction] + Auto function calling");
Console.WriteLine("  [3] Multi-tour         — Enchaînement de plugins sur une conversation");
Console.WriteLine("  [4] Assistant Interactif — Boucle libre avec HrAssistantService");
Console.WriteLine("  [0] Démos 1 → 3 à la suite (sans interactif)\n");
Console.Write(" Votre choix : ");

var choice = Console.ReadLine()?.Trim();

try
{
    switch (choice)
    {
        case "1": await new Demo1_KernelMinimal(kernel).RunAsync();                  break;
        case "2": await new Demo2_Plugins(kernel).RunAsync();                        break;
        case "3": await new Demo3_MultiTurn(kernel).RunAsync();                      break;
        case "4": await new Demo4_InteractiveAssistant(assistant).RunAsync();        break;
        case "0":
        default:
            await new Demo1_KernelMinimal(kernel).RunAsync();
            await new Demo2_Plugins(kernel).RunAsync();
            await new Demo3_MultiTurn(kernel).RunAsync();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n  ℹ️  Démo 4 (interactif) skippée en mode automatique. Lance [4] séparément.");
            Console.ResetColor();
            break;
    }

    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("\n✅ Démo terminée.");
    Console.ResetColor();
}
catch (Exception ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"\n❌ Erreur : {ex.Message}");
    Console.ResetColor();
}
