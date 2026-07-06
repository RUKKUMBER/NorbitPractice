using Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Database.EFCore
{
    internal class EfGameRepository
    {
        private readonly GameContext _context;

        public EfGameRepository(GameContext context)
        {
            _context = context;
        }

        // C — Create
        public void AddGame(Game game)
        {
            _context.Games.Add(game);
            _context.SaveChanges();
        }

        // R — Read
        public Game GetGameById(Guid gameId)
        {
            return _context.Games
                .Include(g => g.Publisher)
                .FirstOrDefault(g => g.GameId == gameId);
        }

        public List<Game> GetGamesCheaperThan(decimal maxPrice)
        {
            return _context.Games
                .Where(g => g.Price < maxPrice)
                .OrderBy(g => g.Price)
                .ToList();
        }

        // U — Update
        public void UpdateGamePrice(Guid gameId, decimal newPrice)
        {
            var game = _context.Games.Find(gameId);
            if (game != null)
            {
                game.Price = newPrice;
                _context.SaveChanges();
            }
        }

        // D — Delete
        public void DeleteGame(Guid gameId)
        {
            var game = _context.Games.Find(gameId);
            if (game != null)
            {
                _context.Games.Remove(game);
                _context.SaveChanges();
            }
        }
    }
}
