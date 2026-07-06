using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FinalTask.Data;
using FinalTask.Dtos;
using FinalTask.Models;

namespace FinalTask.Controllers;

/// <summary>
/// Управление статьями расходов.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ExpenseItemsController : ControllerBase
{
    private readonly FinanceDbContext _context;

    public ExpenseItemsController(FinanceDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Получить все статьи расходов.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ExpenseItemResponse>>> GetAll()
    {
        var items = await _context.ExpenseItems
            .Include(e => e.Category)
            .Select(e => new ExpenseItemResponse
            {
                Id = e.Id,
                Name = e.Name,
                CategoryId = e.CategoryId,
                CategoryName = e.Category.Name,
                IsActive = e.IsActive
            })
            .ToListAsync();

        return Ok(items);
    }

    /// <summary>
    /// Получить статью по Id.
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ExpenseItemResponse>> GetById(int id)
    {
        var item = await _context.ExpenseItems
            .Include(e => e.Category)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (item is null)
        {
            return NotFound();
        }

        return Ok(new ExpenseItemResponse
        {
            Id = item.Id,
            Name = item.Name,
            CategoryId = item.CategoryId,
            CategoryName = item.Category.Name,
            IsActive = item.IsActive
        });
    }

    /// <summary>
    /// Создать новую статью расхода.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ExpenseItemResponse>> Create(SaveExpenseItemRequest request)
    {
        var categoryExists = await _context.Categories.AnyAsync(c => c.Id == request.CategoryId);
        if (!categoryExists)
        {
            return BadRequest("Указанная категория не существует.");
        }

        var item = new ExpenseItem
        {
            Name = request.Name,
            CategoryId = request.CategoryId,
            IsActive = request.IsActive
        };

        _context.ExpenseItems.Add(item);
        await _context.SaveChangesAsync();

        // Загружаем категорию для ответа
        await _context.Entry(item).Reference(e => e.Category).LoadAsync();

        var response = new ExpenseItemResponse
        {
            Id = item.Id,
            Name = item.Name,
            CategoryId = item.CategoryId,
            CategoryName = item.Category.Name,
            IsActive = item.IsActive
        };

        return CreatedAtAction(nameof(GetById), new { id = item.Id }, response);
    }

    /// <summary>
    /// Обновить статью расхода.
    /// </summary>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, SaveExpenseItemRequest request)
    {
        var item = await _context.ExpenseItems.FindAsync(id);

        if (item is null)
        {
            return NotFound();
        }

        var categoryExists = await _context.Categories.AnyAsync(c => c.Id == request.CategoryId);
        if (!categoryExists)
        {
            return BadRequest("Указанная категория не существует.");
        }

        item.Name = request.Name;
        item.CategoryId = request.CategoryId;
        item.IsActive = request.IsActive;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Удалить статью расхода.
    /// </summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _context.ExpenseItems.FindAsync(id);

        if (item is null)
        {
            return NotFound();
        }

        _context.ExpenseItems.Remove(item);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}