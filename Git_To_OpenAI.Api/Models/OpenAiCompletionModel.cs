namespace Git_To_OpenAI.Api.Models
{
    public class OpenAiCompletionModel
    {
        public List<Choice> Choices { get; set; }
    }

    public class Choice
    {
        public string Text { get; set; }
    }
}
