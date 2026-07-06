using FinalTask.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinalTask.Data.Configurations
{
    public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Date).HasColumnType("date");
            builder.Property(t => t.Amount).HasColumnType("decimal(18,2)");
            builder.Property(t => t.Comment).HasMaxLength(200);

            builder.HasOne(t => t.ExpenseItem)
                .WithMany(e => e.Transactions)
                .HasForeignKey(t => t.ExpenseItemId)
                .OnDelete(DeleteBehavior.Restrict);

            // Индекс для быстрого поиска по дате
            builder.HasIndex(t => t.Date);
        }
    }
}
