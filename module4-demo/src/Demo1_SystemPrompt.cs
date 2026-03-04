namespace Module4.PromptEngineering;

/// <summary>
/// Démo 1 — Impact du System Prompt
/// Compare 3 personnalités différentes sur la même question.
/// </summary>
public class SystemPromptDemo
{
    private readonly AiClient _ai;

    public SystemPromptDemo(AiClient ai) => _ai = ai;

    public async Task RunAsync()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\n── Démo 1 : Impact du System Prompt ─────────────");
        Console.ResetColor();
        Console.WriteLine("  Même question, 3 system prompts différents.\n");

        var question = "C'est quoi une API REST ?";

        var personas = new[]
        {
            (
                label: "Expert technique",
                system: "Tu es un architecte logiciel senior. Réponds de façon précise et technique."
            ),
            (
                label: "Formateur débutants",
                system: "Tu es un formateur qui explique à des débutants complets. Utilise des analogies simples, pas de jargon."
            ),
            (
                label: "Assistant JSON strict",
                system: """
                    Tu réponds UNIQUEMENT en JSON. Aucun texte en dehors du JSON.
                    Schema : { "terme": "string", "definition": "string", "exemple": "string" }
                    """
            )
        };

        foreach (var (label, system) in personas)
        {
            var (content, pt, ct) = await _ai.ChatAsync(system, question, temperature: 0.5f, maxTokens: 200);
            AiClient.Print($"System: \"{label}\"", content, pt, ct);
            await Task.Delay(500);
        }
    }
}
