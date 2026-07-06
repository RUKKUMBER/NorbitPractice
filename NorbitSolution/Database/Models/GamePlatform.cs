namespace Database.Models
{
    public class GamePlatform
    {
        public Guid GameId { get; set; }
        public int PlatformId { get; set; }

        public Game Game { get; set; }
        public Platform Platform { get; set; }
    }
}