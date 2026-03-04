using Azure;
using Azure.AI.OpenAI;

namespace Module7.EmbeddingsRag.Services;

public class EmbeddingService
{
    private readonly OpenAIClient       _client;
    private readonly string             _deployment;
    public string ModelName { get; }

    public EmbeddingService(AzureOpenAIOptions opts)
    {
        ModelName   = opts.DeploymentEmbedding;
        _deployment = opts.DeploymentEmbedding;
        _client     = new OpenAIClient(
            new Uri(opts.Endpoint),
            new AzureKeyCredential(opts.ApiKey));
    }

    public async Task<float[]> EmbedAsync(string text)
    {
        var options = new EmbeddingsOptions(_deployment, new[] { text });
        var result  = await _client.GetEmbeddingsAsync(options);
        return result.Value.Data[0].Embedding.ToArray();
    }

    public async Task<float[][]> EmbedBatchAsync(
        IEnumerable<string> texts,
        Action<int, int>? onProgress = null)
    {
        var list    = texts.ToList();
        var results = new List<float[]>();
        int done    = 0;

        foreach (var batch in list.Chunk(16))
        {
            var options = new EmbeddingsOptions(_deployment, batch);
            var r       = await _client.GetEmbeddingsAsync(options);
            results.AddRange(r.Value.Data.Select(e => e.Embedding.ToArray()));
            done += batch.Length;
            onProgress?.Invoke(done, list.Count);
            await Task.Delay(200);
        }

        return [.. results];
    }

    public static float CosineSimilarity(ReadOnlySpan<float> a, ReadOnlySpan<float> b)
    {
        float dot = 0, normA = 0, normB = 0;
        for (int i = 0; i < a.Length; i++)
        {
            dot   += a[i] * b[i];
            normA += a[i] * a[i];
            normB += b[i] * b[i];
        }
        var denom = MathF.Sqrt(normA) * MathF.Sqrt(normB);
        return denom == 0 ? 0 : dot / denom;
    }
}