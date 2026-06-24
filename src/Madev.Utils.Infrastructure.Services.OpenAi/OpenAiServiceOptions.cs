namespace Madev.Utils.Infrastructure.Services.OpenAi;

public class OpenAiServiceOptions
{
    public string Endpoint { get; set; } = null!;
    public string DeploymentModel { get; set; } = null!;
    public string SystemChatMessage { get; set; } = null!;
    public float? Temperature { get; set; } = 0f;
    public float? TopP { get; set; } = 1f;
    public float? FrequencyPenalty { get; set; } = 0f;
    public float? PresencePenalty { get; set; } = 0f;
    public int? MaxOutputTokenCount { get; set; }
}
