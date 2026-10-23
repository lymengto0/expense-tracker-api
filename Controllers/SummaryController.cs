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
public class SummaryController : ControllerBase
{
    private readonly AppDbContext _db;

    public SummaryController(AppDbContext db)
    {
        _db = db;
    }

    // GET /api/summary?month=10&year=2026
    [HttpGet]
    public async Task<ActionResult<SummaryDto>> Get([FromQuery] int? month, [FromQuery] int? year)
    {
        var userId = User.GetUserId();

        var now = DateTime.UtcNow;
        var m = month ?? now.Month;
        var y = year ?? now.Year;

        if (m < 1 || m > 12)
            return BadRequest(new { message = "Month must be between 1 and 12." });
        if (y < 2000 || y > 2100)
            return BadRequest(new { message = "Year must be between 2000 and 2100." });

        // Date range: first day of the month up to (not including) the first day of next month
        var start = new DateTime(y, m, 1);
        var end = start.AddMonths(1);

        var monthQuery = _db.Transactions
            .AsNoTracking()
            .Where(t => t.UserId == userId && t.Date >= start && t.Date < end);

        // 1) Totals by type
        var totals = await monthQuery
            .GroupBy(t => t.Type)
            .Select(g => new { Type = g.Key, Total = g.Sum(x => x.Amount) })
            .ToListAsync();

        var income = totals.FirstOrDefault(x => x.Type == TransactionType.Income)?.Total ?? 0m;
        var expense = totals.FirstOrDefault(x => x.Type == TransactionType.Expense)?.Total ?? 0m;

        // 2) Expense totals by category (for the chart)
        var byCategory = await monthQuery
            .Where(t => t.Type == TransactionType.Expense)
            .GroupBy(t => new { t.CategoryId, t.Category!.Name })
            .Select(g => new CategorySpendDto
            {
                CategoryId = g.Key.CategoryId,
                CategoryName = g.Key.Name,
                Total = g.Sum(x => x.Amount)
            })
            .OrderByDescending(x => x.Total)
            .ToListAsync();

        // 3) Percent of total expenses, calculated in memory
        foreach (var c in byCategory)
            c.Percent = expense > 0 ? Math.Round(c.Total / expense * 100, 1) : 0;

        return Ok(new SummaryDto
        {
            Month = m,
            Year = y,
            TotalIncome = income,
            TotalExpense = expense,
            Balance = income - expense,
            ByCategory = byCategory
        });
    }
}