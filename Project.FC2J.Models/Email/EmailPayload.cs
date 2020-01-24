namespace Project.FC2J.Models.Email
{
    public class EmailPayload
    {
        public string Body { get; set; } 
        public string Subject { get; set; } 
        public string To { get; set; } 
        public string Attachment { get; set; } 
    }
}
