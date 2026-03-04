using Azure.AI.OpenAI;

namespace Module4.PromptEngineering;

/// <summary>
/// Démo 4 — Few-shot Prompting
/// Montre l'impact d'exemples dans le prompt (0-shot vs 3-shot).
/// </summary>
public class FewShotDemo
{
    private readonly AiClient _ai;

    public FewShotDemo(AiClient ai) => _ai = ai;

    public async Task RunAsync()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\n── Démo 4 : Few-Shot Prompting ───────────────────");
        Console.ResetColor();
        Console.WriteLine("  Zero-shot vs Few-shot sur une tâche de classification.\n");

        var testPhrases = new[]
        {
            "Ce produit est incroyable, je l'adore !",
            "La livraison a pris 3 semaines, très décevant.",
            "Correct, sans plus."
        };

        // ── Zero-shot ─────────────────────────────────────────────────────────
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("  [ZERO-SHOT] Sans exemples :");
        Console.ResetColor();

        var zeroShotSystem = """
            Classifie le sentiment de la phrase.
            Réponds uniquement par: positif, négatif ou neutre.
            """;

        foreach (var phrase in testPhrases)
        {
            var (content, pt, ct) = await _ai.ChatAsync(zeroShotSystem, phrase, temperature: 0, maxTokens: 10);
            Console.WriteLine($"  \"{phrase}\" → {content.Trim()}");
            await Task.Delay(200);
        }

        // ── Few-shot ──────────────────────────────────────────────────────────
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("\n  [FEW-SHOT] Avec 3 exemples :");
        Console.ResetColor();

        var fewShotSystem = """
            Classifie le sentiment de la phrase.
            Réponds uniquement par: positif, négatif ou neutre.

            Exemples :
            Phrase: "Super qualité, je recommande !" → positif
            Phrase: "Arnaque totale, à fuir."        → négatif
            Phrase: "Le produit fait le travail."     → neutre
            """;

        foreach (var phrase in testPhrases)
        {
            var (content, pt, ct) = await _ai.ChatAsync(fewShotSystem, phrase, temperature: 0, maxTokens: 10);
            Console.WriteLine($"  \"{phrase}\" → {content.Trim()}");
            await Task.Delay(200);
        }

        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("\n  💡 Le few-shot améliore la cohérence du format et la précision.");
        Console.ResetColor();
    }
}
