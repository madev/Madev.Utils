namespace Madev.Utils.Infrastructure.Services.OpenAi;

public class OpenAiServiceOptions
{
    public string Endpoint { get; set; } = null!;
    public string DeploymentModel { get; set; } = null!;
    public string SystemChatMessage { get; set; } = null!;
    public float? Temperature { get; set; }
}
