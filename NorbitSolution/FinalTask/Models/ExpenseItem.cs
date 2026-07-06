namespace FinalTask.Models
{
    public class ExpenseItem
    {
        public int Id { get; set; }

        /// <summary>
        /// Название статьи расхода.
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// Идентификатор категории.
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// Признак активности статьи.
        /// </summary>
        public bool IsActive { get; set; }

        // Навигационные свойства
        public Category Category { get; set; } = null!;
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
