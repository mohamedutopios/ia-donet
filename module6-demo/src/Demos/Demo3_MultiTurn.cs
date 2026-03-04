using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Module6.SemanticKernel.Plugins;

namespace Module6.SemanticKernel.Demos;

/// <summary>
/// Démo 3 — Conversation multi-tour avec ChatHistory et plugins.
/// Montre que le LLM enchaîne plusieurs appels de plugins sur un seul message.
/// </summary>
public class Demo3_MultiTurn
{
    private readonly Kernel      _kernel;
    private readonly ChatHistory _history;

    public Demo3_MultiTurn(Kernel kernel)
    {
        _kernel = kernel;
        _kernel.Plugins.AddFromType<HrPlugin>();
        _kernel.Plugins.AddFromType<EmailPlugin>();
        _kernel.Plugins.AddFromType<CalendarPlugin>();

        _history = new ChatHistory();
        _history.AddSystemMessage(
            "Tu es un assistant RH expert. Utilise les outils disponibles. " +
            "Réponds en français de façon concise.");
    }

    public async Task RunAsync()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\n── Démo 3 : Multi-tour & Enchaînement de plugins ────────");
        Console.ResetColor();

        var chat     = _kernel.GetRequiredService<IChatCompletionService>();
        var settings = new OpenAIPromptExecutionSettings
        {
            FunctionChoiceBehavior = Microsoft.SemanticKernel.FunctionChoiceBehavior.Auto()
        };

        // Séquence de messages qui exploite la mémoire conversationnelle
        var messages = new[]
        {
            "Cherche des candidats en C#.",
            "Parmi eux, qui est à Paris ?",
            "Planifie un entretien avec Alice Martin le 25 mars à 14h, durée 45 min.",
            "Rédige-lui un mail de convocation pour un poste de Lead Dev.",
            "Montre-moi le calendrier actuel."
        };

        foreach (var msg in messages)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"\n  👤 {msg}");
            Console.ResetColor();

            _history.AddUserMessage(msg);
            var reply = await chat.GetChatMessageContentAsync(_history, settings, _kernel);
            _history.AddAssistantMessage(reply.Content!);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"  🤖 {reply.Content}");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"     [historique : {_history.Count} messages]");
            Console.ResetColor();

            await Task.Delay(400);
        }
    }
}
