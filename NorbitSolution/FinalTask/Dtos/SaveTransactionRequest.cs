using System.ComponentModel.DataAnnotations;

namespace FinalTask.Dtos
{
    public class SaveTransactionRequest
    {
        /// <summary>
        /// Дата транзакции (без времени).
        /// </summary>
        [Required]
        public DateTime Date { get; set; }

        /// <summary>
        /// Сумма (положительное число).
        /// </summary>
        [Required]
        [Range(0.01, 1000000)] // индивидуальная проверка дневного лимита будет в контроллере
        public decimal Amount { get; set; }

        /// <summary>
        /// Комментарий.
        /// </summary>
        [MaxLength(200)]
        public string? Comment { get; set; }

        /// <summary>
        /// Id статьи расхода.
        /// </summary>
        [Required]
        public int ExpenseItemId { get; set; }
    }

    /// <summary>
    /// Ответ API с данными транзакции.
    /// </summary>
    public class TransactionResponse
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string? Comment { get; set; }
        public int ExpenseItemId { get; set; }
        public string? ExpenseItemName { get; set; }
    }
}