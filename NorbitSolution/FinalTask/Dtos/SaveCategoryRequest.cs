using System.ComponentModel.DataAnnotations;

namespace FinalTask.Dtos
{
    public class SaveCategoryRequest
    {
        /// <summary>
        /// Название категории (обязательно).
        /// </summary>
        [Required]
        [MaxLength(100)]
        public required string Name { get; set; }

        /// <summary>
        /// Месячный бюджет.
        /// </summary>
        [Range(0, double.MaxValue)]
        public decimal MonthlyBudget { get; set; }

        /// <summary>
        /// Активна ли категория.
        /// </summary>
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// Ответ API с данными категории.
    /// </summary>
    public class CategoryResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal MonthlyBudget { get; set; }
        public bool IsActive { get; set; }
    }
}
