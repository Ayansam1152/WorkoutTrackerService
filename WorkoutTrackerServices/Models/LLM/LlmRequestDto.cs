using System.Collections.Generic;

namespace WorkoutTrackerServices.Models.LLM
{
    public class LlmRequestDto
    {
        public string Prompt { get; set; } = string.Empty;
    }

    public class LlmResponseDto
    {
        public string Response { get; set; } = string.Empty;
    }
}
