using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Module6.SemanticKernel.Plugins;

namespace Module6.SemanticKernel.Demos;

/// <summary>
/// Démo 2 — Plugins natifs : [KernelFunction] + Auto function calling
/// </summary>
public class Demo2_Plugins
{
    private readonly Kernel _kernel;

    public Demo2_Plugins(Kernel kernel)
    {
        _kernel = kernel;
        _kernel.Plugins.AddFromType<HrPlugin>();
        _kernel.Plugins.AddFromType<EmailPlugin>();
    }

    public async Task RunAsync()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\n── Démo 2 : Plugins & Auto Function Calling ─────────────");
        Console.ResetColor();
        Console.WriteLine("  Le LLM choisit et appelle les plugins automatiquement.\n");

        var chat     = _kernel.GetRequiredService<IChatCompletionService>();
        var settings = new OpenAIPromptExecutionSettings
        {
            FunctionChoiceBehavior = Microsoft.SemanticKernel.FunctionChoiceBehavior.Auto()
        };

        var questions = new[]
        {
            "Trouve-moi des candidats qui connaissent Azure.",
            "Combien de jours de RTT a-t-on droit par an ?",
            "Rédige une convocation pour Alice Martin pour un entretien le 20 mars à 10h pour un poste de Dev Senior.",
        };

        foreach (var question in questions)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"\n  👤 {question}");
            Console.ResetColor();

            var history = new ChatHistory();
            history.AddSystemMessage("Tu es un assistant RH. Utilise les outils disponibles. Réponds en français.");
            history.AddUserMessage(question);

            var reply = await chat.GetChatMessageContentAsync(history, settings, _kernel);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"  🤖 {reply.Content}");
            Console.ResetColor();
            await Task.Delay(500);
        }
    }
}
