using FinalTask.Models;
using Microsoft.EntityFrameworkCore;

namespace FinalTask.Data
{
    public class FinanceDbContext(DbContextOptions<FinanceDbContext> options) : DbContext(options)
    {
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<ExpenseItem> ExpenseItems => Set<ExpenseItem>();
        public DbSet<Transaction> Transactions => Set<Transaction>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(FinanceDbContext).Assembly);
        }
    }
}
