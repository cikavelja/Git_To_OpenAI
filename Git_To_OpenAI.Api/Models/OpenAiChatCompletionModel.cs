namespace Git_To_OpenAI.Api
{
    public class OpenAiChatCompletionModel
    {
        public List<ChatCompletionChoice> Choices { get; set; }
    }

    public class ChatCompletionChoice
    {
        public ChatMessage Message { get; set; }
    }

    public class ChatMessage
    {
        public string Role { get; set; }
        public string Content { get; set; }
    }
}