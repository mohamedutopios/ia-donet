namespace Module3.AzureOpenAI;

/// <summary>
/// Options lues depuis appsettings.json → section "AzureOpenAI"
/// </summary>
public class AzureOpenAIOptions
{
    public string Endpoint           { get; set; } = string.Empty;
    public string ApiKey             { get; set; } = string.Empty;
    public string DeploymentChat     { get; set; } = "gpt-4o";
    public string DeploymentEmbedding{ get; set; } = "text-embedding-3-small";
}
