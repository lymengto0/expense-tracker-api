namespace ExpenseTracker.Api.Dtos;

public class CategorySpendDto
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public decimal Percent { get; set; }
}

public class SummaryDto
{
    public int Month { get; set; }
    public int Year { get; set; }
    public decimal TotalIncome { get; set; }
    public decimal TotalExpense { get; set; }
    public decimal Balance { get; set; }
    public List<CategorySpendDto> ByCategory { get; set; } = new();
}