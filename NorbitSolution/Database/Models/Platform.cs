namespace Database.Models
{
    public class Platform
    {
        public int PlatformId { get; set; }
        public string PlatformName { get; set; }
        public string Manufacturer { get; set; }
        public DateTime? ReleaseDate { get; set; }

        public List<GamePlatform> GamePlatforms { get; set; }
    }
}