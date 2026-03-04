namespace Module4.PromptEngineering;

/// <summary>
/// Démo 3 — Tokens et contrôle de longueur
/// Compare max_tokens 50 vs 300 et montre l'estimation du coût.
/// </summary>
public class TokensDemo
{
    private readonly AiClient _ai;

    // Tarif GPT-4o ($/1M tokens) — indicatif
    private const decimal InputRate  = 0.000005m;
    private const decimal OutputRate = 0.000015m;

    public TokensDemo(AiClient ai) => _ai = ai;

    public async Task RunAsync()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\n── Démo 3 : Tokens & Coût ────────────────────────");
        Console.ResetColor();

        var system = "Tu es un assistant technique.";
        var user   = "Explique les microservices.";

        var limites = new[] { 50, 150, 400 };

        foreach (var maxT in limites)
        {
            var (content, pt, ct) = await _ai.ChatAsync(system, user, temperature: 0.3f, maxTokens: maxT);

            var cout = (pt * InputRate) + (ct * OutputRate);

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\n  📝 max_tokens = {maxT}");
            Console.ResetColor();
            Console.WriteLine($"  {content}");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"  [tokens → prompt:{pt} | completion:{ct} | coût estimé: ${cout:F6}]");
            Console.ResetColor();

            if (ct == maxT)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("  ⚠️  Réponse tronquée — max_tokens atteint !");
                Console.ResetColor();
            }

            await Task.Delay(500);
        }

        // Estimation coût 1000 requêtes/jour
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("\n  💡 Projection : 1000 req/jour à max_tokens=150");
        Console.WriteLine($"     ≈ ${1000 * (100 * InputRate + 150 * OutputRate):F3} / jour");
        Console.ResetColor();
    }
}
