using Database.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Database.ADO
{
    internal class AdoNetGameRepository
    {
        private readonly string _connectionString;

        public AdoNetGameRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        // C — Create
        public void AddGame(Game game)
        {
            string query = @"
                INSERT INTO Games (Title, Description, Price, ReleaseDate, HasMultiplayer, PublisherId)
                VALUES (@Title, @Description, @Price, @ReleaseDate, @HasMultiplayer, @PublisherId)";

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Title", game.Title);
                command.Parameters.AddWithValue("@Description", (object)game.Description ?? DBNull.Value);
                command.Parameters.AddWithValue("@Price", game.Price);
                command.Parameters.AddWithValue("@ReleaseDate", (object)game.ReleaseDate ?? DBNull.Value);
                command.Parameters.AddWithValue("@HasMultiplayer", game.HasMultiplayer);
                command.Parameters.AddWithValue("@PublisherId", (object)game.PublisherId ?? DBNull.Value);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        // R — Read
        public Game GetGameById(Guid gameId)
        {
            string query = "SELECT * FROM Games WHERE GameId = @GameId";

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@GameId", gameId);
                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return MapGameFromReader(reader);
                    }
                }
            }
            return null;
        }

        public List<Game> GetGamesCheaperThan(decimal maxPrice)
        {
            var games = new List<Game>();
            string query = "SELECT * FROM Games WHERE Price < @MaxPrice ORDER BY Price";

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@MaxPrice", maxPrice);
                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        games.Add(MapGameFromReader(reader));
                    }
                }
            }
            return games;
        }

        // U — Update
        public void UpdateGamePrice(Guid gameId, decimal newPrice)
        {
            string query = "UPDATE Games SET Price = @Price WHERE GameId = @GameId";

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Price", newPrice);
                command.Parameters.AddWithValue("@GameId", gameId);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        // D — Delete
        public void DeleteGame(Guid gameId)
        {
            string query = "DELETE FROM Games WHERE GameId = @GameId";

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@GameId", gameId);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        private Game MapGameFromReader(SqlDataReader reader)
        {
            return new Game
            {
                GameId = reader.GetGuid(reader.GetOrdinal("GameId")),
                Title = reader.GetString(reader.GetOrdinal("Title")),
                Description = reader.IsDBNull("Description") ? null : reader.GetString("Description"),
                Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                ReleaseDate = reader.IsDBNull("ReleaseDate") ? (DateTime?)null : reader.GetDateTime("ReleaseDate"),
                HasMultiplayer = reader.GetBoolean(reader.GetOrdinal("HasMultiplayer")),
                PublisherId = reader.IsDBNull("PublisherId") ? (Guid?)null : reader.GetGuid("PublisherId")
            };
        }
    }
}
