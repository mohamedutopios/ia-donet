using Microsoft.Extensions.Configuration;
using Module7.EmbeddingsRag;
using Module7.EmbeddingsRag.Demos;
using Module7.EmbeddingsRag.Services;

// ── Configuration ──────────────────────────────────────────────────────────────
var config = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false)
    .Build();

var aiOpts = config.GetSection("AzureOpenAI").Get<AzureOpenAIOptions>()
    ?? throw new InvalidOperationException("Section AzureOpenAI manquante.");

var ragOpts = config.GetSection("Rag").Get<RagOptions>()
    ?? new RagOptions();

if (aiOpts.Endpoint.Contains("VOTRE") || aiOpts.ApiKey.Contains("VOTRE"))
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("❌ Configure appsettings.json avec ton Endpoint et ApiKey Azure OpenAI.");
    Console.ResetColor();
    return;
}

// ── Services ───────────────────────────────────────────────────────────────────
var embeddingService = new EmbeddingService(aiOpts);
var ragService       = new RagService(aiOpts, ragOpts);

// ── Menu ───────────────────────────────────────────────────────────────────────
Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("""
 ╔══════════════════════════════════════════════════╗
 ║   Module 7 — Embeddings & RAG                    ║
 ║   Formation IA .NET — Utopios                    ║
 ╚══════════════════════════════════════════════════╝
 """);
Console.ResetColor();

Console.WriteLine(" ⚡ Ordre recommandé : [1] → [2] → [3] → [4]\n");
Console.WriteLine("  [1] Embeddings & Similarité  — visualiser les vecteurs");
Console.WriteLine("  [2] Indexation               — indexer les docs du dossier /docs");
Console.WriteLine("  [3] Recherche sémantique     — chercher dans l'index");
Console.WriteLine("  [4] Chatbot RAG interactif   — Q&A sur vos documents");
Console.WriteLine("  [0] Démos 1 → 3 automatiques (sans interactif)\n");
Console.Write(" Votre choix : ");

var choice = Console.ReadLine()?.Trim();

try
{
    switch (choice)
    {
        case "1": await new Demo1_Embeddings(embeddingService).RunAsync();    break;
        case "2": await new Demo2_Indexing(ragService).RunAsync();            break;
        case "3": await new Demo3_Search(ragService).RunAsync();              break;
        case "4": await new Demo4_RagChatbot(ragService).RunAsync();          break;
        case "0":
        default:
            await new Demo1_Embeddings(embeddingService).RunAsync();
            await new Demo2_Indexing(ragService).RunAsync();
            await new Demo3_Search(ragService).RunAsync();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n  ℹ️  Démo 4 (chatbot interactif) disponible via [4].");
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
    if (ex.Message.Contains("index.json"))
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("   → Lance d'abord la démo [2] pour créer l'index.");
    }
    Console.ResetColor();
}
