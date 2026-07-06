using FinalTask.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinalTask.Data.Configurations
{
    public class ExpenseItemConfiguration : IEntityTypeConfiguration<ExpenseItem>
    {
        public void Configure(EntityTypeBuilder<ExpenseItem> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Name).IsRequired().HasMaxLength(100);

            builder.HasOne(e => e.Category)
                .WithMany(c => c.ExpenseItems)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
