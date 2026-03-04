using Azure;
using Azure.AI.OpenAI;

namespace Module3.AzureOpenAI;

/// <summary>
/// Démo 1 — Appel simple avec HttpClient (bas niveau)
/// Montre comment construire manuellement la requête REST.
/// </summary>
public class HttpClientDemo
{
    private readonly HttpClient _http;
    private readonly AzureOpenAIOptions _opts;

    public HttpClientDemo(HttpClient http, AzureOpenAIOptions opts)
    {
        _http = http;
        _opts = opts;
        _http.DefaultRequestHeaders.Add("api-key", opts.ApiKey);
    }

    public async Task RunAsync()
    {
        Console.WriteLine("\n── Démo 1 : HttpClient brut ──────────────────");

        var url = $"{_opts.Endpoint}openai/deployments/{_opts.DeploymentChat}" +
                  "/chat/completions?api-version=2024-02-01";

        var body = new
        {
            messages = new[]
            {
                new { role = "system",  content = "Tu es un assistant pédagogique concis." },
                new { role = "user",    content = "Explique ce qu'est un token en 2 phrases." }
            },
            max_tokens = 150,
            temperature = 0.7
        };

        var json    = System.Text.Json.JsonSerializer.Serialize(body);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

        var response = await _http.PostAsync(url, content);
        response.EnsureSuccessStatusCode();

        var raw    = await response.Content.ReadAsStringAsync();
        var parsed = System.Text.Json.JsonDocument.Parse(raw);
        var text   = parsed.RootElement
                           .GetProperty("choices")[0]
                           .GetProperty("message")
                           .GetProperty("content")
                           .GetString();

        Console.WriteLine($"Réponse : {text}");
    }
}

/// <summary>
/// Démo 2 — Appel avec le SDK Azure.AI.OpenAI (recommandé)
/// </summary>
public class SdkChatDemo
{
    private readonly OpenAIClient _client;
    private readonly AzureOpenAIOptions _opts;

    public SdkChatDemo(AzureOpenAIOptions opts)
    {
        _opts   = opts;
        _client = new OpenAIClient(
            new Uri(opts.Endpoint),
            new AzureKeyCredential(opts.ApiKey));
    }

    public async Task RunAsync()
    {
        Console.WriteLine("\n── Démo 2 : SDK Azure.AI.OpenAI ─────────────");

        var options = new ChatCompletionsOptions
        {
            DeploymentName = _opts.DeploymentChat,
            Messages =
            {
                new ChatRequestSystemMessage("Tu es un assistant pédagogique."),
                new ChatRequestUserMessage("Qu'est-ce que la température dans un LLM ? Réponds en 3 points.")
            },
            MaxTokens   = 300,
            Temperature = 0.5f
        };

        var response = await _client.GetChatCompletionsAsync(options);
        var message  = response.Value.Choices[0].Message.Content;

        Console.WriteLine($"Réponse :\n{message}");
        Console.WriteLine($"\nTokens utilisés — Prompt: {response.Value.Usage.PromptTokens} " +
                          $"| Completion: {response.Value.Usage.CompletionTokens} " +
                          $"| Total: {response.Value.Usage.TotalTokens}");
    }
}

/// <summary>
/// Démo 3 — Conversation multi-tour avec historique
/// </summary>
public class ConversationDemo
{
    private readonly OpenAIClient _client;
    private readonly AzureOpenAIOptions _opts;
    private readonly List<ChatRequestMessage> _history = new();

    public ConversationDemo(AzureOpenAIOptions opts)
    {
        _opts   = opts;
        _client = new OpenAIClient(
            new Uri(opts.Endpoint),
            new AzureKeyCredential(opts.ApiKey));

        // Message système persistant
        _history.Add(new ChatRequestSystemMessage(
            "Tu es un assistant de formation .NET expert. " +
            "Réponds de façon concise et pédagogique."));
    }

    public async Task<string> SendAsync(string userMessage)
    {
        _history.Add(new ChatRequestUserMessage(userMessage));

        var options = new ChatCompletionsOptions
        {
            DeploymentName = _opts.DeploymentChat,
            MaxTokens      = 400,
            Temperature    = 0.7f
        };

        foreach (var msg in _history)
            options.Messages.Add(msg);

        var response = await _client.GetChatCompletionsAsync(options);
        var reply    = response.Value.Choices[0].Message.Content;

        _history.Add(new ChatRequestAssistantMessage(reply));
        return reply;
    }

    public async Task RunAsync()
    {
        Console.WriteLine("\n── Démo 3 : Conversation multi-tour ─────────");

        var exchanges = new[]
        {
            "Qu'est-ce que le SDK Azure.AI.OpenAI ?",
            "Quelle est la différence avec HttpClient direct ?",
            "Donne-moi un exemple en C# de 5 lignes."
        };

        foreach (var msg in exchanges)
        {
            Console.WriteLine($"\n👤 {msg}");
            var reply = await SendAsync(msg);
            Console.WriteLine($"🤖 {reply}");
        }
    }
}

/// <summary>
/// Démo 4 — Réponse en streaming
/// </summary>
public class StreamingDemo
{
    private readonly OpenAIClient _client;
    private readonly AzureOpenAIOptions _opts;

    public StreamingDemo(AzureOpenAIOptions opts)
    {
        _opts   = opts;
        _client = new OpenAIClient(
            new Uri(opts.Endpoint),
            new AzureKeyCredential(opts.ApiKey));
    }

    public async Task RunAsync()
    {
        Console.WriteLine("\n── Démo 4 : Streaming ───────────────────────");
        Console.Write("🤖 Réponse en temps réel : ");

        var options = new ChatCompletionsOptions
        {
            DeploymentName = _opts.DeploymentChat,
            Messages =
            {
                new ChatRequestSystemMessage("Tu es un assistant concis."),
                new ChatRequestUserMessage(
                    "Liste 5 avantages du SDK Azure.AI.OpenAI vs HttpClient.")
            },
            MaxTokens   = 300,
            Temperature = 0.5f
        };

        var stream = await _client.GetChatCompletionsStreamingAsync(options);
        await foreach (var chunk in stream)
        {
            var text = chunk.ContentUpdate;
            if (!string.IsNullOrEmpty(text))
            {
                Console.Write(text);
                await Task.Delay(10); // effet visuel
            }
        }
        Console.WriteLine("\n");
    }
}

/// <summary>
/// Démo 5 — Réponse JSON structurée
/// </summary>
public class JsonOutputDemo
{
    private readonly OpenAIClient _client;
    private readonly AzureOpenAIOptions _opts;

    public JsonOutputDemo(AzureOpenAIOptions opts)
    {
        _opts   = opts;
        _client = new OpenAIClient(
            new Uri(opts.Endpoint),
            new AzureKeyCredential(opts.ApiKey));
    }

    public async Task RunAsync()
    {
        Console.WriteLine("\n── Démo 5 : Sortie JSON structurée ──────────");

        var options = new ChatCompletionsOptions
        {
            DeploymentName = _opts.DeploymentChat,
            Messages =
            {
                new ChatRequestSystemMessage("""
                    Tu es un assistant qui répond UNIQUEMENT en JSON valide.
                    Ne jamais ajouter de texte avant ou après le JSON.
                    Schema attendu :
                    {
                      "concept": "string",
                      "definition": "string (1 phrase)",
                      "exemple_csharp": "string (snippet court)",
                      "difficulte": "debutant|intermediaire|avance"
                    }
                    """),
                new ChatRequestUserMessage("Explique le concept de tokens dans un LLM.")
            },
            MaxTokens   = 400,
            Temperature = 0
        };

        var response = await _client.GetChatCompletionsAsync(options);
        var raw      = response.Value.Choices[0].Message.Content;

        Console.WriteLine("JSON brut reçu :");
        Console.WriteLine(raw);

        // Désérialiser
        try
        {
            var doc = System.Text.Json.JsonDocument.Parse(raw);
            Console.WriteLine($"\n✅ JSON valide !");
            Console.WriteLine($"   Concept    : {doc.RootElement.GetProperty("concept").GetString()}");
            Console.WriteLine($"   Difficulté : {doc.RootElement.GetProperty("difficulte").GetString()}");
        }
        catch
        {
            Console.WriteLine("⚠️ JSON invalide — prévoir un retry.");
        }
    }
}
