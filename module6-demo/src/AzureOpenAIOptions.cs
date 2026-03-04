namespace Module6.SemanticKernel;

public class AzureOpenAIOptions
{
    public string Endpoint            { get; set; } = string.Empty;
    public string ApiKey              { get; set; } = string.Empty;
    public string DeploymentChat      { get; set; } = "gpt-4o";
    public string DeploymentEmbedding { get; set; } = "text-embedding-3-small";
}
