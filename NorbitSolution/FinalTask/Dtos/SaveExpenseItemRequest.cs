using System.ComponentModel.DataAnnotations;

namespace FinalTask.Dtos
{
    public class SaveExpenseItemRequest
    {
        /// <summary>
        /// Название статьи (обязательно).
        /// </summary>
        [Required]
        [MaxLength(100)]
        public required string Name { get; set; }

        /// <summary>
        /// Id категории.
        /// </summary>
        [Required]
        public int CategoryId { get; set; }

        /// <summary>
        /// Активна ли статья.
        /// </summary>
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// Ответ API с данными статьи расхода.
    /// </summary>
    public class ExpenseItemResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public bool IsActive { get; set; }
    }
}
