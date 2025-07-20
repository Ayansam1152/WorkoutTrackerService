using System.Text;
using Microsoft.Extensions.AI;
using OllamaSharp;
using OllamaSharp.Models;
using WorkoutTrackerServices.Models.LLM;

namespace WorkoutTrackerServices.Services
{
    public interface ILlmService
    {
        Task<LlmResponseDto> GetLlmResponseAsync(LlmRequestDto request);
    }

    public class LlmService : ILlmService
    {
        private readonly IChatClient _client;
        private readonly string _defaultModel;
        private readonly string _systemFilePath;

        public LlmService(IConfiguration config, IChatClient client)
        {
            _defaultModel = config["Ollama:DefaultModel"] ?? "llama3";
            _systemFilePath = config["Ollama:SystemFilePath"] ?? "knowledgeFileContents.md";
            _client = client;
        }

        public async Task<LlmResponseDto> GetLlmResponseAsync(LlmRequestDto request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Prompt))
            {
                throw new ArgumentException("Request and prompt cannot be null or empty.");
            }


            var chatHistory = new List<ChatMessage>();
            try
            {
                // Always load the system prompt from the output directory's LLM/knowledgeFileContents.md
                string systemPromptPath = Path.Combine(AppContext.BaseDirectory, "LLM", "knowledgeFileContents.md");
                if (File.Exists(systemPromptPath))
                {
                    string knowledgeFileContents = File.ReadAllText(systemPromptPath);
                    chatHistory.Add(new ChatMessage(ChatRole.System, knowledgeFileContents));
                }
                else
                {
                    // Optionally log: system prompt file not found
                }
            }
            catch (Exception)
            {
                // Optionally log: error reading system prompt file
            }

            chatHistory.Add(new ChatMessage(ChatRole.User, request.Prompt));

            var responseBuilder = new StringBuilder();
            // Optionally, you can load the system prompt from the new LLM/knowledgeFileContents.md here if needed
            // string systemFile = Path.Combine("LLM", "knowledgeFileContents.md");
            // if (File.Exists(systemFile))
            // {
            //     var systemPrompt = File.ReadAllText(systemFile);
            //     // You can prepend systemPrompt to the user prompt if your LLM supports it
            // }
            await foreach (var update in _client.GetStreamingResponseAsync(chatHistory))
            {
                responseBuilder.Append(update.Text);
            }
            return new LlmResponseDto { Response = responseBuilder.ToString() };
        }
    }
}
