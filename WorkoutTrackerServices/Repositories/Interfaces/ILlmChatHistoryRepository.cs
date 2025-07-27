using System.Collections.Generic;
using System.Threading.Tasks;
using WorkoutTrackerServices.Entities;

namespace WorkoutTrackerServices.Repositories.Interfaces
{
    public interface ILlmChatHistoryRepository
    {
        Task AddChatAsync(LlmChatHistory chat);
        Task<List<LlmChatHistory>> GetUserChatsAsync(int userId, int maxCount = 20);
    }
}
