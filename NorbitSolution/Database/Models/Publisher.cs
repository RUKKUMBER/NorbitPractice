namespace Database.Models
{
    public class Publisher
    {
        public Guid PublisherId { get; set; }
        public string PublisherName { get; set; }
        public string Country { get; set; }

        public List<Game> Games { get; set; }
    }
}