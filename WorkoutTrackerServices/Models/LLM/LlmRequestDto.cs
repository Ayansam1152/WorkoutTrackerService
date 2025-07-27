using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WorkoutTrackerServices.Models.LLM
{
    public class LlmRequestDto
    {
        public string Prompt { get; set; } = string.Empty;
        [JsonIgnore]
        public int UserId { get; set; }
    }

    public class LlmResponseDto
    {
        public string Response { get; set; } = string.Empty;
    }
}
