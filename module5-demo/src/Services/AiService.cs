using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Options;
using Module5.ApiIA.Models;

namespace Module5.ApiIA.Services;

// ── Interface ─────────────────────────────────────────────────────────────────

public interface IAiService
{
    Task<ChatResponse>      ChatAsync     (ChatRequest      request);
    Task<SummarizeResponse> SummarizeAsync(SummarizeRequest request);
}

// ── Implémentation ────────────────────────────────────────────────────────────

public class AiService : IAiService
{
    private readonly OpenAIClient       _client;
    private readonly AzureOpenAIOptions _opts;
    private readonly ILogger<AiService> _logger;

    public AiService(IOptions<AzureOpenAIOptions> opts, ILogger<AiService> logger)
    {
        _opts   = opts.Value;
        _logger = logger;
        _client = new OpenAIClient(
            new Uri(_opts.Endpoint),
            new AzureKeyCredential(_opts.ApiKey));
    }

    // ── Chat ──────────────────────────────────────────────────────────────────

    public async Task<ChatResponse> ChatAsync(ChatRequest request)
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();
        _logger.LogInformation("ChatAsync → {Chars} chars, temp={Temp}", request.Message.Length, request.Temperature);

        var options = new ChatCompletionsOptions
        {
            DeploymentName = _opts.DeploymentChat,
            Messages =
            {
                new ChatRequestSystemMessage(
                    "Tu es un assistant pédagogique expert en développement .NET. " +
                    "Réponds de façon claire et concise."),
                new ChatRequestUserMessage(request.Message)
            },
            Temperature = request.Temperature,
            MaxTokens   = request.MaxTokens
        };

        var response = await _client.GetChatCompletionsAsync(options);
        sw.Stop();

        var choice = response.Value.Choices[0];
        var usage  = response.Value.Usage;

        _logger.LogInformation("ChatAsync ← {Tokens} tokens, {Ms}ms", usage.TotalTokens, sw.ElapsedMilliseconds);

        return new ChatResponse
        {
            Reply        = choice.Message.Content,
            PromptTokens = usage.PromptTokens,
            ReplyTokens  = usage.CompletionTokens,
            TotalTokens  = usage.TotalTokens,
            ElapsedMs    = sw.ElapsedMilliseconds
        };
    }

    // ── Summarize ─────────────────────────────────────────────────────────────

    public async Task<SummarizeResponse> SummarizeAsync(SummarizeRequest request)
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();
        _logger.LogInformation("SummarizeAsync → {Chars} chars, format={Format}", request.Text.Length, request.Format);

        var formatInstruction = request.Format switch
        {
            SummaryFormat.Bullets   => $"en {request.MaxPoints} points bullet (préfixés par •)",
            SummaryFormat.Paragraph => $"en 1 paragraphe de {request.MaxPoints} phrases maximum",
            SummaryFormat.Json      => $$"""
                en JSON : { "points": ["string", ...] } avec {{request.MaxPoints}} points max.
                Réponds UNIQUEMENT avec le JSON, sans texte autour.
                """,
            _ => $"en {request.MaxPoints} points"
        };

        var systemPrompt = $"""
            Tu es un expert en synthèse de texte.
            Résume le texte fourni {formatInstruction}.
            Sois concis et conserve l'essentiel.
            """;

        var options = new ChatCompletionsOptions
        {
            DeploymentName = _opts.DeploymentChat,
            Messages =
            {
                new ChatRequestSystemMessage(systemPrompt),
                new ChatRequestUserMessage(request.Text)
            },
            Temperature = 0.3f,
            MaxTokens   = 600
        };

        var response = await _client.GetChatCompletionsAsync(options);
        sw.Stop();

        var summary = response.Value.Choices[0].Message.Content;
        var usage   = response.Value.Usage;

        _logger.LogInformation("SummarizeAsync ← {Tokens} tokens, {Ms}ms", usage.TotalTokens, sw.ElapsedMilliseconds);

        return new SummarizeResponse
        {
            Summary     = summary,
            WordCount   = summary.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length,
            TotalTokens = usage.TotalTokens,
            ElapsedMs   = sw.ElapsedMilliseconds
        };
    }
}
