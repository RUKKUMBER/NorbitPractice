namespace Database.Models
{
    public class Game
    {
        public Guid GameId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public bool HasMultiplayer { get; set; }
        public Guid? PublisherId { get; set; }
        public Publisher Publisher { get; set; }
        public List<Review> Reviews { get; set; }
    }
}
