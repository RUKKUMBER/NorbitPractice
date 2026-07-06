namespace Database.Models
{
    public class Genre
    {
        public int GenreId { get; set; }
        public string GenreName { get; set; }

        public List<GameGenre> GameGenres { get; set; }
    }
}