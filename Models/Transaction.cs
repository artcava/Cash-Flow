namespace CashFlow.Models;

public class Transaction
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int ActivityId { get; set; }
    public Activity? Activity { get; set; } // Navigazione
    public int AccountId { get; set; }
    public Account? Account { get; set; } // Navigazione
}