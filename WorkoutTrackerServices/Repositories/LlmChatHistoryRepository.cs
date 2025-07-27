using Microsoft.EntityFrameworkCore;
using WorkoutTrackerServices.Entities;
using WorkoutTrackerServices.Repositories.Interfaces;

namespace WorkoutTrackerServices.Repositories
{
    public class LlmChatHistoryRepository : ILlmChatHistoryRepository
    {
        private readonly WorkoutContext _context;
        public LlmChatHistoryRepository(WorkoutContext context)
        {
            _context = context;
        }

        public async Task AddChatAsync(LlmChatHistory chat)
        {
            _context.LlmChatHistories.Add(chat);
            await _context.SaveChangesAsync();
        }

        public async Task<List<LlmChatHistory>> GetUserChatsAsync(int userId, int maxCount = 20)
        {
            return await _context.LlmChatHistories
                .Where(c => c.UserId == userId)
                .OrderByDescending(c => c.Timestamp)
                .Take(maxCount)
                .ToListAsync();
        }
    }
}
