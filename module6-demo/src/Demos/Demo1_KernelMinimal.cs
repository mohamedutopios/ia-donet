using Microsoft.SemanticKernel;

namespace Module6.SemanticKernel.Demos;

/// <summary>
/// Démo 1 — Kernel minimal : InvokePromptAsync avec template {{$variable}}
/// </summary>
public class Demo1_KernelMinimal
{
    private readonly Kernel _kernel;

    public Demo1_KernelMinimal(Kernel kernel) => _kernel = kernel;

    public async Task RunAsync()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\n── Démo 1 : Kernel Minimal — InvokePromptAsync ───────────");
        Console.ResetColor();

        // ── Template avec variable ───────────────────────────────────────────
        var template = "Explique le concept de {{$topic}} en 3 points concis pour un développeur .NET débutant.";

        var topics = new[] { "injection de dépendances", "Semantic Kernel", "embeddings vectoriels" };

        foreach (var topic in topics)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\n  Topic : {topic}");
            Console.ResetColor();

            var result = await _kernel.InvokePromptAsync(
                template,
                new KernelArguments { ["topic"] = topic });

            Console.WriteLine($"  {result}");
            await Task.Delay(300);
        }
    }
}
