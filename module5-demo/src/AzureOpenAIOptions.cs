namespace Module5.ApiIA;

public class AzureOpenAIOptions
{
    public string Endpoint       { get; set; } = string.Empty;
    public string ApiKey         { get; set; } = string.Empty;
    public string DeploymentChat { get; set; } = "gpt-4o";
}
