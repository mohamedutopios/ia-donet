using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Module6.SemanticKernel.Plugins;

namespace Module6.SemanticKernel.Services;

/// <summary>
/// Service central de l'assistant RH.
/// Orchestre le Kernel, les plugins et l'historique de conversation.
/// </summary>
public class HrAssistantService
{
    private readonly Kernel       _kernel;
    private readonly ChatHistory  _history;

    public HrAssistantService(AzureOpenAIOptions opts, ILoggerFactory loggerFactory)
    {
        // ── Construire le Kernel ─────────────────────────────────────────────
        _kernel = Kernel.CreateBuilder()
            .AddAzureOpenAIChatCompletion(
                deploymentName: opts.DeploymentChat,
                endpoint:       opts.Endpoint,
                apiKey:         opts.ApiKey)
            .Build();

        // ── Enregistrer les plugins ──────────────────────────────────────────
        _kernel.Plugins.AddFromType<HrPlugin>();
        _kernel.Plugins.AddFromType<EmailPlugin>();
        _kernel.Plugins.AddFromType<CalendarPlugin>();

        // ── Initialiser l'historique ─────────────────────────────────────────
        _history = new ChatHistory();
        _history.AddSystemMessage("""
            Tu es un assistant RH expert pour une entreprise tech.
            Tu as accès à des outils pour :
            - Rechercher des candidats par compétence
            - Consulter les politiques de congés
            - Générer des emails professionnels (convocation, refus, bienvenue)
            - Gérer le calendrier des entretiens

            Utilise ces outils lorsque c'est pertinent.
            Réponds toujours en français, de façon professionnelle et concise.
            Si tu utilises un outil, explique brièvement ce que tu as fait.
            """);
    }

    /// <summary>Envoie un message et retourne la réponse de l'assistant.</summary>
    public async Task<string> ChatAsync(string userMessage)
    {
        _history.AddUserMessage(userMessage);

        var chat     = _kernel.GetRequiredService<IChatCompletionService>();
        var settings = new OpenAIPromptExecutionSettings
        {
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
        };

        var reply = await chat.GetChatMessageContentAsync(
            _history, settings, _kernel);

        _history.AddAssistantMessage(reply.Content!);
        return reply.Content!;
    }

    /// <summary>Réinitialise l'historique tout en conservant le system prompt.</summary>
    public void Reset()
    {
        var system = _history[0]; // garder le message système
        _history.Clear();
        _history.Add(system);
    }

    /// <summary>Retourne le nombre de messages dans l'historique.</summary>
    public int HistoryCount => _history.Count - 1; // -1 pour le system message
}
