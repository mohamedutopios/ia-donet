using Azure;
using Azure.AI.OpenAI;
using Module7.EmbeddingsRag.Models;
using System.Text.Json;

namespace Module7.EmbeddingsRag.Services;

/// <summary>
/// Service RAG complet : indexation, recherche sémantique, génération augmentée.
/// </summary>
public class RagService
{
    private readonly EmbeddingService   _embedding;
    private readonly ChunkService       _chunker;
    private readonly AzureOpenAIOptions _opts;
    private readonly RagOptions         _ragOpts;
    private readonly OpenAIClient       _client;
    private VectorIndex                 _index = new();

    public RagService(AzureOpenAIOptions opts, RagOptions ragOpts)
    {
        _opts      = opts;
        _ragOpts   = ragOpts;
        _embedding = new EmbeddingService(opts);
        _chunker   = new ChunkService(ragOpts);
        _client    = new OpenAIClient(
            new Uri(opts.Endpoint),
            new AzureKeyCredential(opts.ApiKey));
    }

    // ── INDEXATION ────────────────────────────────────────────────────────────

    public async Task IndexDirectoryAsync(string docsDir)
    {
        Console.WriteLine($"\n  📂 Indexation de : {docsDir}");

        var allChunks = _chunker.ChunkDirectory(docsDir).ToList();
        Console.WriteLine($"  {allChunks.Count} chunks extraits — génération des embeddings...");

        var texts   = allChunks.Select(c => c.ChunkText).ToList();
        var vectors = await _embedding.EmbedBatchAsync(texts,
            (done, total) => Console.Write($"\r  Embeddings : {done}/{total}"));

        Console.WriteLine();

        _index = new VectorIndex
        {
            Model     = _embedding.ModelName,
            IndexedAt = DateTime.UtcNow,
            ChunkSize = _ragOpts.ChunkSize,
            Chunks    = allChunks.Select((c, i) => new DocChunk
            {
                Id     = $"{c.FileName}_{i}",
                Source = c.FileName,
                Text   = c.ChunkText,
                Vector = vectors[i]
            }).ToList()
        };

        var json = JsonSerializer.Serialize(_index,
            new JsonSerializerOptions { WriteIndented = false });
        await File.WriteAllTextAsync(_ragOpts.IndexPath, json);

        Console.WriteLine($"  ✅ Index sauvegardé → {_ragOpts.IndexPath} ({_index.Chunks.Count} chunks)");
    }

    public async Task LoadIndexAsync()
    {
        if (!File.Exists(_ragOpts.IndexPath))
            throw new FileNotFoundException(
                $"Index introuvable : {_ragOpts.IndexPath}. Lance d'abord l'indexation.");

        var json = await File.ReadAllTextAsync(_ragOpts.IndexPath);
        _index   = JsonSerializer.Deserialize<VectorIndex>(json)
                   ?? throw new InvalidDataException("Index JSON invalide.");

        Console.WriteLine($"  📦 Index chargé : {_index.Chunks.Count} chunks ({_index.Model}) — indexé le {_index.IndexedAt:dd/MM HH:mm}");
    }

    // ── RECHERCHE ─────────────────────────────────────────────────────────────

    public async Task<List<SearchHit>> SearchAsync(string question)
    {
        var qVector = await _embedding.EmbedAsync(question);

        return _index.Chunks
            .Select(chunk => new SearchHit(
                chunk,
                EmbeddingService.CosineSimilarity(qVector, chunk.Vector)))
            .Where(h => h.Similarity >= _ragOpts.Threshold)
            .OrderByDescending(h => h.Similarity)
            .Take(_ragOpts.TopK)
            .ToList();
    }

    // ── GÉNÉRATION ────────────────────────────────────────────────────────────

    public async Task<RagAnswer> AskAsync(string question)
    {
        // 1. Rechercher les chunks pertinents
        var hits = await SearchAsync(question);

        if (!hits.Any())
            return new RagAnswer(
                "Je n'ai pas trouvé d'information pertinente dans les documents indexés.",
                [], []);

        // 2. Construire le contexte
        var context = string.Join("\n\n---\n\n",
            hits.Select((h, i) => $"[Source {i + 1}: {h.Chunk.Source}]\n{h.Chunk.Text}"));

        // 3. Prompt augmenté
        var systemPrompt =
            "Tu es un assistant documentaire expert.\n" +
            "Réponds UNIQUEMENT en te basant sur le contexte fourni.\n" +
            "Si l'information n'est pas dans le contexte, dis-le clairement.\n" +
            "Cite toujours les sources utilisées à la fin de ta réponse.\n" +
            "Réponds en français.";

        var userPrompt = $"Contexte :\n{context}\n\nQuestion : {question}";

        // 4. Générer la réponse (API beta.17)
        var options = new ChatCompletionsOptions
        {
            DeploymentName = _opts.DeploymentChat,
            Messages =
            {
                new ChatRequestSystemMessage(systemPrompt),
                new ChatRequestUserMessage(userPrompt)
            },
            Temperature = 0.2f,
            MaxTokens   = 600
        };

        var response = await _client.GetChatCompletionsAsync(options);
        var answer   = response.Value.Choices[0].Message.Content;
        var sources  = hits.Select(h => h.Chunk.Source).Distinct().ToList();
        var scores   = hits.Select(h => h.Similarity).ToArray();

        return new RagAnswer(answer, sources, scores);
    }
}
