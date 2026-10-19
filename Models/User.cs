namespace ExpenseTracker.Api.Models;

public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<Category> Categories { get; set; } = new();
    public List<Transaction> Transactions { get; set; } = new();
}