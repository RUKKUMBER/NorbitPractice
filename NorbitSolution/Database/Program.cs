using Database.ADO;
using Database.EFCore;
using Database.Models;
using Microsoft.Extensions.Configuration;

namespace ComputerGames.App
{
    public class Program
    {
        public static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            string connectionString = configuration.GetConnectionString("DefaultConnection");

            var adoRepo = new AdoNetGameRepository(connectionString);
            var efContext = new GameContext(connectionString);
            var efRepo = new EfGameRepository(efContext);

            Console.WriteLine("=== Демонстрация ADO.NET ===");
            DemoCrud(adoRepo, connectionString);

            Console.WriteLine("\n=== Демонстрация Entity Framework Core ===");
            DemoCrudEf(efRepo);
        }

        private static void DemoCrud(AdoNetGameRepository repo, string connStr)
        {
            var newGame = new Game
            {
                Title = "Test ADO Game",
                Description = "Created by ADO.NET",
                Price = 19.99m,
                ReleaseDate = DateTime.Now,
                HasMultiplayer = false,
                PublisherId = GetFirstPublisherId(connStr) // вспомогательный метод
            };
            repo.AddGame(newGame);
            Console.WriteLine("Игра добавлена через ADO.NET.");

            var cheapGames = repo.GetGamesCheaperThan(30);
            Console.WriteLine($"Найдено {cheapGames.Count} игр дешевле 30:");
            cheapGames.ForEach(g => Console.WriteLine($"- {g.Title} ({g.Price:C})"));

            var firstGame = cheapGames.FirstOrDefault();
            if (firstGame != null)
            {
                repo.UpdateGamePrice(firstGame.GameId, 24.99m);
                Console.WriteLine($"Цена игры '{firstGame.Title}' обновлена.");
            }

            repo.DeleteGame(newGame.GameId);
            Console.WriteLine("Тестовая игра удалена.");
        }

        private static void DemoCrudEf(EfGameRepository repo)
        {
            var newGame = new Game
            {
                Title = "Test EF Game",
                Description = "Created by EF Core",
                Price = 25.00m,
                ReleaseDate = DateTime.Now,
                HasMultiplayer = true
            };
            repo.AddGame(newGame);
            Console.WriteLine("Игра добавлена через EF Core.");

            var cheapGames = repo.GetGamesCheaperThan(30);
            Console.WriteLine($"Найдено {cheapGames.Count} игр дешевле 30:");
            cheapGames.ForEach(g => Console.WriteLine($"- {g.Title} ({g.Price:C})"));

            var firstGame = cheapGames.FirstOrDefault();
            if (firstGame != null)
            {
                repo.UpdateGamePrice(firstGame.GameId, 29.99m);
                Console.WriteLine($"Цена игры '{firstGame.Title}' обновлена.");
            }

            repo.DeleteGame(newGame.GameId);
            Console.WriteLine("Тестовая игра удалена.");
        }

        private static Guid? GetFirstPublisherId(string connectionString)
        {
            string query = "SELECT TOP 1 PublisherId FROM Publishers";
            using (var connection = new Microsoft.Data.SqlClient.SqlConnection(connectionString))
            using (var command = new Microsoft.Data.SqlClient.SqlCommand(query, connection))
            {
                connection.Open();
                var result = command.ExecuteScalar();
                return result == DBNull.Value ? null : (Guid)result;
            }
        }
    }
}