using Microsoft.Extensions.Configuration;
using Module4.PromptEngineering;

// ── Configuration ──────────────────────────────────────────────────────────────
var config = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false)
    .Build();

var opts = config.GetSection("AzureOpenAI").Get<AzureOpenAIOptions>()
    ?? throw new InvalidOperationException("Section AzureOpenAI manquante dans appsettings.json");

if (opts.Endpoint.Contains("VOTRE") || opts.ApiKey.Contains("VOTRE"))
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("❌ Configure appsettings.json avec ton Endpoint et ApiKey Azure OpenAI.");
    Console.ResetColor();
    return;
}

var ai = new AiClient(opts);

// ── Menu ───────────────────────────────────────────────────────────────────────
Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("""
 ╔══════════════════════════════════════════════════╗
 ║   Module 4 — Prompt Engineering                  ║
 ║   Formation IA .NET — Utopios                    ║
 ╚══════════════════════════════════════════════════╝
 """);
Console.ResetColor();

Console.WriteLine(" Choisissez une démo :\n");
Console.WriteLine("  [1] System Prompt     — impact de 3 personnalités différentes");
Console.WriteLine("  [2] Température       — déterminisme vs créativité");
Console.WriteLine("  [3] Tokens & Coût     — max_tokens et estimation financière");
Console.WriteLine("  [4] Few-Shot          — zero-shot vs 3-shot sur classification");
Console.WriteLine("  [5] JSON Structuré    — sortie typée avec retry automatique");
Console.WriteLine("  [6] Chain of Thought  — raisonnement étape par étape");
Console.WriteLine("  [0] Toutes les démos\n");
Console.Write(" Votre choix : ");

var choice = Console.ReadLine()?.Trim();

try
{
    switch (choice)
    {
        case "1": await new SystemPromptDemo(ai).RunAsync();     break;
        case "2": await new TemperatureDemo(ai).RunAsync();      break;
        case "3": await new TokensDemo(ai).RunAsync();           break;
        case "4": await new FewShotDemo(ai).RunAsync();          break;
        case "5": await new JsonStructuredDemo(ai).RunAsync();   break;
        case "6": await new ChainOfThoughtDemo(ai).RunAsync();   break;
        case "0":
        default:
            await new SystemPromptDemo(ai).RunAsync();
            await new TemperatureDemo(ai).RunAsync();
            await new TokensDemo(ai).RunAsync();
            await new FewShotDemo(ai).RunAsync();
            await new JsonStructuredDemo(ai).RunAsync();
            await new ChainOfThoughtDemo(ai).RunAsync();
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
