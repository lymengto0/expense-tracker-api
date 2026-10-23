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
public class TransactionsController : ControllerBase
{
    private readonly AppDbContext _db;

    public TransactionsController(AppDbContext db)
    {
        _db = db;
    }

    // GET /api/transactions?fromDate=&toDate=&categoryId=&type=&page=1&pageSize=10
    [HttpGet]
    public async Task<ActionResult<PagedResult<TransactionDto>>> GetAll([FromQuery] TransactionQueryDto q)
    {
        var userId = User.GetUserId();

        var page = q.Page < 1 ? 1 : q.Page;
        var pageSize = q.PageSize < 1 ? 10 : Math.Min(q.PageSize, 100);

        var query = _db.Transactions
            .AsNoTracking()
            .Where(t => t.UserId == userId);

        if (q.FromDate.HasValue) query = query.Where(t => t.Date >= q.FromDate.Value);
        if (q.ToDate.HasValue)   query = query.Where(t => t.Date <= q.ToDate.Value);
        if (q.CategoryId.HasValue) query = query.Where(t => t.CategoryId == q.CategoryId.Value);
        if (q.Type.HasValue)     query = query.Where(t => t.Type == q.Type.Value);

        var total = await query.CountAsync();

        var items = await query
            .OrderByDescending(t => t.Date)
            .ThenByDescending(t => t.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(t => new TransactionDto
            {
                Id = t.Id,
                Amount = t.Amount,
                Type = t.Type,
                Note = t.Note,
                Date = t.Date,
                CategoryId = t.CategoryId,
                CategoryName = t.Category!.Name
            })
            .ToListAsync();

        return Ok(new PagedResult<TransactionDto>
        {
            Items = items,
            Total = total,
            Page = page,
            PageSize = pageSize
        });
    }

    // GET /api/transactions/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<TransactionDto>> GetById(int id)
    {
        var userId = User.GetUserId();

        var t = await _db.Transactions
            .AsNoTracking()
            .Include(x => x.Category)
            .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

        if (t is null) return NotFound();
        return Ok(ToDto(t));
    }

    // POST /api/transactions
    [HttpPost]
    public async Task<ActionResult<TransactionDto>> Create(TransactionCreateDto dto)
    {
        var userId = User.GetUserId();

        var category = await _db.Categories
            .FirstOrDefaultAsync(c => c.Id == dto.CategoryId && c.UserId == userId);
        if (category is null)
            return BadRequest(new { message = "Category not found." });

        var t = new Transaction
        {
            UserId = userId,
            CategoryId = dto.CategoryId,
            Amount = dto.Amount,
            Type = dto.Type,
            Note = dto.Note?.Trim(),
            Date = dto.Date
        };

        _db.Transactions.Add(t);
        await _db.SaveChangesAsync();

        t.Category = category;
        return CreatedAtAction(nameof(GetById), new { id = t.Id }, ToDto(t));
    }

    // PUT /api/transactions/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, TransactionCreateDto dto)
    {
        var userId = User.GetUserId();

        var t = await _db.Transactions
            .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
        if (t is null) return NotFound();

        var categoryOk = await _db.Categories
            .AnyAsync(c => c.Id == dto.CategoryId && c.UserId == userId);
        if (!categoryOk)
            return BadRequest(new { message = "Category not found." });

        t.CategoryId = dto.CategoryId;
        t.Amount = dto.Amount;
        t.Type = dto.Type;
        t.Note = dto.Note?.Trim();
        t.Date = dto.Date;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    // DELETE /api/transactions/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = User.GetUserId();

        var t = await _db.Transactions
            .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
        if (t is null) return NotFound();

        _db.Transactions.Remove(t);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    private static TransactionDto ToDto(Transaction t) => new()
    {
        Id = t.Id,
        Amount = t.Amount,
        Type = t.Type,
        Note = t.Note,
        Date = t.Date,
        CategoryId = t.CategoryId,
        CategoryName = t.Category?.Name ?? string.Empty
    };
}