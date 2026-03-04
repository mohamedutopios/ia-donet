using Microsoft.Extensions.Configuration;
using Module3.AzureOpenAI;

// ── Configuration ─────────────────────────────────────────────────────────────
var config = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false)
    .Build();

var opts = config.GetSection("AzureOpenAI").Get<AzureOpenAIOptions>()
    ?? throw new InvalidOperationException("Section AzureOpenAI manquante dans appsettings.json");

// Vérification basique de la configuration
if (opts.Endpoint.Contains("VOTRE") || opts.ApiKey.Contains("VOTRE"))
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("❌ Configure d'abord appsettings.json avec ton Endpoint et ApiKey Azure OpenAI.");
    Console.ResetColor();
    return;
}

// ── Menu ──────────────────────────────────────────────────────────────────────
Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("""
 ╔══════════════════════════════════════════════════╗
 ║   Module 3 — Appeler l'IA depuis C#              ║
 ║   Formation IA .NET — Utopios                    ║
 ╚══════════════════════════════════════════════════╝
 """);
Console.ResetColor();

Console.WriteLine(" Choisissez une démo :\n");
Console.WriteLine("  [1] HttpClient brut — requête REST manuelle");
Console.WriteLine("  [2] SDK Azure.AI.OpenAI — appel recommandé");
Console.WriteLine("  [3] Conversation multi-tour avec historique");
Console.WriteLine("  [4] Streaming — réponse en temps réel");
Console.WriteLine("  [5] Sortie JSON structurée");
Console.WriteLine("  [0] Toutes les démos à la suite\n");
Console.Write(" Votre choix : ");

var choice = Console.ReadLine()?.Trim();

try
{
    switch (choice)
    {
        case "1":
            await new HttpClientDemo(new HttpClient(), opts).RunAsync();
            break;
        case "2":
            await new SdkChatDemo(opts).RunAsync();
            break;
        case "3":
            await new ConversationDemo(opts).RunAsync();
            break;
        case "4":
            await new StreamingDemo(opts).RunAsync();
            break;
        case "5":
            await new JsonOutputDemo(opts).RunAsync();
            break;
        case "0":
        default:
            await new HttpClientDemo(new HttpClient(), opts).RunAsync();
            await new SdkChatDemo(opts).RunAsync();
            await new ConversationDemo(opts).RunAsync();
            await new StreamingDemo(opts).RunAsync();
            await new JsonOutputDemo(opts).RunAsync();
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
