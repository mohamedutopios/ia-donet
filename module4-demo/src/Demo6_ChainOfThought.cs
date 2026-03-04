namespace Module4.PromptEngineering;

/// <summary>
/// Démo 6 — Chain of Thought (CoT)
/// Montre comment forcer le raisonnement étape par étape améliore la précision.
/// </summary>
public class ChainOfThoughtDemo
{
    private readonly AiClient _ai;

    public ChainOfThoughtDemo(AiClient ai) => _ai = ai;

    public async Task RunAsync()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\n── Démo 6 : Chain of Thought ─────────────────────");
        Console.ResetColor();
        Console.WriteLine("  Sans CoT vs Avec CoT sur un problème de logique.\n");

        var question = """
            Une application reçoit 1200 requêtes/minute.
            Chaque requête prend en moyenne 80ms à traiter.
            On a 5 instances. Chaque instance peut traiter
            10 requêtes en parallèle.
            Le système peut-il absorber la charge ?
            """;

        // ── Sans Chain of Thought ─────────────────────────────────────────────
        var systemSansCot = "Tu es un expert en architecture logicielle. Réponds directement.";

        var (repSansCot, pt1, ct1) = await _ai.ChatAsync(
            systemSansCot, question, temperature: 0, maxTokens: 150);
        AiClient.Print("Sans Chain of Thought", repSansCot, pt1, ct1);

        await Task.Delay(500);

        // ── Avec Chain of Thought ─────────────────────────────────────────────
        var systemAvecCot = """
            Tu es un expert en architecture logicielle.
            Pour chaque problème, raisonne ÉTAPE PAR ÉTAPE avant de conclure :
            1. Identifie les données clés
            2. Effectue les calculs intermédiaires
            3. Compare avec la capacité disponible
            4. Conclus clairement
            """;

        var (repAvecCot, pt2, ct2) = await _ai.ChatAsync(
            systemAvecCot, question, temperature: 0, maxTokens: 300);
        AiClient.Print("Avec Chain of Thought", repAvecCot, pt2, ct2);

        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("\n  💡 CoT force le modèle à décomposer — moins d'erreurs de calcul.");
        Console.ResetColor();
    }
}
