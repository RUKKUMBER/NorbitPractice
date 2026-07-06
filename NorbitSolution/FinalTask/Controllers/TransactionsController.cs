using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FinalTask.Data;
using FinalTask.Dtos;
using FinalTask.Models;

namespace FinalTask.Controllers;

/// <summary>
/// Управление транзакциями (фактическими расходами).
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TransactionsController : ControllerBase
{
    private readonly FinanceDbContext _context;

    public TransactionsController(FinanceDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Получить все транзакции за всё время.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TransactionResponse>>> GetAll()
    {
        var transactions = await _context.Transactions
            .Include(t => t.ExpenseItem)
            .OrderByDescending(t => t.Date)
            .Select(t => new TransactionResponse
            {
                Id = t.Id,
                Date = t.Date,
                Amount = t.Amount,
                Comment = t.Comment,
                ExpenseItemId = t.ExpenseItemId,
                ExpenseItemName = t.ExpenseItem.Name
            })
            .ToListAsync();

        return Ok(transactions);
    }

    /// <summary>
    /// Получить транзакции за конкретный день.
    /// </summary>
    /// <param name="date">Дата в формате ГГГГ-ММ-ДД</param>
    [HttpGet("byDate")]
    public async Task<ActionResult<IEnumerable<TransactionResponse>>> GetByDate([FromQuery] DateTime date)
    {
        var dayStart = date.Date;
        var dayEnd = dayStart.AddDays(1);

        var transactions = await _context.Transactions
            .Include(t => t.ExpenseItem)
            .Where(t => t.Date >= dayStart && t.Date < dayEnd)
            .OrderBy(t => t.Date)
            .Select(t => new TransactionResponse
            {
                Id = t.Id,
                Date = t.Date,
                Amount = t.Amount,
                Comment = t.Comment,
                ExpenseItemId = t.ExpenseItemId,
                ExpenseItemName = t.ExpenseItem.Name
            })
            .ToListAsync();

        return Ok(transactions);
    }

    /// <summary>
    /// Получить транзакции за месяц.
    /// </summary>
    /// <param name="year">Год</param>
    /// <param name="month">Месяц (1-12)</param>
    [HttpGet("byMonth")]
    public async Task<ActionResult<IEnumerable<TransactionResponse>>> GetByMonth([FromQuery] int year, [FromQuery] int month)
    {
        if (month < 1 || month > 12)
        {
            return BadRequest("Месяц должен быть от 1 до 12.");
        }

        var fromDate = new DateTime(year, month, 1);
        var toDate = fromDate.AddMonths(1);

        var transactions = await _context.Transactions
            .Include(t => t.ExpenseItem)
            .Where(t => t.Date >= fromDate && t.Date < toDate)
            .OrderBy(t => t.Date)
            .Select(t => new TransactionResponse
            {
                Id = t.Id,
                Date = t.Date,
                Amount = t.Amount,
                Comment = t.Comment,
                ExpenseItemId = t.ExpenseItemId,
                ExpenseItemName = t.ExpenseItem.Name
            })
            .ToListAsync();

        return Ok(transactions);
    }

    /// <summary>
    /// Получить транзакцию по Id.
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<TransactionResponse>> GetById(int id)
    {
        var transaction = await _context.Transactions
            .Include(t => t.ExpenseItem)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (transaction is null)
        {
            return NotFound();
        }

        return Ok(new TransactionResponse
        {
            Id = transaction.Id,
            Date = transaction.Date,
            Amount = transaction.Amount,
            Comment = transaction.Comment,
            ExpenseItemId = transaction.ExpenseItemId,
            ExpenseItemName = transaction.ExpenseItem.Name
        });
    }

    /// <summary>
    /// Создать новую транзакцию.
    /// Проверяет дневной лимит 1 млн руб. и активность статьи.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<TransactionResponse>> Create(SaveTransactionRequest request)
    {
        // Проверка существования и активности статьи
        var expenseItem = await _context.ExpenseItems
            .Include(e => e.Category)
            .FirstOrDefaultAsync(e => e.Id == request.ExpenseItemId);

        if (expenseItem is null)
        {
            return BadRequest("Статья расхода не найдена.");
        }

        if (!expenseItem.IsActive)
        {
            return BadRequest("Нельзя использовать неактивную статью расхода.");
        }

        // Проверка дневного лимита: сумма всех транзакций за этот день + новая сумма <= 1 000 000
        var dayStart = request.Date.Date;
        var dayEnd = dayStart.AddDays(1);

        var dailySum = await _context.Transactions
            .Where(t => t.Date >= dayStart && t.Date < dayEnd)
            .SumAsync(t => t.Amount);

        if (dailySum + request.Amount > 1_000_000m)
        {
            return BadRequest("Суммарные траты за день превышают лимит в 1 000 000 рублей.");
        }

        var transaction = new Transaction
        {
            Date = request.Date.Date,
            Amount = request.Amount,
            Comment = request.Comment,
            ExpenseItemId = request.ExpenseItemId
        };

        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();

        // Загружаем связанную статью для ответа
        await _context.Entry(transaction).Reference(t => t.ExpenseItem).LoadAsync();

        var response = new TransactionResponse
        {
            Id = transaction.Id,
            Date = transaction.Date,
            Amount = transaction.Amount,
            Comment = transaction.Comment,
            ExpenseItemId = transaction.ExpenseItemId,
            ExpenseItemName = transaction.ExpenseItem.Name
        };

        return CreatedAtAction(nameof(GetById), new { id = transaction.Id }, response);
    }

    /// <summary>
    /// Обновить транзакцию.
    /// Нельзя изменить статью расхода, если она стала неактивной после создания транзакции.
    /// </summary>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, SaveTransactionRequest request)
    {
        var transaction = await _context.Transactions
            .Include(t => t.ExpenseItem)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (transaction is null)
        {
            return NotFound();
        }

        // Если пытаются изменить статью расхода
        if (request.ExpenseItemId != transaction.ExpenseItemId)
        {
            // Проверить, была ли исходная статья неактивной? По условию: "Если статья расхода стала неактивной после заведения транзакции,
            // запретить редактирование поля 'Статья расхода' для данной транзакции."
            // Здесь это означает, что если исходная статья неактивна, менять поле нельзя.
            if (!transaction.ExpenseItem.IsActive)
            {
                return BadRequest("Нельзя изменить статью расхода, так как исходная статья стала неактивной.");
            }

            // Проверить, что новая статья существует и активна
            var newItem = await _context.ExpenseItems.FindAsync(request.ExpenseItemId);
            if (newItem is null)
            {
                return BadRequest("Новая статья расхода не найдена.");
            }

            if (!newItem.IsActive)
            {
                return BadRequest("Новая статья расхода неактивна.");
            }

            transaction.ExpenseItemId = request.ExpenseItemId;
        }

        // Проверка дневного лимита с учётом изменения суммы
        var dayStart = request.Date.Date;
        var dayEnd = dayStart.AddDays(1);

        // Получаем сумму всех транзакций за этот день, исключая текущую
        var dailySumWithoutCurrent = await _context.Transactions
            .Where(t => t.Date >= dayStart && t.Date < dayEnd && t.Id != id)
            .SumAsync(t => t.Amount);

        if (dailySumWithoutCurrent + request.Amount > 1_000_000m)
        {
            return BadRequest("Суммарные траты за день превышают лимит в 1 000 000 рублей.");
        }

        transaction.Date = request.Date.Date;
        transaction.Amount = request.Amount;
        transaction.Comment = request.Comment;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Удалить транзакцию.
    /// </summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var transaction = await _context.Transactions.FindAsync(id);

        if (transaction is null)
        {
            return NotFound();
        }

        _context.Transactions.Remove(transaction);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Получить цвет стикера для заданного дня в зависимости от суммы трат.
    /// </summary>
    /// <param name="date">Дата в формате ГГГГ-ММ-ДД</param>
    /// <returns>Объект с суммой и цветом (green, yellow, red).</returns>
    [HttpGet("sticker")]
    public async Task<ActionResult> GetSticker([FromQuery] DateTime date)
    {
        var dayStart = date.Date;
        var dayEnd = dayStart.AddDays(1);

        var dailySum = await _context.Transactions
            .Where(t => t.Date >= dayStart && t.Date < dayEnd)
            .SumAsync(t => t.Amount);

        string color;
        if (dailySum < 500)
        {
            color = "green";
        }
        else if (dailySum <= 2000)
        {
            color = "yellow";
        }
        else
        {
            color = "red";
        }

        return Ok(new
        {
            Date = date.Date.ToString("yyyy-MM-dd"),
            TotalAmount = dailySum,
            StickerColor = color
        });
    }
}