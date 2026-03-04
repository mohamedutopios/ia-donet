using Module7.EmbeddingsRag.Services;

namespace Module7.EmbeddingsRag.Demos;

/// <summary>
/// Démo 1 — Visualiser les embeddings et la similarité cosinus.
/// </summary>
public class Demo1_Embeddings
{
    private readonly EmbeddingService _embedding;

    public Demo1_Embeddings(EmbeddingService embedding) => _embedding = embedding;

    public async Task RunAsync()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\n── Démo 1 : Embeddings & Similarité Cosinus ─────────────");
        Console.ResetColor();

        // ── Partie 1 : dimensions du vecteur ──────────────────────────────────
        Console.WriteLine("\n  [1/3] Dimensions du vecteur d'embedding");
        var vector = await _embedding.EmbedAsync("Bonjour le monde");
        Console.WriteLine($"  Texte  : \"Bonjour le monde\"");
        Console.WriteLine($"  Modèle : {_embedding.ModelName}");
        Console.WriteLine($"  Dims   : {vector.Length}");
        Console.WriteLine($"  Extrait: [{string.Join(", ", vector.Take(6).Select(v => $"{v:F4}"))}...]");

        // ── Partie 2 : phrases sémantiquement proches ou lointaines ──────────
        Console.WriteLine("\n  [2/3] Similarités sémantiques");

        var anchor = "J'ai faim, je veux manger";
        var anchVec = await _embedding.EmbedAsync(anchor);

        var comparisons = new[]
        {
            "Je voudrais déjeuner",
            "Mon estomac gargouille",
            "Je suis fatigué",
            "Il fait beau aujourd'hui",
            "La programmation en C# est intéressante",
        };

        Console.WriteLine($"\n  Ancre : \"{anchor}\"\n");
        foreach (var phrase in comparisons)
        {
            var vec  = await _embedding.EmbedAsync(phrase);
            var sim  = EmbeddingService.CosineSimilarity(anchVec, vec);
            var bar  = BuildBar(sim);
            var color = sim >= 0.8f ? ConsoleColor.Green
                      : sim >= 0.5f ? ConsoleColor.Yellow
                      : ConsoleColor.Red;
            Console.ForegroundColor = color;
            Console.WriteLine($"  {sim:F3} {bar}  \"{phrase}\"");
            Console.ResetColor();
            await Task.Delay(200);
        }

        // ── Partie 3 : seuils d'utilisation ──────────────────────────────────
        Console.WriteLine("\n  [3/3] Seuils recommandés");
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("  ≥ 0.85 → Déduplication / quasi-doublons");
        Console.WriteLine("  0.65–0.84 → Recherche sémantique (RAG)");
        Console.WriteLine("  0.40–0.64 → Recommandations");
        Console.WriteLine("  < 0.40  → Filtrer (non pertinent)");
        Console.ResetColor();
    }

    private static string BuildBar(float value)
    {
        int filled = (int)(value * 20);
        return "[" + new string('█', filled) + new string('░', 20 - filled) + "]";
    }
}
