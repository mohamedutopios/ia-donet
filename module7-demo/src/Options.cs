namespace Module7.EmbeddingsRag;

public class AzureOpenAIOptions
{
    public string Endpoint            { get; set; } = string.Empty;
    public string ApiKey              { get; set; } = string.Empty;
    public string DeploymentChat      { get; set; } = "gpt-4o";
    public string DeploymentEmbedding { get; set; } = "text-embedding-3-small";
}

public class RagOptions
{
    public string IndexPath    { get; set; } = "index.json";
    public int    ChunkSize    { get; set; } = 500;
    public int    ChunkOverlap { get; set; } = 100;
    public int    TopK         { get; set; } = 3;
    public float  Threshold    { get; set; } = 0.65f;
}
