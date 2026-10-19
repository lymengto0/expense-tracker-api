namespace ExpenseTracker.Api.Models;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public int UserId { get; set; }
    public User? User { get; set; }

    public List<Transaction> Transactions { get; set; } = new();
}