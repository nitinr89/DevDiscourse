namespace Devdiscourse.Models
{
    public class OpenAIModel
    {
        public required string Question { get; set; }
        public string Answer { get; set; } = string.Empty;
    }
}