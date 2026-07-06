using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FinalTask.Data;
using FinalTask.Dtos;
using FinalTask.Models;

namespace FinalTask.Controllers;

/// <summary>
/// Управление категориями расходов.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly FinanceDbContext _context;

    public CategoriesController(FinanceDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Получить все категории.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryResponse>>> GetAll()
    {
        var categories = await _context.Categories
            .Select(c => new CategoryResponse
            {
                Id = c.Id,
                Name = c.Name,
                MonthlyBudget = c.MonthlyBudget,
                IsActive = c.IsActive
            })
            .ToListAsync();

        return Ok(categories);
    }

    /// <summary>
    /// Получить категорию по Id.
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<CategoryResponse>> GetById(int id)
    {
        var category = await _context.Categories.FindAsync(id);

        if (category is null)
        {
            return NotFound();
        }

        return Ok(new CategoryResponse
        {
            Id = category.Id,
            Name = category.Name,
            MonthlyBudget = category.MonthlyBudget,
            IsActive = category.IsActive
        });
    }

    /// <summary>
    /// Создать новую категорию.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<CategoryResponse>> Create(SaveCategoryRequest request)
    {
        var category = new Category
        {
            Name = request.Name,
            MonthlyBudget = request.MonthlyBudget,
            IsActive = request.IsActive
        };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        var response = new CategoryResponse
        {
            Id = category.Id,
            Name = category.Name,
            MonthlyBudget = category.MonthlyBudget,
            IsActive = category.IsActive
        };

        return CreatedAtAction(nameof(GetById), new { id = category.Id }, response);
    }

    /// <summary>
    /// Обновить существующую категорию.
    /// </summary>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, SaveCategoryRequest request)
    {
        var category = await _context.Categories.FindAsync(id);

        if (category is null)
        {
            return NotFound();
        }

        category.Name = request.Name;
        category.MonthlyBudget = request.MonthlyBudget;
        category.IsActive = request.IsActive;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Удалить категорию.
    /// </summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var category = await _context.Categories.FindAsync(id);

        if (category is null)
        {
            return NotFound();
        }

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}