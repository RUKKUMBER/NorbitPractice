namespace FinalTask.Models
{
    public class Category
    {
        public int Id { get; set; }

        /// <summary>
        /// Название категории.
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// Месячный бюджет в рублях.
        /// </summary>
        public decimal MonthlyBudget { get; set; }

        /// <summary>
        /// Признак активности категории.
        /// </summary>
        public bool IsActive { get; set; }

        // Навигационное свойство
        public ICollection<ExpenseItem> ExpenseItems { get; set; } = new List<ExpenseItem>();
    }
}
