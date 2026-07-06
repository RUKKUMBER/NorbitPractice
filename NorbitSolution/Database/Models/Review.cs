namespace Database.Models
{
    public class Review
    {
        public int ReviewId { get; set; }
        public Guid GameId { get; set; }
        public int UserId { get; set; }
        public string ReviewText { get; set; }
        public decimal Rating { get; set; }
        public DateTime ReviewDate { get; set; }

        public Game Game { get; set; }
        public User User { get; set; }
    }
}