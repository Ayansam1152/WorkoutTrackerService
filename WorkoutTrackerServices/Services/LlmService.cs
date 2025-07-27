using System.Text;
using Microsoft.Extensions.AI;
using OllamaSharp;
using OllamaSharp.Models;
using WorkoutTrackerServices.Models.LLM;
using WorkoutTrackerServices.Repositories.Interfaces;

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
        private readonly ILlmChatHistoryRepository _chatRepo;
        private readonly string _systemPrompt;

        public LlmService(IConfiguration config, IChatClient client, ILlmChatHistoryRepository chatRepo)
        {
            _defaultModel = config["Ollama:DefaultModel"] ?? "llama3";
            _systemFilePath = config["Ollama:SystemFilePath"] ?? "knowledgeFileContents.md";
            _client = client;
            _chatRepo = chatRepo;
            // Load system prompt once
            string systemPromptPath = Path.Combine(AppContext.BaseDirectory, "LLM", "knowledgeFileContents.md");
            if (File.Exists(systemPromptPath))
            {
                _systemPrompt = File.ReadAllText(systemPromptPath);
            }
            else
            {
                _systemPrompt = string.Empty;
            }
        }

        public async Task<LlmResponseDto> GetLlmResponseAsync(LlmRequestDto request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Prompt))
            {
                throw new ArgumentException("Request and prompt cannot be null or empty.");
            }

            // TODO: Pass userId in request or via context
            int userId = request.UserId; // Add UserId property to LlmRequestDto if not present

            var chatHistory = new List<ChatMessage>();
            // Add system prompt only for LLM context
            if (!string.IsNullOrWhiteSpace(_systemPrompt))
            {
                chatHistory.Add(new ChatMessage(ChatRole.System, _systemPrompt));
            }

            // Fetch previous chats for user and add to history
            var previousChats = await _chatRepo.GetUserChatsAsync(userId, 10);
            foreach (var chat in previousChats.OrderBy(c => c.Timestamp))
            {
                chatHistory.Add(new ChatMessage(ChatRole.User, chat.Prompt));
                chatHistory.Add(new ChatMessage(ChatRole.Assistant, chat.Response));
            }

            chatHistory.Add(new ChatMessage(ChatRole.User, request.Prompt));

            var responseBuilder = new StringBuilder();
            await foreach (var update in _client.GetStreamingResponseAsync(chatHistory))
            {
                responseBuilder.Append(update.Text);
            }
            string response = responseBuilder.ToString();

            // Store only user/assistant messages in DB
            await _chatRepo.AddChatAsync(new Entities.LlmChatHistory
            {
                UserId = userId,
                Prompt = request.Prompt,
                Response = response
            });

            return new LlmResponseDto { Response = response };
        }
    }
}
