using Module7.EmbeddingsRag.Services;

namespace Module7.EmbeddingsRag.Demos;

/// <summary>
/// Démo 3 — Recherche sémantique sur l'index : compare mot-clé vs sens.
/// </summary>
public class Demo3_Search
{
    private readonly RagService _rag;

    public Demo3_Search(RagService rag) => _rag = rag;

    public async Task RunAsync()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\n── Démo 3 : Recherche sémantique ────────────────────────");
        Console.ResetColor();

        await _rag.LoadIndexAsync();

        var queries = new[]
        {
            "Comment configurer l'authentification ?",
            "Quels sont les prérequis techniques ?",
            "Gestion des erreurs et exceptions",
            "Performance et optimisation",
        };

        foreach (var query in queries)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"\n  🔍 \"{query}\"");
            Console.ResetColor();

            var hits = await _rag.SearchAsync(query);

            if (!hits.Any())
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("  Aucun résultat au-dessus du seuil.");
                Console.ResetColor();
                continue;
            }

            foreach (var (hit, rank) in hits.Select((h, i) => (h, i + 1)))
            {
                var color = hit.Similarity >= 0.80f ? ConsoleColor.Green
                          : hit.Similarity >= 0.65f ? ConsoleColor.Yellow
                          : ConsoleColor.DarkGray;

                Console.ForegroundColor = color;
                Console.WriteLine($"  #{rank} [{hit.Similarity:F3}] {hit.Chunk.Source}");
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.DarkGray;
                var preview = hit.Chunk.Text.Length > 120
                    ? hit.Chunk.Text[..120] + "..."
                    : hit.Chunk.Text;
                Console.WriteLine($"      {preview}");
                Console.ResetColor();
            }

            await Task.Delay(300);
        }
    }
}
