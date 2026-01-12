using System;

namespace WorkoutTrackerServices.Entities
{
    public class LlmChatHistory
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; } // Navigation property
        public string Prompt { get; set; } = string.Empty;
        public string Response { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
