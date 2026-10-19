namespace ExpenseTracker.Api.Models;

public enum TransactionType
{
    Income = 0,
    Expense = 1
}

public class Transaction
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public TransactionType Type { get; set; }
    public string? Note { get; set; }
    public DateTime Date { get; set; }

    public int UserId { get; set; }
    public User? User { get; set; }

    public int CategoryId { get; set; }
    public Category? Category { get; set; }
}