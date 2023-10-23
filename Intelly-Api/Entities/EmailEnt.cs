namespace Intelly_Api.Entities
{
    public class EmailEnt
    {
        public string? EmailSender { get; set; }
        public string? Recipient { get; set; }
        public string? Body { get; set; }
        public string? ImageUrl { get; set; }
        public string? CompanyName { get; set; }
        public long CompanySenderId { get; set; }
        
        
    }
}
