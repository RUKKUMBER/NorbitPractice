namespace FinalTask.Models
{
    public class Transaction
    {
        public int Id { get; set; }

        /// <summary>
        /// Дата транзакции (без времени).
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Сумма расхода (положительное число).
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Комментарий.
        /// </summary>
        public string? Comment { get; set; }

        /// <summary>
        /// Идентификатор статьи расхода.
        /// </summary>
        public int ExpenseItemId { get; set; }

        // Навигационное свойство
        public ExpenseItem ExpenseItem { get; set; } = null!;
    }
}
