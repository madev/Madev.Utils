using System.Threading;
using System.Threading.Tasks;

namespace Madev.Utils.Infrastructure.Services.OpenAi;

public interface IOpenAiService
{
    Task<string> GetCompletionAsync(string userMessage);

    Task<string> GetCompletionAsync(
        string userMessage,
        string jsonSchemaName,
        string jsonSchema,
        CancellationToken cancellationToken = default);

    Task<string> GetVisionCompletionAsync(
        string userMessage,
        byte[] imageContent,
        string imageMimeType,
        string? jsonSchemaName = null,
        string? jsonSchema = null,
        CancellationToken cancellationToken = default);
}
