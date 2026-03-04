using Azure;
using Azure.AI.OpenAI;

namespace Module4.PromptEngineering;

/// <summary>
/// Client partagé entre toutes les démos.
/// </summary>
public class AiClient
{
    private readonly OpenAIClient _client;
    private readonly AzureOpenAIOptions _opts;

    public AiClient(AzureOpenAIOptions opts)
    {
        _opts   = opts;
        _client = new OpenAIClient(
            new Uri(opts.Endpoint),
            new AzureKeyCredential(opts.ApiKey));
    }

    /// <summary>Appel simple avec system + user prompt.</summary>
    public async Task<(string Content, int PromptTokens, int CompletionTokens)> ChatAsync(
        string systemPrompt,
        string userPrompt,
        float  temperature = 0.7f,
        int    maxTokens   = 500)
    {
        var options = new ChatCompletionsOptions
        {
            DeploymentName = _opts.DeploymentChat,
            Messages =
            {
                new ChatRequestSystemMessage(systemPrompt),
                new ChatRequestUserMessage(userPrompt)
            },
            Temperature = temperature,
            MaxTokens   = maxTokens
        };

        var response = await _client.GetChatCompletionsAsync(options);
        var choice   = response.Value.Choices[0];
        return (
            choice.Message.Content,
            response.Value.Usage.PromptTokens,
            response.Value.Usage.CompletionTokens
        );
    }

    /// <summary>Affiche la réponse avec les méta-données tokens.</summary>
    public static void Print(string label, string content, int promptT, int completionT)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"\n  📝 {label}");
        Console.ResetColor();
        Console.WriteLine($"  {content}");
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine($"  [tokens → prompt:{promptT} | completion:{completionT} | total:{promptT + completionT}]");
        Console.ResetColor();
    }
}
