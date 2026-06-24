using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Azure.AI.OpenAI;
using Azure.Core;
using OpenAI.Chat;

namespace Madev.Utils.Infrastructure.Services.OpenAi;

public class OpenAiService : IOpenAiService
{
    private readonly AzureOpenAIClient _client;
    private readonly string _deploymentModel;
    private readonly string _systemChatMessage;
    private readonly ChatCompletionOptions _chatOptions;

    public OpenAiService(OpenAiServiceOptions options, TokenCredential credential)
    {
        _client = new AzureOpenAIClient(new Uri(options.Endpoint), credential);
        _deploymentModel = options.DeploymentModel;
        _systemChatMessage = options.SystemChatMessage;
        _chatOptions = BuildChatOptions(options);
    }

    public Task<string> GetCompletionAsync(string userMessage)
    {
        var messages = new List<ChatMessage>
        {
            new SystemChatMessage(_systemChatMessage),
            new UserChatMessage(userMessage),
        };
        return SendChatAsync(messages, CancellationToken.None);
    }

    public Task<string> GetCompletionAsync(
        string userMessage,
        string jsonSchemaName,
        string jsonSchema,
        CancellationToken cancellationToken = default)
    {
        var effectiveUserMessage = BuildEffectiveUserMessage(userMessage, jsonSchemaName, jsonSchema);
        var messages = new List<ChatMessage>
        {
            new SystemChatMessage(_systemChatMessage),
            new UserChatMessage(ChatMessageContentPart.CreateTextPart(effectiveUserMessage)),
        };
        return SendChatAsync(messages, cancellationToken);
    }

    public Task<string> GetVisionCompletionAsync(
        string userMessage,
        byte[] imageContent,
        string imageMimeType,
        string? jsonSchemaName = null,
        string? jsonSchema = null,
        CancellationToken cancellationToken = default)
    {
        var effectiveUserMessage = BuildEffectiveUserMessage(userMessage, jsonSchemaName, jsonSchema);
        var messages = new List<ChatMessage>
        {
            new SystemChatMessage(_systemChatMessage),
            new UserChatMessage(
                ChatMessageContentPart.CreateTextPart(effectiveUserMessage),
                ChatMessageContentPart.CreateImagePart(BinaryData.FromBytes(imageContent), imageMimeType)),
        };
        return SendChatAsync(messages, cancellationToken);
    }

    private static ChatCompletionOptions BuildChatOptions(OpenAiServiceOptions options)
    {
        var chatOptions = new ChatCompletionOptions();
        if (options.Temperature.HasValue) chatOptions.Temperature = options.Temperature.Value;
        if (options.TopP.HasValue) chatOptions.TopP = options.TopP.Value;
        if (options.FrequencyPenalty.HasValue) chatOptions.FrequencyPenalty = options.FrequencyPenalty.Value;
        if (options.PresencePenalty.HasValue) chatOptions.PresencePenalty = options.PresencePenalty.Value;
        if (options.MaxOutputTokenCount.HasValue) chatOptions.MaxOutputTokenCount = options.MaxOutputTokenCount.Value;
        return chatOptions;
    }

    private static string BuildEffectiveUserMessage(string userMessage, string? jsonSchemaName, string? jsonSchema)
    {
        if (string.IsNullOrWhiteSpace(jsonSchema) || string.IsNullOrWhiteSpace(jsonSchemaName))
            return userMessage;

        return $"{userMessage}\n\n" +
               $"Return a single JSON object matching this JSON schema exactly. Schema name: {jsonSchemaName}.\n" +
               $"JSON schema:\n{jsonSchema}";
    }

    private async Task<string> SendChatAsync(IReadOnlyList<ChatMessage> messages, CancellationToken cancellationToken)
    {
        var chatClient = _client.GetChatClient(_deploymentModel);
        var response = await chatClient.CompleteChatAsync(messages, _chatOptions, cancellationToken);
        return StripMarkdownFences(response.Value.Content[0].Text);
    }

    private static string StripMarkdownFences(string text)
    {
        var trimmed = text.Trim();
        if (!trimmed.StartsWith("```")) return trimmed;
        var firstNewline = trimmed.IndexOf('\n');
        if (firstNewline < 0) return trimmed;
        var withoutOpening = trimmed[(firstNewline + 1)..];
        var closingFence = withoutOpening.LastIndexOf("```", StringComparison.Ordinal);
        return closingFence < 0 ? withoutOpening.Trim() : withoutOpening[..closingFence].Trim();
    }
}
