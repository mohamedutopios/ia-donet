using System.Text.Json.Serialization;

namespace Module7.EmbeddingsRag.Models;

/// <summary>Un chunk de document avec son embedding.</summary>
public class DocChunk
{
    [JsonPropertyName("id")]     public string   Id     { get; set; } = string.Empty;
    [JsonPropertyName("source")] public string   Source { get; set; } = string.Empty;
    [JsonPropertyName("text")]   public string   Text   { get; set; } = string.Empty;
    [JsonPropertyName("vector")] public float[]  Vector { get; set; } = [];
}

/// <summary>Index vectoriel sérialisé en JSON.</summary>
public class VectorIndex
{
    [JsonPropertyName("chunks")]     public List<DocChunk> Chunks     { get; set; } = [];
    [JsonPropertyName("model")]      public string         Model      { get; set; } = string.Empty;
    [JsonPropertyName("indexed_at")] public DateTime       IndexedAt  { get; set; }
    [JsonPropertyName("chunk_size")] public int            ChunkSize  { get; set; }
}

/// <summary>Résultat d'une recherche sémantique.</summary>
public record SearchHit(DocChunk Chunk, float Similarity);

/// <summary>Réponse RAG avec sources citées.</summary>
public record RagAnswer(string Answer, List<string> Sources, float[] Similarities);
