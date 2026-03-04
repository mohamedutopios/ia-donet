using Module7.EmbeddingsRag.Services;

namespace Module7.EmbeddingsRag.Demos;

/// <summary>
/// Démo 2 — Indexer les documents du dossier /docs.
/// </summary>
public class Demo2_Indexing
{
    private readonly RagService _rag;

    public Demo2_Indexing(RagService rag) => _rag = rag;

    public async Task RunAsync()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\n── Démo 2 : Indexation de documents ─────────────────────");
        Console.ResetColor();

        var docsDir = Path.Combine(AppContext.BaseDirectory, "docs");

        if (!Directory.Exists(docsDir) || !Directory.GetFiles(docsDir, "*.txt").Any())
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\n  ⚠️  Dossier '{docsDir}' vide ou introuvable.");
            Console.WriteLine("  Ajoutez des fichiers .txt dans src/docs/ et relancez.");
            Console.ResetColor();
            return;
        }

        var sw = System.Diagnostics.Stopwatch.StartNew();
        await _rag.IndexDirectoryAsync(docsDir);
        sw.Stop();

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"\n  ✅ Indexation terminée en {sw.ElapsedMilliseconds}ms");
        Console.WriteLine("  Relancez la Démo 3 ou 4 pour interroger l'index.");
        Console.ResetColor();
    }
}
