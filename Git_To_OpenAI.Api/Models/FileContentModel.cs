namespace Git_To_OpenAI.Api.Models
{
    public class FileContentModel
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string Sha { get; set; }
        public string Content { get; set; }
        public string Encoding { get; set; }
    }
}
