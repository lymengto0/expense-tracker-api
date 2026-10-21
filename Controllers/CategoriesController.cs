using ExpenseTracker.Api.Data;
using ExpenseTracker.Api.Dtos;
using ExpenseTracker.Api.Extensions;
using ExpenseTracker.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly AppDbContext _db;

    public CategoriesController(AppDbContext db)
    {
        _db = db;
    }

    // GET /api/categories
    [HttpGet]
    public async Task<ActionResult<List<CategoryDto>>> GetAll()
    {
        var userId = User.GetUserId();

        var items = await _db.Categories
            .Where(c => c.UserId == userId)
            .OrderBy(c => c.Name)
            .Select(c => new CategoryDto { Id = c.Id, Name = c.Name })
            .ToListAsync();

        return Ok(items);
    }

    // GET /api/categories/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<CategoryDto>> GetById(int id)
    {
        var userId = User.GetUserId();

        var c = await _db.Categories
            .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

        if (c is null) return NotFound();

        return Ok(new CategoryDto { Id = c.Id, Name = c.Name });
    }

    // POST /api/categories
    [HttpPost]
    public async Task<ActionResult<CategoryDto>> Create(CategoryCreateDto dto)
    {
        var userId = User.GetUserId();
        var name = dto.Name.Trim();

        if (string.IsNullOrWhiteSpace(name))
            return BadRequest(new { message = "Name is required." });

        var exists = await _db.Categories
            .AnyAsync(c => c.UserId == userId && c.Name == name);
        if (exists)
            return Conflict(new { message = "You already have a category with this name." });

        var category = new Category { Name = name, UserId = userId };
        _db.Categories.Add(category);
        await _db.SaveChangesAsync();

        var result = new CategoryDto { Id = category.Id, Name = category.Name };
        return CreatedAtAction(nameof(GetById), new { id = category.Id }, result);
    }

    // PUT /api/categories/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, CategoryCreateDto dto)
    {
        var userId = User.GetUserId();
        var name = dto.Name.Trim();

        if (string.IsNullOrWhiteSpace(name))
            return BadRequest(new { message = "Name is required." });

        var category = await _db.Categories
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);
        if (category is null) return NotFound();

        var duplicate = await _db.Categories
            .AnyAsync(c => c.UserId == userId && c.Name == name && c.Id != id);
        if (duplicate)
            return Conflict(new { message = "You already have a category with this name." });

        category.Name = name;
        await _db.SaveChangesAsync();

        return NoContent();
    }

    // DELETE /api/categories/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = User.GetUserId();

        var category = await _db.Categories
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);
        if (category is null) return NotFound();

        var inUse = await _db.Transactions.AnyAsync(t => t.CategoryId == id);
        if (inUse)
            return Conflict(new { message = "This category has transactions and can't be deleted." });

        _db.Categories.Remove(category);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}