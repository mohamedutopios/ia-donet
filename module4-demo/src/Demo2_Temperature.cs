namespace Module4.PromptEngineering;

/// <summary>
/// Démo 2 — Effet de la température
/// Appelle 3× avec température 0, 0.5 et 1.0 sur une tâche créative.
/// </summary>
public class TemperatureDemo
{
    private readonly AiClient _ai;

    public TemperatureDemo(AiClient ai) => _ai = ai;

    public async Task RunAsync()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\n── Démo 2 : Effet de la Température ─────────────");
        Console.ResetColor();
        Console.WriteLine("  Même prompt, températures 0 / 0.7 / 1.2\n");

        var system = "Tu es un copywriter créatif.";
        var user   = "Écris un slogan accrocheur pour une application de gestion de tâches. 1 seule phrase.";

        var temperatures = new[] { 0f, 0.7f, 1.2f };

        foreach (var temp in temperatures)
        {
            // Appeler 2× à temp=0 pour montrer la déterminisme
            var calls = temp == 0f ? 2 : 1;
            for (int i = 0; i < calls; i++)
            {
                var (content, pt, ct) = await _ai.ChatAsync(system, user, temperature: temp, maxTokens: 60);
                var label = temp == 0f
                    ? $"Température {temp:F1} — appel {i + 1}/2 (déterministe)"
                    : $"Température {temp:F1}";
                AiClient.Print(label, content, pt, ct);
                await Task.Delay(300);
            }
        }

        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("\n  💡 Temp=0 → même réponse à chaque appel (déterministe)");
        Console.WriteLine("     Temp=1.2 → plus créatif mais moins prévisible");
        Console.ResetColor();
    }
}
